Compiling and loading continuation-based evaluators (cont/README.TXT)
---------------------------------------------------------------------

Archive cont.zip contains the files used below.

A. Loading two continuation-based interpreters for a functional
   language with exceptions:

   fsharpi Contfun.fs

   open Contfun;;
   eval1 ex1 [];; 
   eval1 ex2 [("n", Int 10)];
   #q;;


B. Loading two continuation-based interpreters for an imperative
   language with exceptions:

   fsharpi Contimp.fs

   open Contimp;;
   run1 ex1;;
   run1 ex2;;
   run2 ex3;;


C. Loading a continuation-based interpreter for micro-Icon, a language
   in which an expression can have multiple results:

   fsharpi Icon.fs

   open Icon;;
   run ex1;;
   run ex2;;
   run ex3and;;
   run ex3or;;


D. Compile and run a Java implementation of factorial in
   continuation-passing style:

   javac Factorial.java
   java Factorial 10


E. Compile and run example illustrating longjmp and setjmp in C (under Unix):

   gcc testlongjmp.c -o testlongjmp
   ./testlongjmp 10
   ./testlongjmp 11
