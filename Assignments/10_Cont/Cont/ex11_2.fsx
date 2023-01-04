// 11_2 part i
let rec revc (xs: 'a list) (c: ('a list -> 'a list)) : 'a list =
    match xs with
    | []      -> c []
    | x :: xs -> revc xs (fun r -> c(r @ [x]))

revc [1; 2; 3] id;;

// 11_2 part ii
revc [1; 2; 3] (fun v -> v @ v);;

// 11_2 part iii
let rec revi xs acc =
    match xs with
    | []      -> acc
    | x :: xs -> revi xs (x :: acc)

revi [1; 2; 3] [];;