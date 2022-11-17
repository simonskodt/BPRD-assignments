# Assignment 10

## Exercise 11.1

### Part i

```fsharp
let rec lenc list c =
    match list with 
    | [] -> c 0
    | _::xs -> lenc xs (fun r -> c(1+r));;
val lenc: list: 'a list -> c: (int -> 'b) -> 'b

> (printf "The answer is ’%d’\n") (lenc [2; 5; 7] id);;
The answer is ’3’
val it: unit = ()
```

### Part ii

```fsharp
> (printf "The answer is ’%d’\n") (lenc [2; 5; 7] (fun v -> 2*v));;
The answer is ’6’
val it: unit = ()
```

### Part iii

```fsharp
> let leni list acc =
-     let rec aux list acc =
-         match list with
-         | [] -> acc
-         | _::xs -> aux xs (acc + 1)
-     aux list acc;;
val leni: list: 'a list -> acc: int -> int

> (printf "The answer is ’%d’\n") (leni [2; 5; 7] 0);;
The answer is ’3’
val it: unit = ()
```

The relation between lenc and leni, is that they both use the heap instead of the stack.

## Exercise 11.2

### Part i

```fsharp
> let rec revc (xs: 'a list) (c: ('a list -> 'a list)) : 'a list =
-     match xs with
-     | [] -> c []
-     | x :: xs -> revc xs (fun r -> c(r @ [x]));;
val revc: xs: 'a list -> c: ('a list -> 'a list) -> 'a list

> (printf "The answer is ’%A’\n") (revc [1; 2; 3] id);;;;
The answer is ’[3; 2; 1]’
val it: unit = ()
```

### Part ii

```fsharp
> (printf "The answer is ’%A’\n") (revc [1; 2; 3] (fun v -> v @ v));;;;
The answer is ’[3; 2; 1; 3; 2; 1]’
val it: unit = ()
```
If you call revc with the function `(fun v -> v @ v)`, then it reverses the list twice. So, it duplicates the reversed list.

### Part iii

```fsharp
> let revi xs acc =
-     let rec aux xs acc =
-         match xs with
-         | [] -> acc
-         | x :: xs -> aux xs (x :: acc)
-     aux xs acc;;
val revi: xs: 'a list -> acc: 'a list -> 'a list

> (printf "The answer is ’%A’\n") (revi [1; 2; 3] []);;;;
The answer is ’[3; 2; 1]’
val it: unit = ()
```

## Exercise 11.3

```fsharp
> let rec prodc xs c =
-     match xs with
-     | [] -> c 1
-     | x :: xs -> prodc xs (fun r -> c(r * x));;
val prodc: xs: int list -> c: (int -> 'a) -> 'a

> (printf "The answer is ’%d’\n") (prodc [1; 2; 3] id);;
The answer is ’6’
val it: unit = ()
```

## Exercise 11.4



## Exercise 11.8
