Compiling and loading the naive imperative evaluator (Imp/README.TXT)
---------------------------------------------------------------------

A. Archive imp.zip contains the files used below.

   Loading and running the naive imperative language

   fsharpi Naive.fs

   open Naive;;
   run ex1;;
   run ex2;;

B. To compile array.c you can use gcc on Linux/Mac:

   gcc array.c

   and then execute the result program a.out in a terminal.

  ./a.out
  11 22
  22 22
  11 22

C. To run Parameters.cs do the following on a Mac/Linux:

   mcs Parameters.cs

   mcs is the Mono C# compiler. Maybe you have another C# compiler installed.

   mono Parameters.exe 11 22
