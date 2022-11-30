fun test t = fn r -> print (t = r)

fun genList n =
  let
    fun loop n = fn acc ->
      if n < 0 then acc else loop (n-1) (n::acc)
  in
    loop n nil
  end
      
fun printList xs =
  if isnil xs then 0  (* Return arbitrary value as result. *)
  else (print (hd xs);
        printList (tl xs))
      
fun sumList xs =
  if isnil xs then 0 else hd xs + (sumList (tl xs))

fun append xs = fn ys ->
  if isnil xs then
    ys		       
  else
    (hd xs) :: (append (tl xs) ys)

fun reverse xs =
  let
    fun loop xs = fn acc ->
      if isnil xs then acc else loop (tl xs) (hd xs :: acc)
  in
    loop xs nil
  end

fun map f = fn xs ->
  if isnil xs then nil else f (hd xs) :: map f (tl xs)

fun cmp f = fn xs -> fn ys ->
  if isnil xs && isnil ys then true
  else if isnil xs then false (* Lists not of equal length *)
  else if isnil ys then false (* Lists not of equal lenght *)
  else f (hd xs) (hd ys) && cmp f (tl xs) (tl ys)
					     
(* Allocates many cons cells, which immediately die and can be collected *)
fun l01 n =
  if n > 0 then
    let
      val xs = n::nil
    in
      l01 (n-1)
    end
  else
    n

val l02 =
  let
    val xs = ((1 :: 2 :: nil) :: nil) :: nil
  in
    hd hd hd xs
  end		

val l03 =
  let
    val xs = ((1 :: 2 :: nil) :: nil) :: nil
    val xss = xs :: xs:: (append xs xs) :: nil
  in				       
    xss
  end

val l04 =
  let
    fun pair x = fn y -> x :: y :: nil  (* Represent a coordinate as a list with two elements *)
    fun fst p = hd p
    fun snd p = hd (tl p)
    val p = pair 3 4
    val xs = 3 :: 4 :: nil
  in
    test (fst p) 3;
    test (snd p) 4;
    test (cmp (fn x -> fn y -> x = y) p xs) true
  end
      
begin
  test (l01 10) 0;
  test l02 1;
  test (sumList (genList 10)) 55;
  test (sumList (append (genList 10) (genList 10))) 110;
  test (sumList (reverse (append (genList 10) (genList 10)))) 110;
  test (sumList (map (fn x -> 2*x) (reverse (append (genList 10) (genList 10))))) 220;
  test l04 true
end
