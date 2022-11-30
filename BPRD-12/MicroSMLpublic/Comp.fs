(* File MicroSML/Comp.fs

   A compiler from Micro-SML to an abstract machine.

   Direct (forwards) compilation without optimization of jumps to
   jumps, tail-calls etc.

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
   function compExpr easier to read as the accumulator is not needed *)

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
  | []         -> if x = exnNumVar then snd (List.head (fst exnNumVarEnv))
                  else  failwith ("Comp.lookup: " + x + " not found")
  | (y, v)::yr -> if x=y then v else lookup yr x

let filterGlobalsInScope (env,_) fvs =
  Set.filter (fun fv -> match lookup env fv with Glovar _ -> true | _ -> false) fvs

(* Leave content of variable x at top of the stack *)
let loadVar varEnv x : instr list =
  //printf "env: %s" (ppVarEnv varEnv);
  match lookup (fst varEnv) x with
  | Glovar addr  -> [CSTI addr; LDI]
  | Locvar offset  -> [GETBP; CSTI offset; ADD; LDI]
  | Closvar offset -> [GETBP; LDI; HEAPLDI offset] (* First access closure and then offset into closure *)

(* Code for Generative Exception Numbering *)
let nextExnNumCode varEnv =
  //printf "%s\n" (ppVarEnv varEnv);
  match lookup (fst varEnv) exnNumVar with
    (* Leave new exception number at top of the stack and update global variable *)
  | Glovar addr -> [CSTI addr; CSTI addr; LDI; CSTI 1; ADD; STI]
  | _ -> failwith "Global exception variable is not in the environment"
let initExnNumCode addr = [CSTI addr; CSTI 0; STI]
(* Compiling Micro-SML expressions: 

   * e       is the expression to compile
   * varEnv  is the local and gloval variable environment 

   Net effect principle: if the compilation (compExpr e varEnv funEnv) of
   expression e returns the instruction sequence instrs, then the
   execution of instrs will leave the rvalue of expression e on the
   stack top (and thus extend the current stack frame with one element).  
*)
let rec compExpr (kind: int->var) (varEnv : varEnv) (e : expr<typ>) : instr list =
  let (env,fdepth) = varEnv
  match e with
  | CstI (i,_) -> [CSTI i]
  | CstB (b,_) -> if b then [CSTI 1] else [CSTI 0]
  | CstN _     -> [NIL]  
  | Var (x,_)  -> loadVar varEnv x
  | Prim1(ope,e1,_) ->
    compExpr kind varEnv e1 @
    (match (ope,getTypExpr e1) with
     | ("print",TypI) -> [PRINTI]
     | ("print",TypB) -> [PRINTB]
     | ("print",TypL _) -> [PRINTL] 
     | ("print",t) ->
       debug ("Warning: compExpr.Prim1: print not implemented on type " + (TypeInference.showType t)); []
     | ("hd",_)    -> [CAR]  
     | ("tl",_)    ->  [CDR]  
     | ("isnil",_) -> [NIL;EQ]
     | _ -> failwith ("compExpr.Prim1 "+ope+" not implemented"))
  | Prim2(ope, e1, e2,_) ->
    compExpr kind varEnv e1 @
    compExpr kind (incVarIdx varEnv 1) e2 @
    (match ope with
     | "*" -> [MUL]
     | "+" -> [ADD]
     | "-" -> [SUB]
     | "%" -> [MOD]
     | "=" -> [EQ]
     | "<>" -> [EQ;NOT]
     | "<" -> [LT]
     | ">" -> [SWAP;LT]
     | "<=" ->[SWAP;LT;NOT]
     | ">=" -> [LT;NOT]
     | "::" -> [CONS]
     | _ -> failwith ("compExpr.prim2 " + ope + " not implemented"))
  | AndAlso(e1,e2,_) ->
    let labend   = newLabel()
    let labfalse = newLabel()
    compExpr kind varEnv e1
    @ [IFZERO labfalse]
    @ compExpr kind varEnv e2
    @ [GOTO labend; Label labfalse; CSTI 0; Label labend]
  | OrElse(e1,e2,_) ->
    let labend  = newLabel()
    let labtrue = newLabel()
    compExpr kind varEnv e1
    @ [IFNZRO labtrue]
    @ compExpr kind varEnv e2
    @ [GOTO labend; Label labtrue; CSTI 1; Label labend]
  | Seq(e1,e2,_) ->
    compExpr kind varEnv e1
    @ [INCSP -1] (* Remove result of e1 *)
    @ compExpr kind varEnv e2
  | Let(valdecs,letBody) ->
    let (newEnv,numVals,iVals) =
      List.fold (fun (env,numVals,iVals) valdec -> let (newEnv,numVals',iVal) = compValdec kind env valdec
                                                   (newEnv,numVals+numVals',iVal::iVals)) (varEnv,0,[]) valdecs
    let addrFirstVal = [GETSP; CSTI (numVals - 1); SUB]      
    let iBody = compExpr kind (incVarIdx newEnv 1) letBody (* Make room for addrFirstVal *)
    List.concat (List.rev iVals) @ addrFirstVal @ iBody @ [STI;INCSP -numVals]
  | If(e1, e2, e3) ->
    let labelse = newLabel()
    let labend = newLabel()
    compExpr kind varEnv e1 @ [IFZERO labelse] @
    compExpr kind varEnv e2 @ [GOTO labend] @
    [Label labelse] @ compExpr kind varEnv e3 @
    [Label labend]
  | Fun(x,fBody,_) ->
    let funcLab = newLabel()
    (* To minimize closures, we do not copy globals in scope in closure. *)    
    let fvsAll = freevars fBody - (set [x])
    let fvsGlobalInScope = filterGlobalsInScope varEnv fvsAll
    let fvsClos = Set.toList (fvsAll - fvsGlobalInScope)
    let _ = debug ("FN: " + funcLab + ", parameter: " + x + ", fvs in clos: " + (ppFreevars fvsClos))
    let codeFreevars = List.map (loadVar varEnv) fvsClos 
    (* Closure at index 0, argument at index 1, fv1 at index 1 in closure; idx 0 is code pointer. *)
    let varEnv = (x, Locvar 1) ::
                 (List.mapi (fun i x -> (x,Closvar (i+1))) fvsClos) @
                 (List.map (fun x -> (x,lookup (fst varEnv) x)) (Set.toList fvsGlobalInScope))
    (* Add function to global program. Closure/Arg at index 0/1 *)
    let _ = addFunc funcLab (compExpr Locvar (varEnv,2) fBody @ [RET 2])
    let sizeClos = List.length fvsClos + 1
    [PUSHLAB funcLab] @ List.concat codeFreevars @ [ACLOS sizeClos; HEAPSTI sizeClos]
  | Call(eFun, eArg,tOpt,_) ->
    let cInst = match (!opt_p,tOpt) with (true,Some true) -> TCLOSCALL 1 | _ -> CLOSCALL 1      
    compExpr kind varEnv eFun @
    compExpr kind (incVarIdx varEnv 1) eArg @
    [cInst]
  | Raise(e,_) -> compExpr kind varEnv e @ [THROW]
  | TryWith(e1,ExnVar exn,e2) ->
    let labend = newLabelWName "TryWithEnd"
    let labexn = newLabelWName ("TryWith_" + exn)
    loadVar varEnv exn @
    [PUSHHDLR labexn] @
    compExpr kind (incVarIdx varEnv 3 (* Handler size = 3 *)) e1 @
    [POPHDLR; GOTO labend;Label labexn] @
    compExpr kind varEnv e2 @
    [Label labend]
    
and compValdec (kind: int->var) (varEnv: varEnv) (t:valdec<typ>) : varEnv * int * instr list =
  debug ("compValdec with varEnv = " + (ppVarEnv varEnv));
  match t with
  | Fundecs fs ->
    (* Calculate fvs, closure size and fresh code label. *)
    let numfs = List.length fs
    (* Add closures to compile environment *)
    let varEnvWClos =
      List.fold (fun (env,fdepth) (f,_,_) ->
                   ([(f,kind fdepth)] @ env, fdepth+1)) varEnv fs
    let fsfvs = List.map (fun (f,x,fBody) ->
                          (* To minimize closures, we do not copy globals in scope in closure. *)
                          let fvsAll = freevars fBody - (set [x;f])
                          let fvsGlobalInScope = filterGlobalsInScope varEnvWClos fvsAll
                          let fvsClos = Set.toList (fvsAll - fvsGlobalInScope)
                          let _ = debug ("Fundecs: "+f+", parameter: "+x+", fvs in clos: "+(ppFreevars fvsClos))
                          let labFunc = newLabelWName ("LabFunc_" + f)
                          (f,x,fBody,fvsAll,fvsGlobalInScope,fvsClos,List.length fvsClos + 1,labFunc)) fs
    (* Code to allocate closures *)
    let iaClos = List.map (fun (_,_,_,_,_,_,sizeClos,_) -> ACLOS sizeClos) fsfvs
    //let _ = printf "varEnvWClos: %s\n" (ppVarEnv varEnvWClos)      
    (* Code to copy free variables and code label to each closure *)
    let iFillClos = List.map (fun (f,x,fBody,fvsAll,fvsGlobalsInScope,fvsClos,sizeClos,funcLab) ->
                              let codefvsClos = List.concat (List.map (loadVar varEnvWClos) fvsClos)
                              let codeClosPtr = loadVar varEnvWClos f
                              PUSHLAB funcLab :: codefvsClos @ codeClosPtr @ [HEAPSTI sizeClos;INCSP -1]) fsfvs
    (* Generate code for each function body *)
    let codefBody (f,x,fBody,fvsAll,fvsGlobalInScope,fvsClos,sizeClos,funcLab) =
      (* Closure at index 0, argument at index 1, fv1 at index 1 in closure; idx 0 is code pointer. *)
      let varEnv = (f, Locvar 0) :: (x, Locvar 1) ::
                   (List.mapi (fun i x -> (x,Closvar (i+1))) fvsClos) @
                   (List.map (fun x -> (x,lookup (fst varEnvWClos) x)) (Set.toList fvsGlobalInScope))
      addFunc funcLab (compExpr Locvar (varEnv,2) fBody @ [RET 2])
    let _ = List.iter codefBody fsfvs
    let insts = iaClos @ (List.concat iFillClos)
    (varEnvWClos,numfs,insts)

  | Valdec (x,eRhs) ->
    let (env,fdepth) = varEnv
    let ieRhs = compExpr kind varEnv eRhs
    let newEnv = ((x,kind fdepth) :: env, fdepth+1)
    (newEnv,1,ieRhs)

  | Exn(ExnVar exn,_) ->
    let (env,fdepth) = varEnv
    let exnNumCode = nextExnNumCode varEnv
    let newEnv = ((exn,kind fdepth) :: env, fdepth+1)
    (newEnv,1,exnNumCode)
    (* global variable exnNum, code to generate next number, leave on stack *)

and compProg (p:program<typ>) : instr list =
  let _ = resetLabels()
  let _ = resetFuncs()
  let _ = resetGlobalInit()
  let labMain = newLabel()
  let emptyEnv = exnNumVarEnv (* Global exception number as first global variable, addr 0 *)
  let _ = addGlobalInit (newLabelWName "G_ExnVar") (initExnNumCode 0) 
  match p with
    | Prog(valdecs,e) ->
    let compValdec' (numVals,varEnv) t =
      let (env,numVals',insts) = compValdec Glovar varEnv t
      let _ = addGlobalInit (newLabelWName "G_Valdecs") insts
      (numVals+numVals',env)
    let (numVals,(env,_)) = List.fold compValdec' (1 (* 1 to also remove exn counter.*),emptyEnv) valdecs
    addFunc labMain ((compExpr Locvar (env,0) e) @ [RET 0])
    getGlobalInit() @
    [GETSP;CSTI (numVals-1);SUB] @
    [CALL(0,labMain); STI; INCSP -numVals; STOP] @
    getFuncs()
    
(* Compile a complete Micro-SML program and write the resulting instruction list
   to file fname; also, return the program as a list of instructions.
 *)

let intsToFile (inss : int list) (fname : string) = 
  File.WriteAllText(fname, String.concat " " (List.map string inss))

let compileToFile (opt_p',debug_p',verbose_p',program,fname) =
  let _ = (debug_p := debug_p'; opt_p := opt_p'; verbose_p := verbose_p') (* Set compiler flags *)
  let instrs   = compProg program 
  let bytecode = code2ints instrs
  let _ = if !verbose_p then printfn "\nCompiled to %s\n%s\n" fname (ppInsts instrs) else ()
  intsToFile bytecode fname
  instrs

