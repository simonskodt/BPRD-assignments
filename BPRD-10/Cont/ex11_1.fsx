// 11_1 part i
let rec lenc list c =
    match list with 
    | []    -> c 0
    | _::xs -> lenc xs (fun r -> c(1+r))

(printf "The answer is ’%d’\n") (lenc [2; 5; 7] id)

// 11_1 part ii
(printf "The answer is ’%d’\n") (lenc [2; 5; 7] (fun v -> 2*v))


// 11_1 part iii
let leni list acc =
    let rec aux list acc =
        match list with
        | [] -> acc
        | _::xs -> aux xs (acc + 1)
    aux list acc

(printf "The answer is ’%d’\n") (leni [2; 5; 7] 0)