Compiling and loading the micro-ML evaluator and parser (Fun/README.TXT)
------------------------------------------------------------------------

A. Loading the micro-ML evaluator, with abstract syntax only

   fsharpi Absyn.fs Fun.fs

   open Absyn;;
   open Fun;;
   let res = run (Prim("+", CstI 5, CstI 7));;
   #q;;


B. Generating and compiling the lexer and parser, and loading them:

   fsyacc --module FunPar FunPar.fsy
   fslex --unicode FunLex.fsl
   fsharpi -r ~/fsharp/FsLexYacc.Runtime.dll Absyn.fs FunPar.fs FunLex.fs Parse.fs   

   open Parse;;
   let e1 = fromString "5+7";;
   let e2 = fromString "let y = 7 in y + 2 end";;
   let e3 = fromString "let f x = x + 7 in f 2 end";;


C. Using the lexer, parser and first-order evaluator together:

   fsyacc --module FunPar FunPar.fsy
   fslex --unicode FunLex.fsl
   fsharpi -r ~/fsharp/FsLexYacc.Runtime.dll Absyn.fs FunPar.fs FunLex.fs Parse.fs Fun.fs ParseAndRun.fs

   open ParseAndRun;;
   run (fromString "5+7");;
   run (fromString "let y = 7 in y + 2 end");;
   run (fromString "let f x = x + 7 in f 2 end");;


