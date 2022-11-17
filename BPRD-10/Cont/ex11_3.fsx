// 11_3 part i
let rec prodc xs c =
    match xs with
    | [] -> c 1
    | x :: xs -> prodc xs (fun r -> c(r * x))

(printf "The answer is ’%d’\n") (prodc [1; 2; 3] id)