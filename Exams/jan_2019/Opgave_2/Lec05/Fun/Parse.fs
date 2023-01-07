(* Lexing and parsing of micro-ML programs using fslex and fsyacc *)

module Parse

open System
open System.IO
open System.Text
open (*Microsoft.*)FSharp.Text.Lexing
open Absyn

(* Plain parsing from a string, with poor error reporting *)

let fromString (str : string) : expr =
    let lexbuf = (*Lexing. insert if using old PowerPack *)LexBuffer<char>.FromString(str)
    try 
      FunPar.Main FunLex.Token lexbuf
    with 
      | exn -> let pos = lexbuf.EndPos 
               failwithf "%s near line %d, column %d\n" 
                  (exn.Message) (pos.Line+1) pos.Column
             
(* Parsing from a file *)

let fromFile (filename : string) =
    use reader = new StreamReader(filename)
    let lexbuf = (* Lexing. insert if using old PowerPack *) LexBuffer<char>.FromTextReader reader
    try 
      FunPar.Main FunLex.Token lexbuf
    with 
      | exn -> let pos = lexbuf.EndPos 
               failwithf "%s in file %s near line %d, column %d\n" 
                  (exn.Message) filename (pos.Line+1) pos.Column

(* Exercise it *)

let e1 = fromString "5+7";;
let e2 = fromString "let f x = x + 7 in f 2 end";;

(* Examples in concrete syntax *)

let ex1 = fromString 
            @"let f1 x = x + 1 in f1 12 end";;

(* Example: factorial *)

let ex2 = fromString 
            @"let fac x = if x=0 then 1 else x * fac(x - 1)
              in fac n end";;

(* Example: deep recursion to check for constant-space tail recursion *)

let ex3 = fromString 
            @"let deep x = if x=0 then 1 else deep(x-1) 
              in deep count end";;
    
(* Example: static scope (result 14) or dynamic scope (result 25) *)

let ex4 = fromString 
            @"let y = 11
              in let f x = x + y
                 in let y = 22 in f 3 end 
                 end
              end";;

(* Example: two function definitions: a comparison and Fibonacci *)

let ex5 = fromString
            @"let ge2 x = 1 < x
              in let fib n = if ge2(n) then fib(n-1) + fib(n-2) else 1
                 in fib 25 
                 end
              end";;


let as11 = fromString @"let let x = { } in x end " (* ex1 *)
let as12 = fromString @"let x = {field1 = 32} in x.field1 end" (* ex2 *)
let as13 = fromString @"let x = {field1 = 32; field2 = 33} in x end" (* ex3 *)
let as14 = fromString @"let x = {field1 = 32; field2 = 33} in x.field1 end" (* ex4 *)
let as15 = fromString @"let x = {field1 = 32; field2 = 33} in x.field1+x.field2 end" (* ex5 *)