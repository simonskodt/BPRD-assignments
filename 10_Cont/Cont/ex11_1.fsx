// 11_1 part i
let rec lenc list c =
    match list with 
    | []    -> c 0
    | _::xs -> lenc xs (fun r -> c(1+r))

lenc [2; 5; 7] id

lenc [2; 5; 7] (printf "The answer is '%d'\n")

// 11_1 part ii
lenc [2; 5; 7] (fun v -> 2*v)


// 11_1 part iii
let rec leni list acc =
    match list with
    | []    -> acc
    | _::xs -> leni xs (acc + 1)

leni [2; 5; 7] 0