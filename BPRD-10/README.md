# Assignment 10

## Exercise 11.1

### Part i
```fsharp
> let rec lenc list c =
-     match list with 
-     | [] -> c 0
-     | _::xs -> lenc xs (fun r -> c(1+r));;
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

## Exercise 11.2

## Exercise 11.3

## Exercise 11.4

## Exercise 11.8
