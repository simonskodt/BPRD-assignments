(* A functional language with integers and higher-order functions 
   sestoft@itu.dk 2009-09-11

   The language is higher-order because the value of an expression may
   be a function (and therefore a function can be passed as argument
   to another function).

   A function definition can have only one parameter, but a
   multiparameter (curried) function can be defined using nested
   function definitions:

      let f x = let g y = x + y in g end in f 6 7 end
 *)

module HigherFun

open Absyn

(* Environment operations *)

type 'v env = (string * 'v) list

let rec lookup env x =
    match env with 
    | []        -> failwith (x + " not found")
    | (y, v)::r -> if x=y then v else lookup r x;;

(* A runtime value is an integer or a function closure *)

(* Exercise 6.2 *)
type value = 
  | Int of int
  | Closure of string * string * expr * value env       (* (f, x, fBody, fDeclEnv) *)
  | Clos of string * expr * value env                   (* x, body, declEnv*)

(* Exercise 6.2 *)
let rec eval (e : expr) (env : value env) : value =
    match e with
    | CstI i -> Int i
    | CstB b -> Int (if b then 1 else 0)
    | Var x  -> lookup env x
    | Prim(ope, e1, e2) -> 
      let v1 = eval e1 env
      let v2 = eval e2 env
      match (ope, v1, v2) with
      | ("*", Int i1, Int i2) -> Int (i1 * i2)
      | ("+", Int i1, Int i2) -> Int (i1 + i2)
      | ("-", Int i1, Int i2) -> Int (i1 - i2)
      | ("=", Int i1, Int i2) -> Int (if i1 = i2 then 1 else 0)
      | ("<", Int i1, Int i2) -> Int (if i1 < i2 then 1 else 0)
      |  _ -> failwith "unknown primitive or wrong type"
    | Let(x, eRhs, letBody) -> 
      let xVal = eval eRhs env
      let letEnv = (x, xVal) :: env 
      eval letBody letEnv
    | If(e1, e2, e3) -> 
      match eval e1 env with
      | Int 0 -> eval e3 env
      | Int _ -> eval e2 env
      | _     -> failwith "eval If"
    | Letfun(f, x, fBody, letBody) -> 
      let bodyEnv = (f, Closure(f, x, fBody, env)) :: env
      eval letBody bodyEnv
    | Call(eFun, eArg) -> 
      let fClosure = eval eFun env
      match fClosure with
      | Closure (f, x, fBody, fDeclEnv) ->
        let xVal = eval eArg env
        let fBodyEnv = (x, xVal) :: (f, fClosure) :: fDeclEnv
        in eval fBody fBodyEnv
      | Clos (x, body, declEnv) -> // Exercise 6.2
        let xVal = eval eArg env
        let fBodyEnv = (x, xVal) :: declEnv
        eval body fBodyEnv
      | _ -> failwith "eval Call: not a function"
    | Fun (x, e) -> // Exercise 6.2
      Clos(x, e, env) 

(* Evaluate in empty environment: program must have no free variables: *)

let run e = eval e [];;

(* Examples in abstract syntax *)

let ex1 = Letfun("f1", "x", Prim("+", Var "x", CstI 1), 
                 Call(Var "f1", CstI 12));

(* Factorial *)

let ex2 = Letfun("fac", "x", 
                 If(Prim("=", Var "x", CstI 0),
                    CstI 1,
                    Prim("*", Var "x", 
                              Call(Var "fac", 
                                   Prim("-", Var "x", CstI 1)))),
                 Call(Var "fac", Var "n"));

(* let fac10 = eval ex2 [("n", Int 10)];; *)

let ex3 = 
    Letfun("tw", "g", 
           Letfun("app", "x", Call(Var "g", Call(Var "g", Var "x")), 
                  Var "app"),
           Letfun("mul3", "y", Prim("*", CstI 3, Var "y"), 
                  Call(Call(Var "tw", Var "mul3"), CstI 11)));;

let ex4 = 
    Letfun("tw", "g",
           Letfun("app", "x", Call(Var "g", Call(Var "g", Var "x")), 
                  Var "app"),
           Letfun("mul3", "y", Prim("*", CstI 3, Var "y"), 
                  Call(Var "tw", Var "mul3")));;

(* Exercise 6.2 *)
let ex5 = Fun("x", Prim("*", CstI 2, Var "x"));;
let ex6 = Let("y", CstI 22, Fun("z", Prim("+", Var "z", Var "y")));;