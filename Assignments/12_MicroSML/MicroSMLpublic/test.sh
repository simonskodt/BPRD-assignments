fslex --unicode FunLex.fsl
fsyacc --module FunPar FunPar.fsy    

fsharpc --standalone -r:/Users/nielshallenberg/fsharp/FsLexYacc.Runtime.dll Absyn.fs FunPar.fs FunLex.fs TypeInference.fs HigherFun.fs Machine.fs Contcomp.fs ParseTypeAndRun.fs MicroSMLC.fs -o microsmlc.exe

#fsharpc --standalone -r:/Users/nh/fsharp/FsLexYacc.Runtime.dll Absyn.fs FunPar.fs FunLex.fs TypeInference.fs HigherFun.fs Machine.fs Comp.fs ParseTypeAndRun.fs MicroSMLC.fs -o microsmlc.exe

gcc -Wall msmlmachinewgc.c -o msmlmachine

mono microsmlc.exe -opt -eval ex01.sml
./msmlmachine ex01.out

mono microsmlc.exe -opt -eval ex02.sml
./msmlmachine ex02.out

mono microsmlc.exe -opt -eval ex03.sml
./msmlmachine ex03.out

mono microsmlc.exe -opt -eval ex04.sml
./msmlmachine ex04.out

mono microsmlc.exe -opt -eval ex05.sml 
./msmlmachine ex05.out

mono microsmlc.exe -opt -eval test.sml
./msmlmachine -silent test.out

mono microsmlc.exe -opt  testgc.sml
./msmlmachine -silent testgc.out

mono microsmlc.exe -opt -eval queens.sml
./msmlmachine queens.out

mono microsmlc.exe -opt -eval list.sml
./msmlmachine list.out

mono microsmlc.exe -opt -eval exn01.sml
./msmlmachine exn01.out

mono microsmlc.exe -opt -eval exn02.sml
./msmlmachine exn02.out

mono microsmlc.exe -opt -eval exn03.sml
./msmlmachine exn03.out

mono microsmlc.exe -opt -eval exn04.sml
./msmlmachine exn04.out

mono microsmlc.exe -opt -eval exn06.sml
./msmlmachine exn06.out
