(* File Fun/ParseAndType.fs *)

module ParseAndType

let fromString = Parse.fromString;;

let inferType = TypeInference.inferType;;
    
(* Well-typed examples ---------------------------------------- *)

(* In the let-body, f is polymorphic *)

let tex1 = 
    inferType(fromString "let f x = 1 in f 7 + f false end");;

(* In the let-body, g is polymorphic because f is *)

let tex2 = 
    inferType(fromString "let g = let f x = 1 in f end in g 7 + g false end");;

(* f is not polymorphic but used consistently *)

let tex3 = 
    inferType(fromString "let g y = let f x = (x=y) in f 1 = f 3 end in g 7 end");;

(* The twice function *)

let tex4 = 
    inferType(fromString 
                @"let tw g = let app x = g (g x) in app end 
                 in let triple y = 3 * y in (tw triple) 11 end 
                 end");;

let tex5 = 
    inferType(fromString 
               @"let tw g = let app x = g (g x) in app end 
                 in tw end");;

(* Declaring a polymorphic function and rebinding it *)

let tex6 =
    inferType(fromString                 
               @"let id x = x 
                 in let i1 = id 
                 in let i2 = id 
                 in let k x = let k2 y = x in k2 end 
                 in (k 2) (i1 false) = (k 4) (i1 i2) end end end end ");;

(* A large type *)

let tex7 =
    inferType(fromString 
               @"let pair x = let p1 y = let p2 p = p x y in p2 end in p1 end 
                 in let id x = x 
                 in let p1 = pair id id 
                 in let p2 = pair p1 p1 
                 in let p3 = pair p2 p2 
                 in let p4 = pair p3 p3 
                 in p4 end end end end end end");;

(* F# will not infer the same type as above (the variables in p1, p2, p3, p4
   will not be generalized) because of the so-called value restriction 
   on polymorphism, which ensures type system soundness in the presence of 
   destructive update.  An F# example similar to the above can be obtained by 
   eta-expansion: turning  let f = e  into  let f p = e p  which is valid
   when f is a function.  The type of p5 below involves 64 different type variables:

   let id x = x
   let pair x y p = p x y
   let p1 p = pair id id p
   let p2 p = pair p1 p1 p
   let p3 p = pair p2 p2 p
   let p4 p = pair p3 p3 p
   let p5 p = pair p4 p4 p
   in p5;;
*)

(* A polymorphic function may be applied to itself *)

let tex8 = 
    inferType(fromString "let f x = x in f f end");;


(* Ill-typed examples ----------------------------------------- *)

(* A function f is not polymorphic in its own right-hand side,  *)

let teex1 () = 
    inferType(fromString "let f x = f 7 + f false in 4 end");;

(* f is not polymorphic in x because y is bound further out *)

let teex2 () = 
    inferType(fromString "let g y = let f x = (x=y) in f 1 = f false end in g end");;

(* circularity: function parameter h cannot be applied to itself *)

let teex3 () = 
    (fromString "let g h = h h in let f x = x in g f end end");;

(* Exercise 6.5, part 1 *)
let ex6_5_1 = 
    (fromString "let f x = 1 
                 in f f end");;

let ex6_5_2 = 
    (fromString "let f g = g g 
                 in f end");;

let ex6_5_3 = 
    (fromString "let f x =
                    let g y = y
                    in g false end
                 in f 42 end");;
let ex6_5_4 = 
    (fromString "let f x =
                    let g y = if true then y else x
                    in g false end
                 in f 42 end");;

let ex6_5_5 = 
    (fromString "let f x =
                    let g y = if true then y else x
                    in g false end
                 in f true end");;

(* Exercise 6.5, part 2 *)
let ex6_5_6 =
    (fromString "let f x = if x = true then true else false in f end");;

let ex6_5_7 =
    (fromString "let f x = if x = 1 then 2 else 3 in f end");;

let ex6_5_8 =
    (fromString "let f x = 
                    let g y = x + y
                    in g end
                 in f end");;

let ex6_5_9 =
    (fromString "let f x = 
                    let g y = x
                    in g end
                 in f end");;                  
                    
let ex6_5_10 =
    (fromString "let f x = 
                    let g y = y
                    in g end
                 in f end");;
 
let ex6_5_11 =
    (fromString "let f x =
                    let g y = 
                        let h z = y (x z) in h end
                    in g end
                in f end");;

let ex6_5_12 =
    (fromString "let f x = let g = f x in g end
                 in f end");;

let ex6_5_13 =
    (fromString "let f x = f 1 in f 4 end");;