# Assignment 10

## Exercise 11.1

### Part i

```fsharp
> let rec lenc list c =
-     match list with 
-     | []    -> c 0
-     | _::xs -> lenc xs (fun r -> c(1+r));;
val lenc: list: 'a list -> c: (int -> 'b) -> 'b

> lenc [2; 5; 7] id;;
val it: int = 3

> lenc [2; 5; 7] (printf "The answer is '%d'\n")- ;;
The answer is '3'
val it: unit = ()
```

### Part ii

```fsharp
> lenc [2; 5; 7] (fun v -> 2*v);;
6
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

> leni [2; 5; 7] 0;;
3
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

> revc [1; 2; 3] id;;
[3; 2; 1]
```

### Part ii

```fsharp
> revc [1; 2; 3] (fun v -> v @ v);;
[3; 2; 1; 3; 2; 1]
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

> revi [1; 2; 3] [];;
[3; 2; 1]
```

## Exercise 11.3

```fsharp
> let rec prodc xs c =
-     match xs with
-     | [] -> c 1
-     | x :: xs -> prodc xs (fun r -> c(r * x));;
val prodc: xs: int list -> c: (int -> 'a) -> 'a

> prodc [1; 2; 3] id;;
6
```

## Exercise 11.4



## Exercise 11.8
