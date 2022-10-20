#!/bin/bash
cd ./Fun
fsyacc --module FunPar FunPar.fsy
fslex --unicode FunLex.fsl
cd ..
dotnet fsi -r FsLexYacc.Runtime.dll ./Fun/Absyn.fs ./Fun/FunPar.fs ./Fun/FunLex.fs ./Fun/Parse.fs