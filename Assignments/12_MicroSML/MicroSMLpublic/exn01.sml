fun test t = fn r -> print (t = r)

exception exn
exception exn2

fun f x =
  try
    let
      fun g x = raise exn
    in	      
      g
    end    
  with exn -> fn x -> x

fun g x =
  try
    let
      val exn' = exn
    in 		     
      raise exn'
    end
  with exn -> x
		    
fun h x =
  (try
     print 1;
     raise exn;
     print 2
   with exn -> print 3);
  print 4

begin
  test (try (f 0) 42 with exn -> 42) 42;
  test (try raise exn + raise exn with exn -> 42) 42;
  test (try raise exn with exn -> 42) 42;
  test (try 1 + raise exn with exn -> 42) 42;
  (* Below aborts because outer exn is not the same as local exn. *)  
  (*test (try (fn x -> let exception exn in 1 + raise exn end) 34 with exn -> 43) 43; *)
  test (g 42) 42;
  test (h 3) 4;

  
  test (let val x = 42 in
            try
              print x; raise exn; print 53
            with exn -> print x
        end) 42		  
end	
