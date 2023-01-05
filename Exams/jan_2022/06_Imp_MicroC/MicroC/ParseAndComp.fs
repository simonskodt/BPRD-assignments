(* File MicroC/ParseAndComp.fs *)

module ParseAndComp

let fromString = Parse.fromString

let fromFile = Parse.fromFile

let compileToFile = Comp.compileToFile

let compileToInsts = Comp.cProgram

let compile p = compileToFile (fromFile (p+".c")) (p+".out")
