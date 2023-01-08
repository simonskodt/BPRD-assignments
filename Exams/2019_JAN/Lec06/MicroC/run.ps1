fslex --unicode CLex.fsl
fsyacc --module CPar CPar.fsy
dotnet fsi -r C:\Users\simon\OneDrive\Skrivebord\5_semester\PRDAT_Programmer-som-data\libraiers\FsLexYacc.Runtime.dll Absyn.fs CPar.fs CLex.fs Parse.fs Machine.fs Contcomp.fs ParseAndContcomp.fs   