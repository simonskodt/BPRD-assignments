(* Example showing the calculation of free variables 
   Try with -debug option *)
begin
  let
    val x = 1       (* fvs=[],    bvs=[x]     *)
    val y = x + 1   (* fvs=[x],   bvs=[x,y]   *)
    val z = x + y   (* fvs=[x,y], bvs=[x,y,z] *)
    fun f x = x + y (* freevars=[x,y]-[x]=[y] *)
  in
    x+y
  end         
end (* freevars=[x,y]+[x,y]-[x,y,z]=[] *)
