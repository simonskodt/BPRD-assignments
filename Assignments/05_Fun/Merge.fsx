// Manually sorting
let merge (lst1: int list, lst2: int list) =
    let rec aux lst1 lst2 acc =
        match (lst1: int list), (lst2: int list) with
        | (x :: xs), (y :: ys) when x > y -> aux (x :: xs) ys (acc @ [y])
        | (x :: xs), (y :: ys) -> aux xs (y :: ys) (acc @ [x])
        | (x :: xs), [] -> aux xs [] (acc @ [x])
        | [], (y :: ys) -> aux [] ys (acc @ [y])
        | [], [] -> acc
    aux lst1 lst2 []

// Using List.sort
let merge2 (xs, ys) =
    List.sort (xs @ ys)

let xs = [3; 5; 12]
let ys = [2; 3; 4; 7]

merge (xs, ys)
merge2 (xs, ys)