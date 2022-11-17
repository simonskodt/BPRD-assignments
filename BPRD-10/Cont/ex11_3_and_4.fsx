// 11_3 part i
let rec prodc xs c =
    match xs with
    | [] -> c 1
    | x :: xs -> prodc xs (fun r -> c(r * x))

prodc [1; 2; 3] id

// 11_4
let rec prodc_optimized xs c =
     match xs with
        | [] -> c 1
        | x :: _ when x = 0 -> c 0
        | x :: xs -> prodc xs (fun r -> c(r * x))

prodc_optimized [1; 2; 0; 3] id

// This should work maybe? exercise says to do it.
prodc_optimized [1; 2; 0; 3] (printf "The answer is '%d'\n")

let prodi_optimized xs acc =
    let rec aux xs acc =
        match xs with
            | [] -> acc
            | x :: _ when x = 0 -> 0
            | x :: xs -> aux xs (acc * x)
    aux xs acc

prodi_optimized [1; 2; 0; 3] 0
