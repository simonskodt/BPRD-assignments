fslex --unicode CLex.fsl
fsyacc --module CPar CPar.fsy
dotnet fsi -r /home/faur/Programming/ITU/5th_Semester/PRDAT/BPRD-assignments/libraries/FsLexYacc.Runtime.dll \
Absyn.fs CPar.fs CLex.fs Parse.fs Machine.fs Comp.fs ParseAndComp.fs \
# --exec ParseScript.fsx