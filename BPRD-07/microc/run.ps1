fslex --unicode CLex.fsl
fsyacc --module CPar CPar.fsy
dotnet fsi -r ..\..\libraries\FsLexYacc.Runtime.dll Absyn.fs CPar.fs CLex.fs Parse.fs Machine.fs Comp.fs ParseAndComp.fs