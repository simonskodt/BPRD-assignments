(* File MicroC/ParseAndContcomp.fs *)
module ParseAndContcomp

let fromString = Parse.fromString

let fromFile = Parse.fromFile

let contCompileToFile = Contcomp.contCompileToFile

let compile p = contCompileToFile (fromFile (p+".c")) (p+".out")
