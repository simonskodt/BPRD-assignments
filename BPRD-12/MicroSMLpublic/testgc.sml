fun test t = fn r -> print (t = r)
			   
(* Garbage Collection *)
fun tgc01 n = (* Generate data that is dead immediately - test gc. *)
  let fun genFn n = if n > 0 then genFn (let val y = fn x -> x + 1 in n-1 end) else 42
  in genFn n
  end

fun tgc02 n = (* Require tail recursion for the closures to be garbage collecable *)
  let val f = fn x -> x-1
  in tgc02' (f n)
  end
and tgc02' n =
  let
    val continue = fn y -> if y <= 0 then false else true
  in
    if continue n then tgc02 n else 42
  end

begin
  test (tgc01 100000) 42;
  test (tgc02 100000) 42
end
      
