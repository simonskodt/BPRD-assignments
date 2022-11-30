fun test t = fn r -> print (t = r)

(* Integer Expressions *)
val tint01 = 43
val tint02 = 5 - 3
val tint03 = 3 - 5

(* Boolean Expressions *)
val tbool01 = true
val tbool02 = false
val tbool03 = true = true
val tbool04 = true = false
val tbool05 = true && true
val tbool06 = true && false
val tbool07 = true || true
val tbool08 = false || true
val tbool09 = true || false
val tbool10 = let val x1 = 2
	          val y1 = 3
	          val x2 = 3
	          val y2 = 3
		  fun abs x = if x < 0 then -x else x
	      in
                x1 = y1 || x2 = y2 || abs (x1-x2) = abs (y1-y2)
	      end		      

(* Let Expressions *)
val tlet01 = let val x = 43 in x end
val tlet02 = let val x = 43+46 in x end
val tlet03 = let in 42 end
val tlet04 = let in 1 end + let in 2 end + let in 3 end
                 
(* Functions *)
val tfun01 = let fun f x = x + 7 in f 2 end
val tfun02 = let fun f1 x = x + 1 in f1 12 end
val tfun03 =
  let
    val v = 44
    fun f x = v + 1
    fun g x = v + 3
  in
    (f 3) + (g  4)
  end
val tfun04 =
  let
    fun add x = fn y -> x+y
  in
    add 2 5
  end
val tfun05 =
  let
    fun add x = let fun f y = x+y in f end
    val addtwo = add 2
  in
    addtwo 5
  end
val tfun06 =
  let
    fun add x = let fun f y = x+y in f end
    val addtwo = add 2
  in
    let val x = 77 in addtwo x end
  end
val tfun07 =
  let fun add x = let fun f y = x+y in f end
  in add 2
  end
val tfun08 = fn x -> 2*x
val tfun09 = let val y = 22 in fn z -> z+y end
val tfun10 = let fun f x = 1 in f f end

(* tfun11: g can't be polymorphic in body of f - curcularity - hense will not type. *)
(*val tfun11 = let fun f g = g g in f end*)

(* g false has type bool. x in f x is never used, hence, final type will be bool. *)
val tfun12 = let fun f x = let fun g y = y in g false end in f 42 end

(* Will not type because x has type int and y has type false and hence the if-expression can't type. *)
(*val tfun13 = let fun f x = let fun g y = if true then y else x in g false end in f 42 end*)

(* This will type, because now x and y has type bool and if expression can type. *)
val tfun14 = let fun f x = let fun g y = if true then y else x in g false end in f true end

val tfun15 = fn x -> x = true
val tfun16 = fn x -> x+1
val tfun17 = fn x -> fn y -> x + y
val tfun18 = fn x -> fn y -> x
val tfun19 = fn x -> fn y -> y
val tfun20 = let fun compose f = fn g -> fn x -> g (f x) in compose end
val tfun21 =
  (* 'a -> 'b non terminating function matches arbritary type. *) 
  let fun infinity x = infinity x in infinity end

(* Will run infinity *)
(*val tfun22 = let fun infinity x = infinity x in infinity 42 end*)

val tfun23 = 
  let fun f x = x + g 4
      and g x = x
  in f 1 
  end

val tfun24 = 
  let fun f x = if 0 < x then f (x-1) + g x else x
      and g x = f (x-1)
  in f 3
  end

val tfun25 =
  let fun f x = g (x-2)
      and g x = if x > 0 then f (x+1) else 32
  in
    f 32
  end

val tfun26 =
  let fun f x = g x
      and g x = x
  in f 3
  end

(* Example: factorial *)
val tfun27 = let fun fac x = if x=0 then 1 else x * fac(x - 1) in fac 5 end

(* Example: deep recursion to check for constant-space tail recursion - or lack off. *)
val tfun28 = let fun deep x = if x=0 then 1 else deep(x-1) in deep 1000 end

(* Example: static scope (result 14) or dynamic scope (result 25) *)
val tfun29 =
  let val y = 11
  in let fun f x = x + y
     in let val y = 22 in f 3 end
     end
   end

(* Example: two function definitions: a comparison and Fibonacci *)
val tfun30 =
  let fun ge2 x = 1 < x
  in let fun fib n = if ge2 n then fib(n-1) + fib(n-2) else 1
     in fib 10 end
  end

val tfun31 =
  let fun ge2 x = 1 < x
      and fib n = if ge2 n then fib(n-1) + fib(n-2) else 1
  in fib 10 end

val tfun32 = (* Mixing global and closure variables *)
  let val x = 42
      val y = 41
  in (fn z -> z+x+y) 1 end

val tfun33 = (* Mixing global, closure and local variables *)
  let val x = 42
  in let val y = 41
     in ((fn z -> let val l = 40 in let val m = 39 in m+l+z+x+y end end) 1)
     end
  end

fun tfun34 i = fn j -> if i <= 0 then 0 else tfun34' (i-1) + tfun34 (i-1) j
and tfun34' i = tfun34 (i-1) i	   

fun tfun35 i = fn j -> fn k -> fn l ->
  let
    fun add m = i + j + k + l + m
  in
    add 1
  end

(* Sequence Expression *)
val tseq01 = false ; 1 ; true ; 42

begin
  test tint01 43;
  test tint02 2;
  test tint03 (-2);

  test tbool01 true ;
  test tbool02 false ;
  test tbool03 true ;
  test tbool04 false ;
  test tbool05 true ;
  test tbool06 false ;
  test tbool07 true ;
  test tbool08 true ;
  test tbool09 true ;
  test tbool10 true ;

  test tlet01 43;
  test tlet02 89;
  test tlet03 42;
  test tlet04 6;

  test tfun01 9  ;
  test tfun02 13 ;
  test tfun03 92 ;
  test tfun04 7  ;
  test tfun05 7  ;
  test tfun06 79 ;
  test (tfun07 8) 10 ;
  test (tfun08 2) 4;
  test (tfun09 5) 27;  
  test tfun10 1;
  test tfun12 false;
  test tfun14 false;
  test (tfun15 false) false;
  test (tfun15 true) true;
  test (tfun16 34) 35;
  test (tfun17 2 34) 36;
  test (tfun18 3 5) 3;
  test (tfun19 3 5) 5;
  test (tfun20 (fn x->x*2) (fn x->x+1) 2) 5;
  test tfun23 5;
  test tfun24 0;
  test tfun25 32;
  test tfun26 3;

  test tfun27 120;
  test tfun28 1;
  test tfun29 14;
  test tfun30 89;
  test tfun31 89;
  test tfun32 84;
  test tfun33 163;
  test (tfun34 4 10) 0;
  test (tfun35 1 2 3 4) 11;
  
  test tseq01 42

end
