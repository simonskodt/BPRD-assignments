let queens n =
  let invalidPosition (x1, y1) (x2, y2) = (x1 = x2) || (y1 = y2) || abs (x1 - x2) = abs (y1 - y2)
  let validSolution (queen, board) = board |> Seq.exists (invalidPosition queen) |> not
  // With the function "loop", we're going to move one column at time, placing queens
  // on each row and creating new boards with only valid solutions.
  let rec loop boards y =
    if y = 0 then
      boards
    else
      let boards' = boards 
                    |> Seq.collect(fun board -> [1 .. n] |> Seq.map(fun x -> (x,y),board))
                    |> Seq.filter validSolution 
                    |> Seq.map(fun (pos, xs) -> pos::xs)
      loop boards' (y - 1)
  loop (Seq.singleton([])) n |> Seq.map (List.rev >> List.map fst)

printf "\nQueens= %A" (queens 10)
