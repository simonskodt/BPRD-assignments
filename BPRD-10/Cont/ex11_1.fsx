let rec lenc list c =
    match list with 
    | [] -> c 0
    | _::xs -> lenc xs (fun r -> c(1+r))

(printf "The answer is ’%d’\n") (lenc [2; 5; 7] id)
(printf "The answer is ’%d’\n") (lenc [2; 5; 7] (fun v -> 2*v))