# BPRD-06

Assignment 7 in 'Programmer som data'.

## Exercises

### Exercise 8.1

**Part (i)**

```fsharp
> open ParseAndComp;;
> compileToFile (fromFile "ex11.c") "ex11.out";;
val it: Machine.instr list =
  [LDARGS; CALL (1, "L1"); STOP; Label "L1"; INCSP 1; INCSP 1; INCSP 100;
   GETSP; CSTI 99; SUB; INCSP 100; GETSP; CSTI 99; SUB; INCSP 100; GETSP;
   CSTI 99; SUB; INCSP 100; GETSP; CSTI 99; SUB; GETBP; CSTI 2; ADD; CSTI 1;
   STI; INCSP -1; GOTO "L3"; Label "L2"; GETBP; CSTI 103; ADD; LDI; GETBP;
   CSTI 2; ADD; LDI; ADD; CSTI 0; STI; INCSP -1; GETBP; CSTI 2; ADD; GETBP;
   CSTI 2; ADD; LDI; CSTI 1; ADD; STI; INCSP -1; INCSP 0; Label "L3"; GETBP;
   CSTI 2; ADD; LDI; GETBP; CSTI 0; ADD; LDI; SWAP; LT; NOT; IFNZRO "L2";
   GETBP; CSTI 2; ADD; CSTI 1; STI; INCSP -1; GOTO "L5"; Label "L4"; GETBP;
   CSTI 204; ADD; LDI; GETBP; CSTI 2; ADD; LDI; ADD; GETBP; CSTI 305; ADD; LDI;
   GETBP; CSTI 2; ADD; LDI; ADD; CSTI 0; STI; STI; INCSP -1; GETBP; CSTI 2;
   ADD; ...]

   > javac Machine.java
   > java Machine.java .\ex11.out 8
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
   5 7 1 4 2 8 6 3
   5 7 2 4 8 1 3 6
   5 7 2 6 3 1 4 8
   5 7 2 6 3 1 8 4
   5 7 4 1 3 8 6 2
   5 8 4 1 3 6 2 7 
   5 8 4 1 7 2 6 3
   6 1 5 2 8 3 7 4
   6 2 7 1 3 5 8 4
   6 2 7 1 4 8 5 3
   6 3 1 7 5 8 2 4
   6 3 1 8 4 2 7 5
   6 3 1 8 5 2 4 7
   6 3 5 7 1 4 2 8
   6 3 5 8 1 4 2 7
   6 3 7 2 4 8 1 5
   6 3 7 2 8 5 1 4
   6 3 7 4 1 8 2 5 
   6 4 1 5 8 2 7 3
   6 4 2 8 5 7 1 3
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

   Ran 0.225 seconds
```

**Part (ii)**

```fsharp
> compileToFile (fromFile "ex3.c") "ex3.out";;
val it: Machine.instr list =
  [LDARGS; CALL (1, "L1"); STOP; Label "L1"; INCSP 1; GETBP; CSTI 1; ADD;
  [LDARGS; CALL (1, "L1"); STOP; Label "L1"; INCSP 1; GETBP; CSTI 1; ADD;
   GETBP; CSTI 0; ADD; LDI; STI; INCSP -1; INCSP 1; GETBP; CSTI 0; ADD; LDI;
   GETBP; CSTI 2; ADD; CALL (2, "L2"); INCSP -1; GETBP; CSTI 2; ADD; LDI;
   PRINTI; INCSP -1; INCSP -1; GETBP; CSTI 1; ADD; LDI; PRINTI; INCSP -1;
   INCSP -1; RET 0; Label "L2"; GETBP; CSTI 1; ADD; LDI; GETBP; CSTI 0; ADD;
   LDI; GETBP; CSTI 0; ADD; LDI; MUL; STI; INCSP -1; INCSP 0; RET 1]

> java Machine.java .\ex3.out 3     
  0 1 2
  Ran 0.029 seconds
```

```fsharp
> compileToFile (fromFile "ex5.c") "ex5.out";;
  val it: Machine.instr list =
    [LDARGS; CALL (1, "L1"); STOP; Label "L1"; INCSP 1; GETBP; CSTI 1; ADD;
     GETBP; CSTI 0; ADD; LDI; STI; INCSP -1; INCSP 1; GETBP; CSTI 0; ADD; LDI;
     GETBP; CSTI 2; ADD; CALL (2, "L2"); INCSP -1; GETBP; CSTI 2; ADD; LDI;
     PRINTI; INCSP -1; INCSP -1; GETBP; CSTI 1; ADD; LDI; PRINTI; INCSP -1;
     INCSP -1; RET 0; Label "L2"; GETBP; CSTI 1; ADD; LDI; GETBP; CSTI 0; ADD;
     LDI; GETBP; CSTI 0; ADD; LDI; MUL; STI; INCSP -1; INCSP 0; RET 1]

> java Machine.java .\ex5.out 4
  16 4 
  Ran 0.03 seconds     
```

The structured bytecode for `ex3.c` and `ex5.c` can be found in `ex3_assembly.out` and `ex5_assembly.out`

Executing `ex3.out` in java

```cmd
> java Machine ex3.out 10
0 1 2 3 4 5 6 7 8 9 
Ran 0.019 seconds
```

Machinetrace for `ex3.out` in Java explained can be found in `ex3trace.txt`

### Exercise 8.3

Found in `Absyn.fs`, `CLex.fsl`, `CPar.fsy` and `Comp.fs`. Find Micro-C example in `ex8_3.c`, and compile bytecode found in `ex8_3.out`.

Bytecode run succefully in `Machine.java`.

```cmd
> java Machine.java .\ex8_3.out 3
0 1 2 
Ran 0.002 seconds
```

### Exercise 8.4

**ex8.c**

Structured bytecode for `ex8.c` and `prog1` can be found in `ex8.out` and `prog1.out`.

First of all, prog1 does not need to:

- Setup main.
- Initialise the variable i.
- Store 20_000_000 in i.
- Handle new scope
- For the expression `i = i - 1`, prog1 handles that itself via DUP.

**ex13.c**



### Exercise 8.5

### Exercise 8.6
