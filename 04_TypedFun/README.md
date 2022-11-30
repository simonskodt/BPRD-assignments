# BPRD-04

All the non-code answers are placed in this README.

## Exercise 4.1

We generated and compiled the lexer and parser to run some examples.

## Exercise 4.2

Can be found in `Parse.fs`.

```fsharp
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
```

## Exercise 4.3

Answered in `Fun.fs` & `Absyn.fs`. 

## Exercise 4.4

Answered in `FunPar.fsy`.

## Exercise 4.5

Answered in `FunLex.fsl` and `FunPar.fsy`. 