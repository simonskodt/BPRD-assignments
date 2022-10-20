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

## Exercise 3.6

Found in file `Parse.fs`.

## Exercise 3.7

Found in files:
```
Absyn.fs
ExprLex.fsl
ExprPar.fsy
```