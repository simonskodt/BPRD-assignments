(* Generative exceptions example *)

fun test t = fn r -> print (t = r)
fun genExn x = let exception E in E end
fun genRaise E = fn x -> raise E
fun genTryWith E = fn g -> try g 1 with E -> 1

begin
  let
    val E1 = genExn 1
    val r1 = genRaise E1
    val h1 = genTryWith E1
    val E2 = genExn 2	
    val r2 = genRaise E2
    val h2 = genTryWith E2
  in
    test (h1 r1) 1;
    test (h2 r2) 1;
    h1 r2 (* Will result in uncaught exception due to generative nature of exceptions in Micro-SML *)
  end
end
