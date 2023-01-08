fsyacc --module FunPar FunPar.fsy
fslex --unicode FunLex.fsl
dotnet fsi -r C:\Users\simon\OneDrive\Skrivebord\5_semester\PRDAT_Programmer-som-data\libraiers\FsLexYacc.Runtime.dll Absyn.fs FunPar.fs FunLex.fs Parse.fs HigherFun.fs ParseAndRunHigher.fs