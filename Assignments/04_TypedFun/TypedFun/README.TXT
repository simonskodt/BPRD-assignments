Compiling and loading the typed micro-ML evaluator (TypedFun/README.TXT)
------------------------------------------------------------------------

A. Loading evaluator and type checker for the explicitly typed language

   fsharpi TypedFun.fs

   open TypedFun;;

   // These typechecks should succeed:
   typeCheck (Prim("+", CstI 5, CstI 7));;
   typeCheck ex1;;

   // This typecheck should throw exception:
   typeCheck exErr1;;

   #q;;
