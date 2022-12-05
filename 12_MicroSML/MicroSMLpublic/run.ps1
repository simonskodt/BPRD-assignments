fslex --unicode FunLex.fsl
fsyacc --module FunPar FunPar.fsy

# This command does not work, but the executable is already generated for us
fsc --standalone -r:..\..\libraries\FsLexYacc.Runtime.dll Absyn.fs FunPar.fs FunLex.fs TypeInference.fs HigherFun.fs Machine.fs Contcomp.fs ParseTypeAndRun.fs MicroSMLC.fs -o microsmlc.exe