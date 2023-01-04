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
> let rec leni list acc =
-    match list with
-    | []    -> acc
-    | _::xs -> leni xs (acc + 1)
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
> let rec revi xs acc =
-   match xs with
-   | []      -> acc
-   | x :: xs -> revi xs (x :: acc)
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

```fsharp
let rec prodc_optimized xs c =
    match xs with
    | []                -> c 1
    | x :: _ when x = 0 -> c 0
    | x :: xs           -> prodc xs (fun r -> c(r * x))
```

```fsharp
> prodc_optimized [1; 2; 0; 3] id;;
val it: int = 0

> prodc_optimized [1; 2; 0; 3] (printf "The answer is '%d'\n");;
The answer is '0'
val it: unit = ()
```

```fsharp
> let rec prodi_optimized xs acc =
-   match xs with
-   | []                -> acc
-   | x :: _ when x = 0 -> 0
-   | x :: xs           -> prodi_optimized aux xs (acc * x)
val prodi_optimized: xs: int list -> acc: int -> int

> prodi_optimized [1; 2; 0; 3] 1;;
val it: int = 0

> prodi_optimized [2; 4] 1;;
val it: int = 8
```

## Exercise 11.8

All files can be found in `Icon.fs`.

### Part i

```fsharp
let ex11_8_ia = Every(Write(Prim("+", CstI 1, (Prim("*", CstI 2, FromTo(1, 4))))));
```

```fsharp
> run ex11_8_ia;;
3 5 7 9 val it: value = Int 0
```

### Part ii

```fsharp
let ex11_8_iia = Write(Prim("<", CstI 50, (Prim("*", CstI 7, FromTo(1, 10)))));

> run ex11_8_iia;; 
56 val it: value = Int 56
```

### Part iii

```fsharp
let ex11_8_iiia = Every(Write(Prim1("sqr", FromTo(3, 6))));;

> run ex11_8_iiia;;
9 16 25 36 val it: value = Int 0

let ex11_8_iiib = Every(Write(Prim1("even", FromTo(1, 7))));;
> run ex11_8_iiib;;
2 4 6 val it: value = Int 0

```

### Part iv

```fsharp
let ex11_8_iva = Every(Write(Prim1("multiples", CstI 3)));;

> run ex11_8_iva;;
... 33000 33003 33006 33009 33012 33015 33018 33021 33024 33027 33030 ...
```
