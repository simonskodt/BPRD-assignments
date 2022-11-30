(* File TypedFun/TypedFun.fs
   An explicitly typed strict first-order functional language. 
   sestoft@itu.dk 2009-09-11

   Different abstract syntax from the first-order and higher-order 
   functional language in Fun/Fun.fs and Fun/HigherFun.fs because
   of the explicit types on function parameters and function results.

   Does not admit mutually recursive function bindings.

   Every function takes exactly one argument.

   Type checking.  Explicit types on the argument and result of each
   declared function.  Expressions and variables may have type int or
   bool or a functional type.  Functions are monomorphically and
   explicitly typed.  

   There is no lexer or parser specification for this explicitly typed
   language because next week we shall infer types rather than check
   them.  
*)

module TypedFun

(* Environment operations *)

type 'v env = (string * 'v) list

let rec lookup env x =
    match env with 
    | []        -> failwith (x + " not found")
    | (y, v)::r -> if x=y then v else lookup r x;;

(* A type is int, bool or function *)

type typ =
  | TypI                                (* int                         *)
  | TypB                                (* bool                        *)
  | TypF of typ * typ                   (* (argumenttype, resulttype)  *)

(* New abstract syntax with explicit types, instead of Absyn.expr: *)

type tyexpr = 
  | CstI of int
  | CstB of bool
  | Var of string
  | Let of string * tyexpr * tyexpr
  | Prim of string * tyexpr * tyexpr
  | If of tyexpr * tyexpr * tyexpr
  | Letfun of string * string * typ * tyexpr * typ * tyexpr
          (* (f,       x,       xTyp, fBody,  rTyp, letBody *)
  | Call of tyexpr * tyexpr

(* A runtime value is an integer or a function closure *)

type value = 
  | Int of int
  | Closure of string * string * tyexpr * value env       (* (f, x, fBody, fDeclEnv) *)

let rec eval (e : tyexpr) (env : value env) : int =
    match e with
    | CstI i -> i
    | CstB b -> if b then 1 else 0
    | Var x  ->
      match lookup env x with
      | Int i -> i 
      | _     -> failwith "eval Var"
    | Prim(ope, e1, e2) -> 
      let i1 = eval e1 env
      let i2 = eval e2 env
      match ope with
      | "*" -> i1 * i2
      | "+" -> i1 + i2
      | "-" -> i1 - i2
      | "=" -> if i1 = i2 then 1 else 0
      | "<" -> if i1 < i2 then 1 else 0
      | _   -> failwith "unknown primitive"
    | Let(x, eRhs, letBody) -> 
      let xVal = Int(eval eRhs env)
      let bodyEnv = (x, xVal) :: env 
      eval letBody bodyEnv
    | If(e1, e2, e3) -> 
      let b = eval e1 env
      if b<>0 then eval e2 env else eval e3 env
    | Letfun(f, x, _, fBody, _, letBody) -> 
      let bodyEnv = (f, Closure(f, x, fBody, env)) :: env 
      eval letBody bodyEnv
    | Call(Var f, eArg) -> 
      let fClosure = lookup env f
      match fClosure with
      | Closure (f, x, fBody, fDeclEnv) ->
        let xVal = Int(eval eArg env)
        let fBodyEnv = (x, xVal) :: (f, fClosure) :: fDeclEnv
        eval fBody fBodyEnv
      | _ -> failwith "eval Call: not a function"
    | Call _ -> failwith "illegal function in Call"

(* Type checking for the first-order functional language: *)

let rec typ (e : tyexpr) (env : typ env) : typ =
    match e with
    | CstI i -> TypI
    | CstB b -> TypB
    | Var x  -> lookup env x 
    | Prim(ope, e1, e2) -> 
      let t1 = typ e1 env
      let t2 = typ e2 env
      match (ope, t1, t2) with
      | ("*", TypI, TypI) -> TypI
      | ("+", TypI, TypI) -> TypI
      | ("-", TypI, TypI) -> TypI
      | ("=", TypI, TypI) -> TypB
      | ("<", TypI, TypI) -> TypB
      | ("&", TypB, TypB) -> TypB
      | _   -> failwith "unknown op, or type error"
    | Let(x, eRhs, letBody) -> 
      let xTyp = typ eRhs env
      let letBodyEnv = (x, xTyp) :: env 
      typ letBody letBodyEnv
    | If(e1, e2, e3) -> 
      match typ e1 env with
      | TypB -> let t2 = typ e2 env
                let t3 = typ e3 env
                if t2 = t3 then t2
                else failwith "If: branch types differ"
      | _    -> failwith "If: condition not boolean"
    | Letfun(f, x, xTyp, fBody, rTyp, letBody) -> 
      let fTyp = TypF(xTyp, rTyp) 
      let fBodyEnv = (x, xTyp) :: (f, fTyp) :: env
      let letBodyEnv = (f, fTyp) :: env
      if typ fBody fBodyEnv = rTyp
      then typ letBody letBodyEnv
      else failwith ("Letfun: return type in " + f)
    | Call(Var f, eArg) -> 
      match lookup env f with
      | TypF(xTyp, rTyp) ->
        if typ eArg env = xTyp then rTyp
        else failwith "Call: wrong argument type"
      | _ -> failwith "Call: unknown function"
    | Call(_, eArg) -> failwith "Call: illegal function in call"

let typeCheck e = typ e [];;


(* Examples of successful type checking *)

let ex1 = Letfun("f1", "x", TypI, Prim("+", Var "x", CstI 1), TypI,
                 Call(Var "f1", CstI 12));;

(* Factorial *)

let ex2 = Letfun("fac", "x", TypI,
                 If(Prim("=", Var "x", CstI 0),
                    CstI 1,
                    Prim("*", Var "x", 
                              Call(Var "fac", 
                                   Prim("-", Var "x", CstI 1)))),
                 TypI,
                 Let("n", CstI 7, Call(Var "fac", Var "n")));;

let fac10 = eval ex2 [];;

let ex3 = Let("b", Prim("=", CstI 1, CstI 2),
              If(Var "b", CstI 3, CstI 4));;

let ex4 = Let("b", Prim("=", CstI 1, CstI 2),
              If(Var "b", Var "b", CstB false));;

let ex5 = If(Prim("=", CstI 11, CstI 12), CstI 111, CstI 666);;

let ex6 = Letfun("inf", "x", TypI, Call(Var "inf", Var "x"), TypI,
                 Call(Var "inf", CstI 0));;

let types = List.map typeCheck [ex1; ex2; ex3; ex4; ex5; ex6];;

(* Examples of type errors; should throw exception when run: *)

let exErr1 = Let("b", Prim("=", CstI 1, CstI 2),
                 If(Var "b", Var "b", CstI 6));;

let exErr2 = Letfun("f", "x", TypB, If(Var "x", CstI 11, CstI 22), TypI,
                    Call(Var "f", CstI 0));;

let exErr3 = Letfun("f", "x", TypB, Call(Var "f", CstI 22), TypI,
                    Call(Var "f", CstB true));;

let exErr4 = Letfun("f", "x", TypB, If(Var "x", CstI 11, CstI 22), TypB,
                    Call(Var "f", CstB true));;
