# Assignment 12

## Exercise 13.1

Console output from the compiler:

```fsharp
/microsmlc.exe -opt -verbose -eval ex09.sml
Micro-SML compiler v 1.1 of 2018-11-18
Compiling ex09.sml to ex09.out

Program after alpha conversion (exercise):
fun f x = if (x < 0) then g 4 else f (x - 1)
and g x = x
begin
  print(f 2)
end
Program with tailcalls:
fun f x = if (x < 0) then g_tail 4 else f_tail (x - 1)
and g x = x
begin
  print(f 2)
end
Program with types:
fun f x = if (x:int < 0:int):bool then g:(int -> int)_tail 4:int:int else f:(int -> int)_tail (x:int - 1:int):int:int
and g x = x:int
begin
  print(f:(int -> int) 2:int:int):int
end
Result type: int

Evaluating Program
4 
Result value: Result (Int 4)
Used: Elapsed 24ms, CPU 30ms
Compiled to ex09.out
LABEL G_ExnVar_L2
     0: CSTI 0
     2: CSTI 0
     4: STI
LABEL G_Valdecs_L3
     5: ACLOS 1
     7: ACLOS 1
     9: PUSHLAB LabFunc_f_L4
    11: CSTI 1
    13: LDI
    14: HEAPSTI 1
    16: INCSP -1
    18: PUSHLAB LabFunc_g_L5
    20: CSTI 2
    22: LDI
    23: HEAPSTI 1
    25: INCSP -1
    27: GETSP
    28: CSTI 2
    30: SUB
    31: CALL 0 L1
    34: STI
    35: INCSP -3
    37: STOP
LABEL LabFunc_f_L4
    38: GETBP
    39: CSTI 1
    41: ADD
    42: LDI
    43: CSTI 0
    45: LT
    46: IFZERO L6
    48: CSTI 2
    50: LDI
    51: CSTI 4
    53: TCLOSCALL 1
LABEL L6
    55: GETBP
    56: LDI
    57: GETBP
    58: CSTI 1
    60: ADD
    61: LDI
    62: CSTI 1
    64: SUB
    65: TCLOSCALL 1
LABEL LabFunc_g_L5
    67: GETBP
    68: CSTI 1
    70: ADD
    71: LDI
    72: RET 2
LABEL L1
    74: CSTI 1
    76: LDI
    77: CSTI 2
    79: CLOSCALL 1
    81: PRINTI
    82: RET 0

```

Console output from executing:

```bash
MsmlVM/src/msmlmachine ex09.out             
4 
Result value: 4
Used 0 cpu milli-seconds
```

1 - What is the result value of running ex09.out?

4

2 - What type does the result value have? (Look at the result produced by the inter-
preter).

Result type: int

3 - What application calls have been annotated as tail calls? Explain how this matches
the intuition behind a tail call.

```fsharp
fun f x = if (x < 0) then g_tail 4 else f_tail (x - 1)
and g x = x
begin
  print(f 2)
end
```

Since this is a if-else condition, we know that either of the functions are called, but not both. In both branches a function is called as the last execution. Therefore they are tailcalls.

4 - What type has been annotated for the call sites to the functions f and g? Function
f is called in two places, and g in one place.

g:(int -> int) and f:(int -> int).

They are both determined at the compile time, to be int -> int.

5 - What is the running time for executing the example using the evaluator, and what
is the running time using the byte code ex09.out using msmlmachine?

Evaluator: CPU 30ms
Byte code: CPU 0ms

The difference is, that the bytecode is already compiled, while the evaulator must first compile it, then run it.

6 - Now compile the example ex09.sml without optimizations. How many byte
code instructions did the optimization save for this small example?

Optimized: 82 instructions
Not-optimized: 89 instructions.

The optimized compiler saves 7 byte code instructions.

## Exercise 13.2

## Exercise 13.3
