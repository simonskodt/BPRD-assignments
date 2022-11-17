// 11_2 part i
let rec revc (xs: 'a list) (c: ('a list -> 'a list)) : 'a list =
    match xs with
    | [] -> c []
    | x :: xs -> revc xs (fun r -> c(r @ [x]))

revc [1; 2; 3] id;;

// 11_2 part ii
revc [1; 2; 3] (fun v -> v @ v);;

// 11_2 part iii
let revi xs acc =
    let rec aux xs acc =
        match xs with
        | [] -> acc
        | x :: xs -> aux xs (x :: acc)
    aux xs acc

revi [1; 2; 3] [];;