# BPRD Assignment 3

Here, we will put our none-code answers.

## Exercise 3.3
Write out the rightmost derivation of the string below from the ex-
pression grammar at the end of Sect. 3.6.6, corresponding to ExprPar.fsy. Take
note of the sequence of grammar rules (A–I) used.

`let z = (17) in z + 2 * 3 end EOF`

```txt
    Main
(A) Expr EOF
(F) LET NAME EQ Expr IN Expr END
(G) LET NAME EQ Expr IN Expr TIMES Expr END
(C) LET NAME EQ Expr IN Expr TIMES CSTINT END
(H) LET NAME EQ Expr IN Expr PLUS Expr TIMES CSTINT END
(C) LET NAME EQ Expr IN Expr PLUS CSTINT TIMES CSTINT END
(B) LET NAME EQ Expr IN NAME PLUS CSTINT TIMES CSTINT END
(E) LET NAME EQ LPAR Expr RPAR IN NAME PLUS CSTINT TIMES CSTINT END
(C) LET NAME EQ LPAR CSTINT RPAR IN NAME PLUS CSTINT TIMES CSTINT END
```

## Exercise 3.4

![Derivation as a tree](/derivation_tree.png)

## Exercise 3.5

We went through the different examples in `fsi`; this worked as expected.

### fslex

```cmd
fslex --unicode ExprLex.fsl
compiling to dfas (can take a while...)
15 states
writing output
```

### fsyacc

```cmd
fsyacc ExprPar ExprPar.fsy
        building tables
computing first function...        time: 00:00:00.0528840
building kernels...        time: 00:00:00.0323981
building kernel table...        time: 00:00:00.0098326
computing lookahead relations.................................        time: 00:00:00.0323278
building lookahead table...        time: 00:00:00.0093833
building action table...        shift/reduce error at state 23 on terminal TIMES between {[explicit left 10000] shift(27)} and {noprec reduce(Expr:'IF' Expr 'THEN' Expr 'ELSE' Expr)} - assuming the former because we prefer shift when unable to compare precedences
        shift/reduce error at state 23 on terminal PLUS between {[explicit left 9999] shift(28)} and {noprec reduce(Expr:'IF' Expr 'THEN' Expr 'ELSE' Expr)} - assuming the former because we prefer shift when unable to compare precedences
        shift/reduce error at state 23 on terminal MINUS between {[explicit left 9999] shift(29)} and {noprec reduce(Expr:'IF' Expr 'THEN' Expr 'ELSE' Expr)} - assuming the former because we prefer shift when unable to compare precedences
        time: 00:00:00.0322937
        building goto table...        time: 00:00:00.0021206
        returning tables.
        3 shift/reduce conflicts
        consider setting precedences explicitly using %left %right and %nonassoc on terminals and/or setting explicit precedence on rules using %prec
        30 states
        3 nonterminals
        19 terminals
        11 productions
        #rows in action table: 30
```

### Outputs from examples

```fsharp
> fromString "1 + 2 * 3";;
val it: Absyn.expr = Prim ("+", CstI 1, Prim ("*", CstI 2, CstI 3))

> fromString "1 - 2 - 3";;
val it: Absyn.expr = Prim ("-", Prim ("-", CstI 1, CstI 2), CstI 3)

> fromString "1 + -2";;
val it: Absyn.expr = Prim ("+", CstI 1, CstI -2)

> fromString "x++";;
System.Exception: parse error near line 1, column 3

   at Microsoft.FSharp.Core.PrintfModule.PrintFormatToStringThenFail@1448.Invoke(String message)
   at FSI_0002.Parse.fromString(String str) in /home/faur/Programming/ITU/5th_Semester/PRDAT/BPRD-03/Expr/Parse.fs:line 21
   at <StartupCode$FSI_0013>.$FSI_0013.main@() in /home/faur/Programming/ITU/5th_Semester/PRDAT/BPRD-03/Expr/stdin:line 36
Stopped due to error

> fromString "1 + 1.2";;
System.Exception: Lexer error: illegal symbol near line 1, column 6

   at Microsoft.FSharp.Core.PrintfModule.PrintFormatToStringThenFail@1448.Invoke(String message)
   at FSI_0002.Parse.fromString(String str) in /home/faur/Programming/ITU/5th_Semester/PRDAT/BPRD-03/Expr/Parse.fs:line 21
   at <StartupCode$FSI_0014>.$FSI_0014.main@() in /home/faur/Programming/ITU/5th_Semester/PRDAT/BPRD-03/Expr/stdin:line 37
Stopped due to error

> fromString "1 + ";;
System.Exception: parse error near line 1, column 4

   at Microsoft.FSharp.Core.PrintfModule.PrintFormatToStringThenFail@1448.Invoke(String message)
   at FSI_0002.Parse.fromString(String str) in /home/faur/Programming/ITU/5th_Semester/PRDAT/BPRD-03/Expr/Parse.fs:line 21
   at <StartupCode$FSI_0015>.$FSI_0015.main@() in /home/faur/Programming/ITU/5th_Semester/PRDAT/BPRD-03/Expr/stdin:line 38
Stopped due to error

> fromString "let z = (17) in z + 2 * 3 end";;
val it: Absyn.expr =
  Let ("z", CstI 17, Prim ("+", Var "z", Prim ("*", CstI 2, CstI 3)))

> fromString "let z = 17) in z + 2 * 3 end";;
System.Exception: parse error near line 1, column 11

   at Microsoft.FSharp.Core.PrintfModule.PrintFormatToStringThenFail@1448.Invoke(String message)
   at FSI_0002.Parse.fromString(String str) in /home/faur/Programming/ITU/5th_Semester/PRDAT/BPRD-03/Expr/Parse.fs:line 21
   at <StartupCode$FSI_0017>.$FSI_0017.main@() in /home/faur/Programming/ITU/5th_Semester/PRDAT/BPRD-03/Expr/stdin:line 40
Stopped due to error
> fromString "let in = (17) in z + 2 * 3 end";;
System.Exception: parse error near line 1, column 6

   at Microsoft.FSharp.Core.PrintfModule.PrintFormatToStringThenFail@1448.Invoke(String message)
   at FSI_0002.Parse.fromString(String str) in /home/faur/Programming/ITU/5th_Semester/PRDAT/BPRD-03/Expr/Parse.fs:line 21
   at <StartupCode$FSI_0018>.$FSI_0018.main@() in /home/faur/Programming/ITU/5th_Semester/PRDAT/BPRD-03/Expr/stdin:line 41
Stopped due to error

> fromString "1 + let x=5 in let y=7+x in y+y end + x end";;
val it: Absyn.expr =
  Prim
    ("+", CstI 1,
     Let
       ("x", CstI 5,
        Prim
          ("+",
           Let
             ("y", Prim ("+", CstI 7, Var "x"), Prim ("+", Var "y", Var "y")),
           Var "x")))
```


## Exercise 3.6

Found in file `Parse.fs`.

## Exercise 3.7

Found in files:
```
Absyn.fs
ExprLex.fsl
ExprPar.fsy
```