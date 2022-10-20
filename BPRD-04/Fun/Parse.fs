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

(* Exercise 4.2: Examples*)

(* Example 1: Sum of numbers*)
let ex6 = fromString
            @"let sumOfNumbers x = if x=1 then 1 else x + (sumOfNumbers (x-1))
            in sumOfNumbers 1000 end";;

(* Example 2: number 3 to the power of 8*)
let ex7 = fromString
            @"let threePowerEight n = if n = 0 then 1 else  3* (threePowerEight (n-1))
             in threePowerEight 8 end"

(* Example 3: 3 to the power of n until 11*)
let ex8 = fromString            
            @"let powerOfX x = if x = 0 then 1 else 3 * (powerOfX (x-1))
                in let threeToEleven x = if x = 0 then 1 else powerOfX x + (threeToEleven (x-1))
                  in threeToEleven 11 end
             end"

(* Example 4: compute n..10, power is 8*)
let ex9 = fromString
            @"let powerOf8 x = x*x*x*x*x*x*x*x
                in let oneToTenPow8 x = if x = 1 then 1 else powerOf8 x + (oneToTenPow8 (x-1))
                  in oneToTenPow8 10 end
             end"

(* Exercise 4.4: Examples*)

let ex4_41 = fromString
                @"let pow x n = if n=0 then 1 else x * pow x (n-1) in pow 3 8 end"

let ex4_42 = fromString
              @"let max2 a b = if a<b then b else a 
                  in let max3 a b c = max2 a (max2 b c) 
                    in max3 25 6 62 end 
                end"

(* Exercise 4.5: Examples*)
let ex4_51 = fromString 
               @"let x = 3 && 4 in 5 end"
let ex4_52 = fromString 
               @"let x = 3 || 4 in 5 end"