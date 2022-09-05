(* Programming language concepts for software developers, 2010-08-28 *)

(* Evaluating simple expressions with variables *)

module Intro2

(* Association lists map object language variables to their values *)

let env = [("a", 3); ("c", 78); ("baf", 666); ("b", 111)];;

let emptyenv = [];; (* the empty environment *)

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
  | Max of expr * expr           // 1.1(i)
  | Min of expr * expr           // 1.1(i)
  | If of expr * expr * expr;;   // 1.1(iv)

let e1 = CstI 17;;

let e2 = Prim("+", CstI 3, Var "a");;

let e3 = Prim("+", Prim("*", Var "b", CstI 9), Var "a");;


(* Evaluation within an environment *)

let rec eval e (env : (string * int) list) : int =
    match e with
    | CstI i            -> i
    | Var x             -> lookup env x 
    | Prim(ope, e1, e2) ->                           // 1.1(iii)
        let i1 = eval e1 env
        let i2 = eval e2 env
        match ope with
        | "+" -> i1 + i2
        | "*" -> i1 * i2
        | "-" -> i1 - i2
        | "==" -> if i1 = i2 then 1 else 0           // 1.1(i)
        | _ -> failwith "unknown primitive"
    | Max (e1, e2)      ->                           // 1.1(i)
        let eval1 = eval e1 env
        let eval2 = eval e2 env
        if eval1 > eval2 then eval1 else eval2
    | Min (e1, e2)      ->                           // 1.1(i)
        let eval1 = eval e1 env
        let eval2 = eval e2 env
        if eval1 < eval2 then eval1 else eval2
    | If (e1, e2, e3)   ->                           // 1.1(v)
        if eval e1 env >= 1 then eval e2 env else eval e3 env



let e1v  = eval e1 env;;
let e2v1 = eval e2 env;;
let e2v2 = eval e2 [("a", 314)];;
let e3v  = eval e3 env;;

// 1.2(i)
type aexpr =                                         
  | CstI of int
  | Var of string
  | Add of aexpr * aexpr
  | Mul of aexpr * aexpr
  | Sub of aexpr * aexpr

// 1.2(ii)
let ae1 = Sub(Var "v", Add(Var "w", Var "z"))
let ae2 = Mul(CstI 2, ae1)
let ae3 = Add(Var "x", Add(Var "y", Add(Var "z", Var "v")))

let ae4 = Add(CstI(1), CstI(1))      // 1.1(ii)  

// 1.2(iii)
let fmt (ae: aexpr) : string =
    let rec aux ae (str: string) =
        match ae with
        | CstI i        -> str + (sprintf "%i" i)
        | Var v         -> str + v
        | Add (e1, e2)  -> str + "(" + ((aux e1 "") + " + " + (aux e2 "")) + ")"
        | Mul (e1, e2)  -> str + "(" + ((aux e1 "") + " * " + (aux e2 "")) + ")"
        | Sub (e1, e2)  -> str + "(" + ((aux e1 "") + " - " + (aux e2 "")) + ")"
    aux ae ""

let ae5 = CstI(5)
let ae6 = CstI(5)
let ae7 = Mul(ae5, ae6)

// 1.2(iv)
let rec simplify (ae: aexpr) : aexpr =
    match ae with
    | CstI i       -> CstI i
    | Var v        -> Var v
    | Add (e1, e2) -> 
        let simp1 = simplify e1
        let simp2 = simplify e2
        match simp1, simp2 with
        | CstI 0, e | e, CstI 0 -> e
        | _, _ -> Add(simp1, simp2)
    | Mul (e1, e2) -> 
        let simp1 = simplify e1
        let simp2 = simplify e2
        match simp1, simp2 with
        | CstI 1, e | e, CstI 1 -> e
        | CstI 0, _ | _, CstI 0 -> CstI 0
        | _, _ -> Mul(simp1, simp2)
    | Sub (e1, e2) -> 
        let simp1 = simplify e1
        let simp2 = simplify e2
        match simp1, simp2 with
        | e1, CstI 0 -> e1
        | e1, e2 when e1 = e2 -> CstI 0
        | _, _ -> Sub(simp1, simp2)

// 1.2(v) 
let rec diff (ae: aexpr) (x: string) : aexpr =
    match ae with
    | CstI _             -> CstI 0
    | Var v when v = x   -> CstI 1
    | Var y when y <> x  -> CstI 0
    | Add (e1, e2)       -> Add(diff e1 x, diff e2 x)
    | Sub (e1, e2)       -> Sub(diff e1 x, diff e2 x)
    | Mul (e1, e2)       -> Add (Mul(diff e1 x, e2), (Mul(e1, diff e2 x)))
    | _                  -> failwith "Not to be differentiated"