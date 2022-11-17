(* Exercise 11.1 *)

// (i)
let rec lenc list c =
    match list with 
    | []    -> c 0
    | _::xs -> lenc xs (fun r -> c(1+r))

(printf "The answer is ’%d’\n") (lenc [2; 5; 7] id)

// (ii)
(printf "The answer is ’%d’\n") (lenc [2; 5; 7] (fun v -> 2*v))

// (iii)
let rec leni list acc =
    match list with 
    | []    -> acc
    | _::xs -> leni xs (acc+1)

(printf "The answer is ’%d’\n") (leni [2; 5; 7] 0)