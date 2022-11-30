(* Example showing the calculation of free variables 
   Try with -debug option *)

(* This example fails because the function freevarsValdec in file
   Absyn.fs is wrong.  You can only add x to bvs if x has not
   previously been marked as free. 

   Solved by exercise is to do alpha conversion first.
*)

begin
  let
    val y = 1        
    fun f x = (* freevars=[y] *)
      let
        val z = y + 1
        val y = 1
      in
        z+y+x
      end                  
  in
    f y
  end         
end 
