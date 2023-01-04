(* Below is a solution the the n-Queens Program.
   The source code is a small re-write of the solution found on 
   this url:
     http://rosettacode.org/wiki/N-queens_problem#Standard_ML
*)

fun abs x = if x < 0 then -x else x

fun pair x = fn y -> x :: y :: nil  (* Represent a coordinate as a list with two elements *)
fun fst p = hd p
fun snd p = hd (tl p)				   

(* 
 * val threat : (int * int) -> (int * int) -> bool
 * Returns true iff the queens at the given positions threaten each other
 *)
fun threat p1 = fn p2 ->
  let val x1 = fst p1
      val y1 = snd p1
      val x2 = fst p2
      val y2 = snd p2
  in
    x1 = x2 || y1 = y2 || abs (x1-x2) = abs (y1-y2)
  end

fun exists p = fn xs ->
  if isnil xs then false else p (hd xs) || exists p (tl xs)

(*
 * val conflict : (int * int) -> (int * int) list -> bool
 * Returns true if there exists a conflict with the position and the list of queens.
 *)
fun conflict pos = exists (threat pos)

(*
 * val addqueen : (int * int * (int * int) list * (unit -> (int * int) list option)) -> (int * int) list option
 * Returns either NONE in the case that no solution exists or SOME(l) where l is a list of positions making up the solution.
 *)
fun addqueen i = fn n -> fn qs -> fn fc ->
  let
    fun doTry j =
      if j > n then fc 0
      else if conflict (pair i j) qs then doTry (j + 1)
      else if i = n then (pair i j)::qs
      else addqueen (i + 1) n (pair i j :: qs)  (fn x -> doTry (j + 1))
  in
    doTry 1
  end

(*
 * val queens : int -> (int * int) list option
 * Given the board dimension n, returns a solution for the n-queens problem.
 *)
fun queens n = addqueen 1 n nil (fn x -> nil)

fun doAll n =
  if n < 1 then nil else queens n :: doAll (n-1)
			
begin
  (* n=2: nil *)
  (* n=8: [(8,4),(7,2),(6,7),(5,3),(4,6),(3,8),(2,5),(1,1)] *)
  print(doAll 7)

end
												  
