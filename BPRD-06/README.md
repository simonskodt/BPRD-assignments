# BPRD-06
Assignment 6 in Programmer som data.

## Exercises

### Exercise 7.1

```fsharp
> fromFile "ex1.c";;
val it: Absyn.program =
  Prog
    [Fundec
       (None, "main", [(TypI, "n")],
        Block
          [Stmt
             (While
                (Prim2 (">", Access (AccVar "n"), CstI 0),   
                 Block
                   [Stmt (Expr (Prim1 ("printi", Access (AccVar "n"))));
                    Stmt
                      (Expr
                         (Assign
                            (AccVar "n",
                             Prim2 ("-", Access (AccVar "n"), CstI 1))))]));
           Stmt (Expr (Prim1 ("printc", CstI 10)))])]
```


Indicate which parts are declarations, statements, types and expressions:

- Declarations: `Prog` and `Fundec`. 
- Statements: `Block`, `Stmt`, `While`, `Expr`.
- Types: `TypI`.
- Expressions: `Prim2`, `Access`, `AccVar`, `CstI`, `Prim1` and `Assign`.


```fsharp
> run (fromFile "ex1.c") [17];;
17 16 15 14 13 12 11 10 9 8 7 6 5 4 3 2 1
```

```fsharp
> run (fromFile "ex11.c") [8];;
1 5 8 6 3 7 2 4 
1 6 8 3 7 4 2 5
1 7 4 6 8 2 5 3
1 7 5 8 2 4 6 3
2 4 6 8 3 1 7 5 
2 5 7 1 3 8 6 4
2 5 7 4 1 8 6 3
2 6 1 7 4 8 3 5 
2 6 8 3 1 4 7 5
2 7 3 6 8 5 1 4
2 7 5 8 1 4 6 3
2 8 6 1 3 5 7 4 
3 1 7 5 8 2 4 6
3 5 2 8 1 7 4 6
3 5 2 8 6 4 7 1
3 5 7 1 4 2 8 6 
3 5 8 4 1 7 2 6
3 6 2 5 8 1 7 4
3 6 2 7 1 4 8 5
3 6 2 7 5 1 8 4
3 6 4 1 8 5 7 2
3 6 4 2 8 5 7 1
3 6 8 1 4 7 5 2
3 6 8 1 5 7 2 4 
3 6 8 2 4 1 7 5
3 7 2 8 5 1 4 6
3 7 2 8 6 4 1 5
3 8 4 7 1 6 2 5
4 1 5 8 2 7 3 6
4 1 5 8 6 3 7 2 
4 2 5 8 6 1 3 7
4 2 7 3 6 8 1 5
4 2 7 3 6 8 5 1
4 2 7 5 1 8 6 3
4 2 8 5 7 1 3 6
4 2 8 6 1 3 5 7
4 6 1 5 2 8 3 7 
4 6 8 2 7 1 3 5
4 6 8 3 1 7 5 2
4 7 1 8 5 2 6 3
4 7 3 8 2 5 1 6
4 7 5 2 6 1 3 8 
4 7 5 3 1 6 8 2
4 8 1 3 6 2 7 5
4 8 1 5 7 2 6 3
4 8 5 3 1 7 2 6
5 1 4 6 8 2 7 3
5 1 8 4 2 7 3 6
5 1 8 6 3 7 2 4
5 2 4 6 8 3 1 7 
5 2 4 7 3 8 6 1
5 2 6 1 7 4 8 3
5 2 8 1 4 7 3 6
5 3 1 6 8 2 4 7 
5 3 1 7 2 8 6 4
5 3 8 4 7 1 6 2
5 7 1 3 8 6 4 2
6 4 7 1 3 5 2 8
6 4 7 1 8 2 5 3
6 8 2 4 1 7 5 3
7 1 3 8 6 4 2 5
7 2 4 1 8 5 3 6
7 2 6 3 1 4 8 5
7 3 1 6 8 5 2 4
7 3 8 2 5 1 6 4
7 4 2 5 8 1 3 6
7 4 2 8 6 1 3 5
7 5 3 1 6 8 2 4
8 2 4 1 7 5 3 6
8 2 5 3 1 7 4 6
8 3 1 6 2 5 7 4
8 4 1 3 6 2 7 5
val it: Interp.store =
  map
    [(0, 8); (1, 0); (2, 9); (3, -999); (4, 0); (5, 0); (6, 0); (7, 0); (8, 0);
     ...]
```

### Exercise 7.2



### Exercise 7.3

### Exercise 7.4

### Exercise 7.5