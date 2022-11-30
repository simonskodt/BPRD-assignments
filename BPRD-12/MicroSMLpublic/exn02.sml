(* Expects uncaught exception 2. *)

exception exn
	      
begin

  let
    exception exn2
    val x = 42
  in
    (* A sequence example where a following try is after the exception code of the previous. *)		      
    (try
       raise exn2
     with exn2 -> print 42);

    (try
       raise exn
     with exn -> print 4242);
    
    (try
       raise exn2
     with exn -> print 42)
  end

end
