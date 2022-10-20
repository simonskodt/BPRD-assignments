(* File Fun/ParseAndRunHigher.fs *)

module ParseAndRunHigher

open HigherFun;;

let fromString = Parse.fromString;;

let eval = HigherFun.eval;;

let run e = eval e [];;

(* Examples of higher-order programs, in concrete syntax *)

let ex5 = 
    Parse.fromString 
     @"let tw g = let app x = g (g x) in app end 
       in let mul3 x = 3 * x 
       in let quad = tw mul3 
       in quad 7 end end end";;

let ex6 = 
    Parse.fromString 
     @"let tw g = let app x = g (g x) in app end 
       in let mul3 x = 3 * x 
       in let quad = tw mul3 
       in quad end end end";;

let ex7 = 
    Parse.fromString 
     @"let rep n = 
           let rep1 g = 
               let rep2 x = if n=0 then x else rep (n-1) g (g x) 
               in rep2 end 
           in rep1 end 
       in let mul3 x = 3 * x 
       in let tw = rep 2 
       in let quad = tw mul3 
       in quad 7 end end end end";;

let ex8 = 
    Parse.fromString 
     @"let rep n =
           let rep1 g = 
               let rep2 x = if n=0 then x else rep (n-1) g (g x) 
               in rep2 end 
           in rep1 end 
       in let mul3 x = 3 * x 
       in let twototen = rep 10 mul3 
       in twototen 7 end end end";;

let ex9 = 
    Parse.fromString 
     @"let rep n = 
           let rep1 g = 
               let rep2 x = if n=0 then x else rep (n-1) g (g x) 
               in rep2 end 
           in rep1 end 
       in let mul3 x = 3 * x 
       in let twototen = (rep 10) mul3 
       in twototen 7 end end end";;

(* Exercise 6.1 *)
let ex10 = 
    fromString
        @"let add x = let f y = x+y in f end
        in add 2 5 end"

let ex11 =
    fromString
        @"let add x = let f y = x+y in f end
        in let addtwo = add 2
            in addtwo 5 end
        end"

let ex12 =
    fromString
        @"let add x = let f y = x+y in f end
        in let addtwo = add 2
            in let x = 77 in addtwo 5 end
            end
        end"

let ex13 =
    fromString
        @"let add x = let f y = x+y in f end
        in add 2 end"

(* Exercise 6.2 *)
let ex14 = 
    fromString
        @"fun x -> 2*x"

let ex15 =
    fromString  
        @"let y = 22 in fun z -> z+y end"

(* Exercise 6.3 *)
let ex16 = 
    fromString
        @"let add x = fun y -> x+y
        in add 2 5 end"

let ex17 = 
    fromString
        @"let add = fun x -> fun y -> x+y
        in add 2 5 end"