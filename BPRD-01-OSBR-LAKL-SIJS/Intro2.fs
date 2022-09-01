(* Programming language concepts for software developers, 2010-08-28 *)

(* Evaluating simple expressions with variables *)

module Intro2

(* Association lists map object language variables to their values *)

let env = [("a", 3); ("c", 78); ("baf", 666); ("b", 111)];;

let emptyenv = []; (* the empty environment *)

let rec lookup env x =
    match env with 
    | []        -> failwith (x + " not found")
    | (y, v)::r -> if x=y then v else lookup r x;;

let cvalue = lookup env "c";;


(* Object language expressions with variables *)

type expr = 
  | CstI of int
  | Var of string
  | Prim of string * expr * expr
  | Max of expr * expr
  | Min of expr * expr
  | If of expr * expr * expr

let e1 = CstI 17;;

let e2 = Prim("+", CstI 3, Var "a");;

let e3 = Prim("+", Prim("*", Var "b", CstI 9), Var "a");;


(* Evaluation within an environment *)

let rec eval e (env : (string * int) list) : int =
    match e with
    | CstI i            -> i
    | Var x             -> lookup env x 
    | Prim(ope, e1, e2) -> 
        let i1 = eval e1 env
        let i2 = eval e2 env
        match ope with
        | "+" -> i1 + i2
        | "*" -> i1 * i2
        | "-" -> i1 - i2
        | "==" -> if i1 = i2 then 1 else 0
        | _ -> failwith "unknown primitive"
    | Max (e1, e2)      -> 
        let eval1 = eval e1 env
        let eval2 = eval e2 env
        if eval1 > eval2 then eval1 else eval2
    | Min (e1, e2)      -> 
        let eval1 = eval e1 env
        let eval2 = eval e2 env
        if eval1 < eval2 then eval1 else eval2
    | If (e1, e2, e3)   ->
        if eval e1 env >= 1 then eval e2 env else eval e3 env



let e1v  = eval e1 env;;
let e2v1 = eval e2 env;;
let e2v2 = eval e2 [("a", 314)];;
let e3v  = eval e3 env;;

type aexpr =
  | CstI of int
  | Var of string
  | Add of aexpr * aexpr
  | Mul of aexpr * aexpr
  | Sub of aexpr * aexpr

let ae1 = Sub(Var "v", Add(Var "w", Var "z"))
let ae2 = Mul(CstI 2, ae1)
let ae3 = Add(Var "x", Add(Var "y", Add(Var "z", Var "v")))

let ae4 = Add(CstI(1), CstI(1))

let fmt (ae: aexpr) : string =
    let rec aux ae (str: string) =
        match ae with
        | CstI i        -> str + (sprintf "%i" i)
        | Var v         -> str + v
        | Add (e1, e2)  -> str + "(" + ((aux e1 "") + " + " + (aux e2 "")) + ")"
        | Mul (e1, e2)  -> str + "(" + ((aux e1 "") + " * " + (aux e2 "")) + ")"
        | Sub (e1, e2)  -> str + "(" + ((aux e1 "") + " - " + (aux e2 "")) + ")"
    aux ae ""
