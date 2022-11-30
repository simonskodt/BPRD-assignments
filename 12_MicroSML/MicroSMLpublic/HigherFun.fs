(* A functional language with integers and higher-order functions 
   sestoft@itu.dk 2009-09-11
   nh@itu.dk 2015-02-12
   
   The language is called micro-SML because it has some inspiration from
   Standard ML including generative exceptions and the
   let val ... in end construct.

   Mutual recursive functions, lists and exceptions are supported.

   As with micro-ML, a function definition can have only one
   parameter, but a multiparameter (curried) function can be defined
   using nested function definitions:

     let
       fun f x = fn g y -> x + y
     in
       f 6 7
     end
*)

module HigherFun

open Absyn
open TypeInference

let createNumGen() =
  let num = ref 0
  let newNumGen() = num := !num + 1;!num
  newNumGen

let newFuncName =
  let numGen = createNumGen()
  fun () -> "annFunc" + (string)(numGen())
  
let freshExnName = createNumGen()    

(* Environment operations *)

type 'v env = (string * 'v) list

(* A runtime value is an integer or a function closure *)

type value = 
  | Int of int
  | List of value list
  | ClosureRef of closure ref
and closure =
  Closure of string * string * expr<typ> * value env       (* (f, x, fBody, fDeclEnv) *)

type answer =
  | Result of value  
  | Abort of string 

let rec ppValue = function
  | Int i -> sprintf "%d" i
  | List l -> let elems = List.map ppValue l
              "[" + (String.concat "," elems) + "]"
  | ClosureRef closRef ->
    match !closRef with
    | Closure(f,x,fBody,fDeclEnv) -> "Closure("+f+","+x+"fBody" + "," + "fDeclEnv" + ")"

let ppEnv fPP env =
  let ppEntry (s,v) acc = sprintf "  %s |-> %s \n" s (fPP v) + acc
  "Environment: \n" +
  List.foldBack ppEntry env ""

let rec lookupOpt env x =
  match env with 
  | []        -> None
  | (y, v)::r -> if x=y then Some v else lookupOpt r x

let rec evalExpr (env : value env) (e : expr<typ>)
                 (cont: value -> answer) (econt: value -> answer)  : answer =
  match e with
  | CstI (i,_) -> cont (Int i)
  | CstB (b,_) -> cont (Int (if b then 1 else 0))
  | CstN _     -> cont (List [])
  | Var (x,_)  ->
    match lookupOpt env x with
    | Some v -> cont v
    | None -> Abort ("HigherFun.lookup: " + x + " not found in\n" + (ppEnv ppValue env))
  | Prim1(ope,e1,_) ->
    evalExpr env e1 (fun v1 ->
                     match (ope,getTypExpr e1) with
                     | ("print",TypI) -> printf "%s " (ppValue v1); cont v1
                     | ("print",TypB) -> printf "%s " (if v1=Int 0 then "false" else "true"); cont v1
                     | ("print",TypL _) -> printf "%s " (ppValue v1); cont v1
                     | ("print",_) -> printf "%s " (ppValue v1);  (* Print for all types *) cont v1
                     | ("hd",_) -> match v1 with
                                   | List [] -> Abort "Prim1: Can't do hd on empty list"
                                   | List (x::_) -> cont x
                                   | _ -> Abort ("Prim1: hd on non list value: " + (ppValue v1))
                     | ("tl",_) -> match v1 with
                                   | List [] -> Abort "Prim1: Can't do tl on empty list"
                                   | List (_::xs) -> cont (List xs)
                                   | _ -> Abort ("Prim1: tl on non list value: " + (ppValue v1))
                     | ("isnil",_) -> match v1 with
                                      | List [] -> cont (Int 1)
                                      | List _ -> cont (Int 0)
                                      | _ -> Abort ("Prim1: isnil on non list value: " + (ppValue v1))
                     | _ -> Abort ("Prim1 "+ope+" not implemented")) econt
  | Prim2(ope, e1, e2,_) ->
    evalExpr env e1
      (fun v1 ->
       evalExpr env e2
         (fun v2 ->
          match (ope, v1, v2) with
          | ("*", Int i1, Int i2) -> cont (Int (i1 * i2))
          | ("+", Int i1, Int i2) -> cont (Int (i1 + i2))
          | ("-", Int i1, Int i2) -> cont (Int (i1 - i2))
          | ("%", Int i1, Int i2) -> cont (Int (i1 % i2))
          | ("=", Int i1, Int i2) -> cont (Int (if i1 = i2 then 1 else 0))
          | ("<", Int i1, Int i2) -> cont (Int (if i1 < i2 then 1 else 0))
          | (">", Int i1, Int i2) -> cont (Int (if i1 > i2 then 1 else 0))
          | ("<=", Int i1, Int i2) -> cont (Int (if i1 <= i2 then 1 else 0))
          | (">=", Int i1, Int i2) -> cont (Int (if i1 >= i2 then 1 else 0))
          | ("::",x,List xs) -> cont (List (x::xs))
          |  _ -> Abort ("unknown primitive " + ope + " or wrong type")) econt) econt
  | AndAlso(e1,e2,_) ->
    evalExpr env e1
      (fun v1 ->
       match v1 with
       | Int 0 -> cont (Int 0) (* Short circuit evaluation *)
       | Int _ -> evalExpr env e2 cont econt
       | _     -> Abort "evalExpr AndAlso") econt
  | OrElse(e1,e2,_) ->
    evalExpr env e1 
      (fun v1 ->
       match v1 with
       | Int 0 -> evalExpr env e2 cont econt
       | Int _ -> cont (Int 1) (* Short circuit evaluation *)
       | _     -> Abort "evalExpr OrElse") econt
  | Seq(e1,e2,_) ->
      evalExpr env e1 (* Disregard result of e1 and return e2 *)
        (fun _ -> evalExpr env e2 cont econt) econt
  | Let(valdecs,letBody) -> evalValdecs env valdecs letBody cont econt
  | If(e1, e2, e3) ->
      evalExpr env e1
        (fun v1 -> 
         match v1 with
         | Int 0 -> evalExpr env e3 cont econt
         | Int _ -> evalExpr env e2 cont econt
         | _     -> Abort "evalExpr If") econt
  | Fun(x,fBody,_) ->
    let freeVars = Set.toList (freevars fBody - (set [x]))
    let freeVarsEnv = List.filter (fun (f,_) -> List.exists ((=)f) freeVars) env
    cont (ClosureRef (ref (Closure(newFuncName(), x, fBody, freeVarsEnv))))
  | Call(eFun, eArg,tOpt,_) ->
    evalExpr env eFun
      (fun vFun ->
       match vFun with
       | ClosureRef closRef as fVal ->
         (match !closRef with
          | Closure(f,x,fBody,fDeclEnv) ->
            evalExpr env eArg 
              (fun xVal ->
               let fBodyEnv = (f,fVal) :: (x,xVal) :: fDeclEnv
               evalExpr fBodyEnv fBody cont econt)) econt
       | _ -> Abort "evalExpr Call: not a function") econt
  | Raise(e,aOpt) -> evalExpr env e econt econt (* Raise exception *)
  | TryWith(e1,ExnVar exn,e2) ->
    evalExpr env e1 cont
      (fun vExn1 ->
       match lookupOpt env exn with
       | None -> Abort ("HigherFun.TryWith: Can't find exception " + exn)
       | Some vExn2 -> if vExn1 = vExn2 then evalExpr env e2 cont econt else econt vExn1)
and evalValdecs (env:value env) (ts:valdec<typ> list) (body: expr<typ>)
                (cont: value -> answer) (econt: value -> answer) : answer =
  match ts with
  | [] -> evalExpr env body cont econt                    
  | Fundecs fs :: ts' -> 
    let closureRefs = List.map (fun (f,x,fBody) -> (f, ref (Closure(f,x,fBody,env)))) fs
    let updClosRef (f,closRef) =
      (match !closRef with
       | Closure(f,x,fBody,_) -> 
         let freeVars = Set.toList (freevars fBody - (set [x;f]))
         let freeVarsEnv = List.filter (fun (f,_) -> List.exists ((=)f) freeVars) env
         let freeClosureRefs = List.filter (fun (f,_) -> List.exists ((=)f) freeVars) closureRefs
         let fBodyEnv = (List.map (fun (f,closRef) -> (f, ClosureRef closRef)) freeClosureRefs) @ freeVarsEnv 
         (closRef := Closure(f,x,fBody,fBodyEnv)))
    let _ = List.iter updClosRef closureRefs
    let env' = (List.map (fun (f, closRef) -> (f, ClosureRef closRef)) closureRefs) @ env
    evalValdecs env' ts' body cont econt
  | Valdec (x,eRhs) :: ts' ->
    evalExpr env eRhs (fun xVal -> evalValdecs ((x, xVal) :: env) ts' body cont econt) econt
  | Exn(ExnVar x,_)::ts' -> evalValdecs ((x,Int (freshExnName())) :: env) ts' body cont econt
and evalProg (env:value env) (t:program<typ>) : answer =
  match t with
    | Prog(valdecs,e) -> evalValdecs env valdecs e
                                     (fun v -> Result v)
                                     (fun v -> Abort ("Uncaught exception: " + (ppValue v)))

(* Evaluate in empty environment: program must have no free variables: *)
let check e =
  let rec check' (e : expr<'a>) (env : string list) : bool =
    match e with
    | CstI (i,_) -> true
    | CstB (b,_) -> true
    | CstN _     -> true
    | Var (x,_)  -> List.exists ((=)x) (*(fun b -> b=x)*) env
    | Prim1(ope,e1,_) -> check' e1 env
    | Prim2(ope,e1,e2,_) -> check' e1 env && check' e2 env
    | AndAlso(e1,e2,_) -> check' e1 env && check' e2 env
    | OrElse(e1,e2,_) -> check' e1 env && check' e2 env
    | Seq(e1,e2,_) -> check' e1 env && check' e2 env
    | Let(valdecs, letbody) -> fst (List.fold checkValdec' (true,env) valdecs)
    | If(e1, e2, e3) -> check' e1 env &&
                        check' e2 env &&
                        check' e3 env 
    | Fun(x,fBody,_) -> check' fBody (x::env)
    | Call(eFun,eArg,_,_) -> check' eFun env && check' eArg env
    | Raise(e,_) -> check' e env 
    | TryWith(e1,ExnVar exn,e2) -> check' e1 env && check' e2 env && List.exists ((=)exn) env
  and checkValdec' (b : bool, env : string list) (valdec : valdec<'a>) : bool * string list =
    match valdec with
    |  Fundecs fs -> 
      let fEnv = List.foldBack (fun (f,_,_) env -> f::env) fs env (* fBody may recursively call f *)
      (List.foldBack (fun (_,x,fBody) acc -> check' fBody (x::fEnv) && acc) fs b,  (* Only include x for function f *)
       fEnv)
    | Valdec(x,eRhs) -> (check' eRhs env,x::env)
    | Exn(ExnVar x,_) -> (b, x::env)
  check' e []  
