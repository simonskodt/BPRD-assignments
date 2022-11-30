(* Example where same argument name is used for mutually recursive functions *)

fun h x = x + g (23::nil) (* x has type int *)
and g x = hd x            (* x has type int list *)
      
begin
  print h 3	     
end	    
