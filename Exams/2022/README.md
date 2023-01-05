# Exam 2022

## Exercise 2.5

```fs
INCSP 1;        // nFac som global variabel
INCSP 1;        // resFac som global variabel
LDARGS;         // Loade parameter n fra kommandolinie
CALL (1, "L1"); // Kalde main med n som argument.
STOP;           // Stop ved retur fra main.

Label "L1";                                    // Main
    INCSP 1;                                   // i som lokal variabel
    GETBP; CSTI 1; ADD; CSTI 0; STI; INCSP -1; // i = 0
    CSTI 0; CSTI 0; STI; INCSP -1;             // nFac = 0
    GOTO "L4";                                 // call while loop

Label "L3";                       // 
    CSTI 1;                       // load global variable resFac
    GETBP; CSTI 1; ADD; LDI;      // load i
    CALL (1,"L2"); STI; INCSP -1; // resFac = fac(i)
    GETBP; CSTI 1; ADD;           // get i
    GETBP; CSTI 1; ADD; LDI;      // load i
    CSTI 1; ADD; STI; INCSP -1;   // i = i + 1
    INCSP 0;                      // decrement stack pointer

Label "L4";                  // while loop
    GETBP; CSTI 1; ADD; LDI; // load i
    GETBP; CSTI 0; ADD; LDI; // load n 
    LT; IFNZRO "L3";         // if i < n then goto L3 else continue
    CSTI 42; PRINTSTACK;     // printstack method with 42
    INCSP -1;                // decrement stack pointer by 1
    RET 0;                   // return

Label "L2";                      // function fac
    CSTI 0; CSTI 0; LDI; CSTI 1; ADD; STI; INCSP -1; // nFac = nFac + 1
    CSTI 0; LDI; PRINTSTACK;     // print stack with argument nFac
    GETBP; CSTI 0; ADD; LDI;     // load n
    CSTI 0; EQ; IFZERO "L5";     // if n == 0 then continue else L5
    CSTI 1; RET 1; GOTO "L6";    // return 1 and goto L6

Label "L5";                               //
    GETBP; CSTI 0; ADD; LDI;              // load n
    GETBP; CSTI 0; ADD; LDI; CSTI 1; SUB; // load n and subtract with 1
    CALL (1,"L2");                        // call fac recursively
    MUL; RET 1;                           // multiple n with fac(n-1) and return result 

Label "L6";        //
    INCSP 0; RET 0 // go back to callee
```

## Exercise 2.6

```fs
MicroC % java Machine fac.out 1
-Print Stack 1----------------
Stack Frame // Funktion fac
s[9]: Local/Temp = 0 // Stakplads til lokal variabel n
s[8]: bp = 4
s[7]: ret = 39
Stack Frame // Funktion main
s[6]: Local/Temp = 1 // Stakplads til lokal variabel n
s[5]: Local/Temp = 0 // Stakplads til lokal variabel i
s[4]: Local/Temp = 1 // Temporær værdi til 1 (fra i < n)
s[3]: bp = -999
s[2]: ret = 8
Global
s[1]: 0 // resFac
s[0]: 1 // nFac

-Print Stack 42----------------
Stack Frame // Main
s[5]: Local/Temp = 1 // Stakplads til lokal variabel n
s[4]: Local/Temp = 1 // Stakplads til lokal variabel i
s[3]: bp = -999
s[2]: ret = 8
Global
s[1]: 1 // resFac
s[0]: 1 // nFac
Ran 0.028 seconds
MicroC %
```
