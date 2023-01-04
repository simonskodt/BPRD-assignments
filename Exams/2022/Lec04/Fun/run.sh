fsyacc --module FunPar FunPar.fsy
fslex --unicode FunLex.fsl
dotnet fsi -r /home/faur/Programming/ITU/5th_Semester/PRDAT/BPRD-assignments/libraries/FsLexYacc.Runtime.dll Absyn.fs FunPar.fs FunLex.fs Parse.fs 