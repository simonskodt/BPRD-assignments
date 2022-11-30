(* Example shows the limitation to Absyn.freevars where
   global variables are identified as free variables to local let constructs. 
   Global variables are removed when compiling code for closures, see Contcomp.fs. *)

val x = 1
val y = 2
val z = 3
	    
fun f y = (* Free in clos: [] *)
  let val z = 4
  in
    print (x + y + z)
  end	    

fun h x = let val k = 1 in if x = 0 then print y else g (x-1) + k + x + z end  (* Free in clos: [] *)
and g x = let val l = 1 in if x = 0 then print y else h (x-1) + x + z + l end  (* Free in clos: [] *)
      
begin
  let
    val y = f 5 + h 1
    fun f z = x + z + y (* Free in clos: [y] *)
    val g = fn z -> x + z + y + (f 6) (* Free in clos: [f, y] *)
  in
    print f 7 + g 8
  end	    
end	    
