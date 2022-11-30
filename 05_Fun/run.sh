#!/bin/bash
fsyacc --module FunPar ./Fun/FunPar.fsy
fslex --unicode ./Fun/FunLex.fsl
dotnet fsi -r ./FsLexYacc.Runtime.dll ./Fun/Absyn.fs ./Fun/FunPar.fs ./Fun/FunLex.fs ./Fun/Parse.fs ./Fun2/HigherFun.fs ./Fun2/ParseAndRunHigher.fs ./Fun2/TypeInference.fs ./Fun2/TypedFun.fs ./Fun2/ParseAndType.fs