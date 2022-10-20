(* File Fun/Fun.fs
   A strict functional language with integers and first-order 
   one-argument functions * sestoft@itu.dk

   Does not support mutually recursive function bindings.

   Performs tail recursion in constant space (because F# does).
*)

module Fun

open Absyn

(* Environment operations *)

type 'v env = (string * 'v) list

let rec lookup env x =
    match env with 
    | []        -> failwith (x + " not found")
    | (y, v)::r -> if x=y then v else lookup r x;;

(* A runtime value is an integer or a function closure *)

type value = 
  | Int of int
  | Closure of string * string list * expr * value env       (* (f, x, fBody, fDeclEnv) *)

let rec eval (e : expr) (env : value env) : int =
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
      | _   -> failwith ("unknown primitive " + ope)
    | Let(x, eRhs, letBody) -> 
      let xVal = Int(eval eRhs env)
      let bodyEnv = (x, xVal) :: env
      eval letBody bodyEnv
    | If(e1, e2, e3) -> 
      let b = eval e1 env
      if b<>0 then eval e2 env
      else eval e3 env
    (* Exercise 4.3 *)
    | Letfun(f, xs, fBody, letBody) -> 
      let bodyEnv = (f, Closure(f, xs, fBody, env)) :: env 
      eval letBody bodyEnv
    | Call(Var f, args) -> 
      let fClosure = lookup env f
      match fClosure with
      | Closure (f, xs, fBody, fDeclEnv) ->
        let rec aux xs acc = 
          match xs with
          | x :: xs -> 
                  let xVal = Int(eval x env)
                  let acc' = acc :: (x, xVal)
                  aux xs acc' 
          | [] -> 
                  let fBodyEnv = acc @ (f, fClosure) :: fDeclEnv
                  eval fBody fBodyEnv
        aux xs []
      | _ -> failwith "eval Call: not a function"
    (* END Exercise 4.3 *)
    | Call _ -> failwith "eval Call: not first-order function"

(* Evaluate in empty environment: program must have no free variables: *)

let run e = eval e [];;

(* Examples in abstract syntax *)

let ex1 = Letfun("f1", "x", Prim("+", Var "x", CstI 1), 
                 Call(Var "f1", CstI 12));;

(* Example: factorial *)

let ex2 = Letfun("fac", "x",
                 If(Prim("=", Var "x", CstI 0),
                    CstI 1,
                    Prim("*", Var "x", 
                              Call(Var "fac", 
                                   Prim("-", Var "x", CstI 1)))),
                 Call(Var "fac", Var "n"));;

(* let fac10 = eval ex2 [("n", Int 10)];; *)

(* Example: deep recursion to check for constant-space tail recursion *)

let ex3 = Letfun("deep", "x", 
                 If(Prim("=", Var "x", CstI 0),
                    CstI 1,
                    Call(Var "deep", Prim("-", Var "x", CstI 1))),
                 Call(Var "deep", Var "count"));;
    
let rundeep n = eval ex3 [("count", Int n)];;

(* Example: static scope (result 14) or dynamic scope (result 25) *)

let ex4 =
    Let("y", CstI 11,
        Letfun("f", "x", Prim("+", Var "x", Var "y"),
               Let("y", CstI 22, Call(Var "f", CstI 3))));;

(* Example: two function definitions: a comparison and Fibonacci *)

let ex5 = 
    Letfun("ge2", "x", Prim("<", CstI 1, Var "x"),
           Letfun("fib", "n",
                  If(Call(Var "ge2", Var "n"),
                     Prim("+",
                          Call(Var "fib", Prim("-", Var "n", CstI 1)),
                          Call(Var "fib", Prim("-", Var "n", CstI 2))),
                     CstI 1), Call(Var "fib", CstI 25)));;

let ex6 =
  Letfun
    ("sumOfNumbers", "x",
     If
       (Prim ("=", Var "x", CstI 1), CstI 1,
        Prim
          ("+", Var "x",
           Call (Var "sumOfNumbers", Prim ("-", Var "x", CstI 1)))),
     Call (Var "sumOfNumbers", CstI 1000));;
                     
let ex7 =
  Letfun
    ("threePowerEight", "n",
     If
       (Prim ("=", Var "n", CstI 0), CstI 1,
        Prim
          ("*", CstI 3,
           Call (Var "threePowerEight", Prim ("-", Var "n", CstI 1)))),
     Call (Var "threePowerEight", CstI 8));;

let ex8Helper =
  Letfun
    ("powerOfX", "x",
     If
       (Prim ("=", Var "x", CstI 0), CstI 1,
        Prim ("*", CstI 3, Call (Var "powerOfX", Prim ("-", Var "x", CstI 1)))),
     Call (Var "powerOfX", Var "x"));;

let ex8 =
  Letfun
    ("powerOfX", "x",
     If
       (Prim ("=", Var "x", CstI 0), CstI 1,
        Prim ("*", CstI 3, Call (Var "powerOfX", Prim ("-", Var "x", CstI 1)))),
     Letfun
       ("threeToEleven", "x",
        If
          (Prim ("=", Var "x", CstI 0), CstI 1,
           Prim
             ("+", Call (Var "powerOfX", Var "x"),
              Call (Var "threeToEleven", Prim ("-", Var "x", CstI 1)))),
        Call (Var "threeToEleven", CstI 11)));;

let ex9 =
  Letfun
    ("powerOf8", "x",
     Prim
       ("*",
        Prim
          ("*",
           Prim
             ("*",
              Prim
                ("*",
                 Prim
                   ("*", Prim ("*", Prim ("*", Var "x", Var "x"), Var "x"),
                    Var "x"), Var "x"), Var "x"), Var "x"), Var "x"),
     Letfun
       ("oneToTenPow8", "x",
        If
          (Prim ("=", Var "x", CstI 0), CstI 1,
           Prim
             ("+", Call (Var "powerOf8", Var "x"),
              Call (Var "oneToTenPow8", Prim ("-", Var "x", CstI 1)))),
        Call (Var "oneToTenPow8", CstI 10)));;