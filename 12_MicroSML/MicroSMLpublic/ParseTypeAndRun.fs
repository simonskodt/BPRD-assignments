(* Lexing and parsing of micro-SML programs using fslex and fsyacc *)
(* Includes evaluation, type inference and compiling.              *)

module ParseTypeAndRun

open System
open System.IO
open System.Text
open FSharp.Text.Lexing
open Absyn

(* Calculate CPU time *)
let cpuTime f x =
  let proc = System.Diagnostics.Process.GetCurrentProcess()
  let cpuTimeStamp = proc.TotalProcessorTime
  let timer = new System.Diagnostics.Stopwatch()
  let _ = timer.Start();
  let r = f x
  let cpuTime = (proc.TotalProcessorTime-cpuTimeStamp).TotalMilliseconds
  let elapsed = timer.ElapsedMilliseconds
  (r,cpuTime,elapsed)

let ppTime (cpuTime,elapsed) =
  sprintf "Elapsed %dms and CPU: %dms" elapsed (int64 cpuTime)


(* Plain parsing from a string, with poor error reporting *)
let fromString (str : string) : program<'a> =
  let lexbuf = LexBuffer<char>.FromString(str)
  try 
    FunPar.Main FunLex.Token lexbuf
  with 
  | exn -> let pos = lexbuf.EndPos 
           failwithf "%s near line %d, column %d\n" 
             (exn.Message) (pos.Line+1) pos.Column
                             
(* Parsing from a file *)
let fromFile (filename : string) =
  use reader = new StreamReader(filename)
  let lexbuf = LexBuffer<char>.FromTextReader reader
  try 
    FunPar.Main FunLex.Token lexbuf
  with 
  | exn -> let pos = lexbuf.EndPos 
           failwithf "%s in file %s near line %d, column %d\n" 
             (exn.Message) filename (pos.Line+1) pos.Column


(* Parsing, infer types and executing in one function *)
let ppType = function
    None -> ""
  | Some t -> ":"+ (TypeInference.showType t)
             
let run s =
  try
    let p = fromString s
    let (typ',env',p') = TypeInference.inferProg p
    let r = HigherFun.evalProg [] p' 
    (sprintf "\nProgram: %s" (Absyn.ppProg ppType p')) +
    (sprintf "\nResult type: %s" (TypeInference.showType typ')) +
    (sprintf "\nResult value: %A" r) +
             "\n"
  with 
    Failure s -> sprintf "ParseTypeAndRun.run giving error %s" s

let compProg' (opt_p,debug_p,verbose_p,eval_p,alpha_p,program,fname) =
  try
    let p = if alpha_p then Absyn.alphaConv program else program
    let _ = if verbose_p && alpha_p
              then printf "\nProgram after alpha conversion (exercise):\n%s"
                    (Absyn.ppProg (fun _ -> "") p)
    let p = Absyn.tailcalls p
    let _ = if verbose_p && opt_p
            then printf "\nProgram with tailcalls:\n%s" (Absyn.ppProg (fun _ -> "") p)
    let (typ',env',p') = TypeInference.inferProg p
    let _ = if verbose_p then
              printf "\nProgram with types:\n%s" (Absyn.ppProg ppType p');
              printf "\nResult type: %s\n" (TypeInference.showType typ')
            else ()
    let _ = if eval_p then
              printf "\nEvaluating Program\n";
              let (r,cpu,elapsed) = cpuTime (HigherFun.evalProg []) p'
              printf "\nResult value: %A\n" r;
              printf "Used: Elapsed %dms, CPU %dms" elapsed (int64 cpu)
            else ()
    let _ = Comp.compileToFile (opt_p,debug_p,verbose_p,p',fname)
    printf "\nCompiled to file %s\n" fname
  with 
    Failure eMsg -> printf "ParseTypeAndRun.compProg' ERROR: %s \n" eMsg

let compProg (program,fname) =
  compProg' (false,false,false,false,false,fromString program,fname)

let compFile (opt_p,debug_p,verbose_p,eval_p,alpha_p,pname,fname) =
  compProg' (opt_p,debug_p,verbose_p,eval_p,alpha_p,fromFile pname,fname)    
