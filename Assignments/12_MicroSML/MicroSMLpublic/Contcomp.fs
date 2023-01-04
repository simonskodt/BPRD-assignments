(* File MicroSML/Contcomp.fs

   A compiler from Micro-SML to an abstract machine.

   Backwards compilation with peephole optimizations corresponding to
   Chapter 12.

*)

module Comp

open System.IO
open Absyn
open TypeInference
open Machine

(* Compiler flags. *)
let debug_p = ref false
let opt_p = ref false
let verbose_p = ref false

let debug s = if !debug_p then printf "\nDEBUG %s\n" s

(* Accumulate code from functions in a mutable list. This makes the
   function cExpr easier to read as the accumulator is not needed *)

let (resetFuncs,addFunc,getFuncs,
     resetGlobalInit,addGlobalInit,getGlobalInit) =
  let funcInsts : instr list list ref = ref []
  let globalInsts : instr list list ref = ref []
  let reset iss = fun () -> iss := []
  let add iss lab insts = iss := (Label lab::insts) :: !iss
  let get iss = fun () -> (List.concat << List.rev) !iss
  (reset funcInsts,   add funcInsts,   get funcInsts,
   reset globalInsts, add globalInsts, get globalInsts)

(* A global variable has an absolute address, a local one has an offset: *)
type var = 
  | Glovar of int                   (* absolute address in stack           *)
  | Locvar of int                   (* address relative to bottom of frame *)
  | Closvar of int                  (* address relative to closure *)

let ppVar = function
  | Glovar o  -> "Glovar[" + o.ToString() + "]"
  | Locvar o  -> "Locvar[" + o.ToString() + "]"
  | Closvar o -> "Closvar[" + o.ToString() + "]"

(* The variable environment keeps track of global and local variables, and 
   keeps track of next available offset for local variables *)
type varEnv = var env * int

let incVarIdx (env,idx) i = (env,idx+i)

let ppVarEnv (env,fDepth) =
  "\nfDepth = " + fDepth.ToString() + "\n" +
  "[ " + (String.concat "\n" (List.map (fun (x,v) -> x + " |-> " + (ppVar v)) env)) + " ]\n"

(* Global Exception Number Generator *)
let exnNumVar = "__exnNum__"  
let exnNumVarEnv = ([(exnNumVar,Glovar 0)],1)

(* Simple environment operations *)
type 'data env = (string * 'data) list
let rec lookup env x = 
  match env with 
  | []         -> if x = exnNumVar then snd (List.head (fst exnNumVarEnv))  (* TODO eliminate need *)
                  else  failwith ("Comp.lookup: " + x + " not found")
  | (y, v)::yr -> if x=y then v else lookup yr x

let filterGlobalsInScope (env,_) fvs =
  Set.filter (fun fv -> match lookup env fv with Glovar _ -> true | _ -> false) fvs

(* Code-generating functions that perform local optimizations, see Chapter 12. *)

let rec addINCSP m1 C : instr list =
  match (!opt_p,C) with
  | (true,INCSP m2            :: C1) -> addINCSP (m1+m2) C1
  | (true,RET m2              :: C1) -> RET (m2-m1) :: C1
  | (true,Label lab :: RET m2 :: _)  -> RET (m2-m1) :: C  (* Becomes: RET(m2-m1)::Label lab::RET m2 *)
  | _                                -> if m1=0 then C else INCSP m1 :: C

let addLabel C : label * instr list =          (* Conditional jump to C *)
  match (!opt_p,C) with
  | (true,Label lab :: _) -> (lab, C)
  | (true,GOTO lab :: _)  -> (lab, C)
  | _                     -> let lab = newLabel() 
                             (lab, Label lab :: C)

let makeJump C : instr * instr list =          (* Unconditional jump to C *)
  match (!opt_p,C) with
  | (true,RET m              :: _) -> (RET m, C)
  | (true,Label lab :: RET m :: _) -> (RET m, C)
  | (true,Label lab          :: _) -> (GOTO lab, C)
  | (true,GOTO lab           :: _) -> (GOTO lab, C)
  | _                              -> let lab = newLabel() 
                                      (GOTO lab, Label lab :: C)

let rec deadcode C = (* Remove all code until next label *)
  match (!opt_p,C) with
  | (true,[])             -> []
  | (true,Label lab :: _) -> C
  | (true,_ :: C1)        -> deadcode C1
  | (false,_)             -> C

let addNOT C =
  match (!opt_p,C) with
  | (true,NOT        :: C1) -> C1
  | (true,IFZERO lab :: C1) -> IFNZRO lab :: C1 
  | (true,IFNZRO lab :: C1) -> IFZERO lab :: C1 
  | _                       -> NOT :: C

let addJump jump C =                    (* jump is GOTO or RET *)
  if !opt_p then  
    let C1 = deadcode C
    match (jump,C1) with
    | (GOTO lab1, Label lab2 :: _) -> if lab1=lab2 then C1 
                                      else GOTO lab1 :: C1
    | _                            -> jump :: C1
  else
    jump :: C                                         

let addGOTO lab C =
  addJump (GOTO lab) C

let rec addCST i C =
  if !opt_p then    
    match (i, C) with
    | (0, ADD        :: C1) -> C1
    | (0, SUB        :: C1) -> C1
    | (0, NOT        :: C1) -> addCST 1 C1
    | (_, NOT        :: C1) -> addCST 0 C1
    | (1, MUL        :: C1) -> C1
    | (1, DIV        :: C1) -> C1
    | (0, EQ         :: C1) -> addNOT C1
    | (_, INCSP m    :: C1) -> if m < 0 then addINCSP (m+1) C1
                               else CSTI i :: C
    | (0, IFZERO lab :: C1) -> addGOTO lab C1
    | (_, IFZERO lab :: C1) -> C1
    | (0, IFNZRO lab :: C1) -> C1
    | (_, IFNZRO lab :: C1) -> addGOTO lab C1
    | _                     -> CSTI i :: C
  else    
    CSTI i :: C

(* Leave content of variable x at top of the stack *)
let loadVar varEnv x C : instr list =
  match lookup (fst varEnv) x with
  | Glovar addr  -> addCST addr (LDI :: C)
  | Locvar offset  -> GETBP :: addCST offset (ADD :: LDI :: C)
  | Closvar offset -> GETBP :: LDI :: HEAPLDI offset :: C (* First access closure and then offset into closure *)

(* Code for Generative Exception Numbering *)
let nextExnNumCode varEnv C =
  match lookup (fst varEnv) exnNumVar with
    (* Leave new exception number at top of the stack and update global variable *)
  | Glovar addr -> addCST addr (addCST addr (LDI :: addCST 1 (ADD :: STI :: C)))
  | _ -> failwith "Contcomp.nextExnNumCode.Global exception variable is not in the environment"
let initExnNumCode addr = addCST addr (addCST 0 [STI])

(* Compiling Micro-SML expressions: 
   * kind    is context for variable, either local or global.
   * varEnv  is the local and gloval variable environment
   * e       is the expression to compile
   * C       is the code following the code for this expression   
*)
let rec cExpr (kind: int->var) (varEnv : varEnv) (e : expr<typ>) (C: instr list) : instr list =
  let (env,fdepth) = varEnv
  match e with
  | CstI (i,_) -> addCST i C
  | CstB (b,_) -> addCST (if b then 1 else 0) C
  | CstN _     -> NIL :: C
  | Var (x,_)  -> loadVar varEnv x C
  | Prim1(ope,e1,_) ->
    cExpr kind varEnv e1 
      (match (ope,getTypExpr e1) with
       | ("print",TypI) -> PRINTI :: C
       | ("print",TypB) -> PRINTB :: C
       | ("print",TypL _) -> PRINTL :: C
       | ("print",t) ->
         debug ("Warning: cExpr.Prim1: print not implemented on type " + (TypeInference.showType t)); C
       | ("hd",_)    -> CAR :: C  
       | ("tl",_)    ->  CDR :: C
       | ("isnil",_) -> NIL :: EQ :: C
       | _ -> failwith ("cExpr.Prim1 "+ope+" not implemented"))
  | Prim2(ope, e1, e2,_) ->
    cExpr kind varEnv e1
      (cExpr kind (incVarIdx varEnv 1) e2 
        (match ope with
         | "*" -> MUL :: C
         | "+" -> ADD :: C
         | "-" -> SUB :: C
         | "%" -> MOD :: C
         | "=" -> EQ :: C
         | "<>" -> EQ :: addNOT C
         | "<" -> LT :: C
         | ">" -> SWAP :: LT :: C
         | "<=" -> SWAP :: LT :: addNOT C
         | ">=" -> LT :: addNOT C
         | "::" -> CONS :: C
         | _ -> failwith ("cExpr.prim2 " + ope + " not implemented")))
  | AndAlso(e1,e2,_) ->
    match C with
    | IFZERO lab :: _ ->
       cExpr kind varEnv e1 (IFZERO lab :: cExpr kind varEnv e2 C)
    | IFNZRO labthen :: C1 -> 
      let (labelse, C2) = addLabel C1
      cExpr kind varEnv e1
        (IFZERO labelse 
          :: cExpr kind varEnv e2 (IFNZRO labthen :: C2))
    | _ ->
      let (jumpend,  C1) = makeJump C
      let (labfalse, C2) = addLabel (addCST 0 C1)
      cExpr kind varEnv e1
        (IFZERO labfalse 
          :: cExpr kind varEnv e2 (addJump jumpend C2))
  | OrElse(e1,e2,_) ->
    match C with
    | IFNZRO lab :: _ -> 
      cExpr kind varEnv e1 (IFNZRO lab :: cExpr kind varEnv e2 C)
    | IFZERO labthen :: C1 ->
      let(labelse, C2) = addLabel C1
      cExpr kind varEnv e1 
        (IFNZRO labelse :: cExpr kind varEnv e2
          (IFZERO labthen :: C2))
    | _ ->
      let (jumpend, C1) = makeJump C
      let (labtrue, C2) = addLabel(addCST 1 C1)
      cExpr kind varEnv e1
        (IFNZRO labtrue :: cExpr kind varEnv e2 (addJump jumpend C2))
  | Seq(e1,e2,_) ->
    cExpr kind varEnv e1 
      (addINCSP -1 (* Remove result of e1 *)
        (cExpr kind varEnv e2 C))
  | Let(valdecs,letBody) ->
    let ((_,fdepth') as bodyEnv,vdEnvs) =
      List.fold (fun (accEnv,accVdEnv) vd -> let (nextEnv,vdEnv) = genValdecEnv kind accEnv vd
                                             (nextEnv,(vd,vdEnv)::accVdEnv)) (varEnv,[]) valdecs 
    let vdEnvs' = List.rev vdEnvs
    let numVals = fdepth' - fdepth
    let iVals C = List.foldBack (cValdec kind) vdEnvs' C
    let addrFirstVal C = GETSP :: addCST (numVals - 1) (SUB :: C)
    let iBody C = cExpr kind (incVarIdx bodyEnv 1) letBody C (* Make room for addrFirstVal *)
    iVals (addrFirstVal (iBody (STI :: addINCSP -numVals C)))
  | If(e1, e2, e3) ->
    let (jumpend, C1) = makeJump C
    let (labelse, C2) = addLabel (cExpr kind varEnv e3 C1)
    cExpr kind varEnv e1 (IFZERO labelse
      :: cExpr kind varEnv e2 (addJump jumpend C2))                        
  | Fun(x,fBody,_) ->
    let funcLab = newLabel()
    (* To minimize closures, we do not copy globals in scope in closure. *)    
    let fvsAll = freevars fBody - (set [x])
    let fvsGlobalInScope = filterGlobalsInScope varEnv fvsAll
    let fvsClos = Set.toList (fvsAll - fvsGlobalInScope)
    let _ = debug ("FN: " + funcLab + ", parameter: " + x + ", fvs in clos: " + (ppFreevars fvsClos))
    (* Closure at index 0; argument at index 1; fv1 at index 1 in closure; idx 0 is code pointer. *)
    let bodyEnv = (x, Locvar 1) ::
                  (List.mapi (fun i x -> (x,Closvar (i+1))) fvsClos) @
                  (List.map (fun x -> (x,lookup (fst varEnv) x)) (Set.toList fvsGlobalInScope))
    (* Add function to global program. Closure/Arg at index 0/1 *)
    let _ = addFunc funcLab (cExpr Locvar (bodyEnv,2) fBody [RET 2])
    let sizeClos = List.length fvsClos + 1
    let codeFreevars' = List.foldBack (loadVar varEnv) fvsClos (ACLOS sizeClos :: HEAPSTI sizeClos :: C)  
    PUSHLAB funcLab :: codeFreevars'
  | Call(eFun, eArg,tOpt,_) ->
    let cInst C = match (!opt_p,tOpt) with (true,Some true) -> TCLOSCALL 1 :: deadcode C | _ -> CLOSCALL 1 :: C
    cExpr kind varEnv eFun (cExpr kind (incVarIdx varEnv 1) eArg (cInst C))
  | Raise(e,_) -> cExpr kind varEnv e (THROW :: deadcode C) 
  | TryWith(e1,ExnVar exn,e2) -> (* jump and label optimizations left as exercise TODO *)
    let labend = newLabelWName "TryWithEnd"
    let labexn = newLabelWName ("TryWith_" + exn)
    loadVar varEnv exn
      (PUSHHDLR labexn ::
        (cExpr kind (incVarIdx varEnv 3 (* Handler size = 3 *)) e1 
          (POPHDLR :: GOTO labend :: Label labexn ::
            (cExpr kind varEnv e2 (Label labend :: C)))))

(* genValdecEnv returns two environments (nextEnv,vdEnv):
     - nextEnv is the environment to be used by the following vd or body
     - vdEnv is the environment to be used by the vd at hand.
   Notice the difference. For Fundecs nextEnv and vdEnv is the same because
   all functions in the group can be used in the body for each function.
   This is NOT the case for let bound variables where the variable is not in
   scope until the following vd or body.
*)
and genValdecEnv (kind: int->var) ((env,fdepth) as curEnv) vd =
  match vd with        
  | Fundecs fs ->
    let newEnv = List.fold (fun (env,fdepth) (f,x,fBody) ->
                             ((f,kind fdepth) :: env, fdepth+1)) curEnv fs
    (newEnv,newEnv)
  | Valdec (x,eRhs) -> (((x,kind fdepth) :: env, fdepth+1),curEnv)
  | Exn(ExnVar exn,_) -> (((exn,kind fdepth) :: env, fdepth+1),curEnv)
  
and cValdec (kind: int->var) (vd:valdec<typ>, varEnv: varEnv) (C: instr list) : instr list =
  debug ("cValdec with varEnv = " + (ppVarEnv varEnv));
  (* varEnv has been precalculated to be the environment for the valdec to compile *)        
  match vd with
  | Fundecs fs ->
    (* Calculate fvs for each function in fs. *)
    let fsfvs =
      List.map (fun (f,x,fBody) ->
                  (* To minimize closures, do not copy globals in scope in closure. *)
                  let fvsAll = freevars fBody - (set [x;f])
                  let fvsGlobalInScope = filterGlobalsInScope varEnv fvsAll
                  let fvsClos = Set.toList (fvsAll - fvsGlobalInScope)
                  let _ = debug ("Fundecs: "+f+", parameter: "+x+", fvs in clos: "+(ppFreevars fvsClos))
                  let labFunc = newLabelWName ("LabFunc_" + f)
                  (f,x,fBody,fvsGlobalInScope,fvsClos,List.length fvsClos + 1,labFunc)) fs
    (* Code to allocate closures *)
    let iaClos C = List.foldBack (fun (_,_,_,_,_,sizeClos,_) C -> ACLOS sizeClos :: C) fsfvs C
    (* Code to copy free variables and code label to each closure *)
    let iFillClos C =
      List.foldBack (fun (f,_,_,_,fvsClos,sizeClos,funcLab) C ->
                       let codefvsClos C = List.foldBack (loadVar varEnv) fvsClos C
                       let codeClosPtr C = loadVar varEnv f C
                       PUSHLAB funcLab :: codefvsClos (codeClosPtr (HEAPSTI sizeClos :: addINCSP -1 C))) fsfvs C
    (* Generate code for each function body *)
    let codefBody (f,x,fBody,fvsGlobalInScope,fvsClos,_,funcLab) =
      (* Closure at index 0, argument at index 1, fv1 at index 1 in closure; idx 0 is code pointer. *)
      let varEnvBody = (f, Locvar 0) :: (x, Locvar 1) ::
                       (List.mapi (fun i x -> (x,Closvar (i+1))) fvsClos) @
                        (List.map (fun x -> (x,lookup (fst varEnv) x)) (Set.toList fvsGlobalInScope))
      addFunc funcLab (cExpr Locvar (varEnvBody,2) fBody [RET 2])
    let _ = List.iter codefBody fsfvs
    let insts = iaClos (iFillClos C)
    insts

  | Valdec (x,eRhs) -> cExpr kind varEnv eRhs C

  | Exn(ExnVar exn,_) ->
    (* Code to push next exn number on stack *)
    nextExnNumCode varEnv C

and cProgram (p:program<typ>) : instr list =
  let _ = resetLabels()
  let _ = resetFuncs()
  let _ = resetGlobalInit()
  let labMain = newLabel()
  let initEnv = exnNumVarEnv (* Global exception number as first global variable, addr 0 *)
  let _ = addGlobalInit (newLabelWName "G_ExnVar") (initExnNumCode 0) 
  match p with
    | Prog(valdecs,e) ->
    let ((bodyEnv,fdepth'),vdEnvs) =
      List.fold (fun (accEnv,accVdEnv) vd -> let (nextEnv,vdEnv) = genValdecEnv Glovar accEnv vd
                                             (nextEnv,(vd,vdEnv)::accVdEnv)) (initEnv,[]) valdecs
    let vdEnvs' = List.rev vdEnvs
    let numVals = fdepth'   (* Remove everything below the end result value. *)
    let iVals C = List.foldBack (cValdec Glovar) vdEnvs' C
    let _ = addGlobalInit (newLabelWName "G_Valdecs") (iVals [])
    let _ = addFunc labMain ((cExpr Locvar (bodyEnv,0) e) [RET 0])
    getGlobalInit() @
    (GETSP :: addCST (numVals-1) (SUB :: CALL(0,labMain) :: STI :: addINCSP -numVals (STOP :: getFuncs())))
    
(* Compile a complete Micro-SML program and write the resulting instruction list
   to file fname; also, return the program as a list of instructions.
 *)

let intsToFile (inss : int list) (fname : string) = 
  File.WriteAllText(fname, String.concat " " (List.map string inss))

let compileToFile (opt_p',debug_p',verbose_p',program,fname) =
  let _ = (debug_p := debug_p'; opt_p := opt_p'; verbose_p := verbose_p') (* Set compiler flags *)
  let instrs   = cProgram program 
  let bytecode = code2ints instrs
  let _ = if !verbose_p then printfn "\nCompiled to %s\n%s\n" fname (ppInsts instrs) else ()
  intsToFile bytecode fname
  instrs

