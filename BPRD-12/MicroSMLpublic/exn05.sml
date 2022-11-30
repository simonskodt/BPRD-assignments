(* Generative exceptions example *)

begin
  let val x = 43
  in    
    raise (if x > 0 then 1 else 2)
  end	    
end
