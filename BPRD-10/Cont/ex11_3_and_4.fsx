// 11_3 part i
let rec prodc xs c =
    match xs with
    | [] -> c 1
    | x :: xs -> prodc xs (fun r -> c(r * x))

(printf "The answer is ’%d’\n") (prodc [1; 2; 3] id)

// 11_4
let rec prodc_optimized xs c =
     match xs with
    | [] -> c 1
    | x :: _ when x = 0 -> 0
    | x :: xs -> prodc xs (fun r -> c(r * x))

(printf "The answer is ’%d’\n") (prodc_optimized [1; 2; 0; 3] id)