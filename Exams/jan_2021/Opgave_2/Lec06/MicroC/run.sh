fslex --unicode CLex.fsl
fsyacc --module CPar CPar.fsy
fsharpi -r /home/faur/Programming/ITU/5th_Semester/PRDAT/BPRD-assignments/libraries/FsLexYacc.Runtime.dll \
Absyn.fs CPar.fs CLex.fs Parse.fs Interp.fs ParseAndRun.fs Machine.fs Comp.fs ParseAndComp.fs   
