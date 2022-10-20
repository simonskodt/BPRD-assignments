   fslex --unicode ./MicroC/CLex.fsl
   fsyacc --module CPar ./MicroC/CPar.fsy
   fsharpi -r ./MicroC/FsLexYacc.Runtime.dll ./MicroC/Absyn.fs ./MicroC/CPar.fs ./MicroC/CLex.fs ./MicroC/Parse.fs ./MicroC/Interp.fs ./MicroC/ParseAndRun.fs