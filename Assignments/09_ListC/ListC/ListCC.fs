(* File ListC/ListCC.fs *)

let args = System.Environment.GetCommandLineArgs()

let _ = printf "List-C compiler v 1.0.0.0 of 2012-02-13\n";;

let _ = if args.Length > 1 then
           let source = args.[1]
           let stem = if source.EndsWith(".lc") then source.Substring(0,source.Length-3) 
                      else if source.EndsWith(".c") then source.Substring(0,source.Length-2) 
                           else source
           let target = stem + ".out"
           printf "Compiling %s to %s\n" source target;
           try ignore (Comp.compileToFile (Parse.fromFile source) target)
           with Failure msg -> printf "ERROR: %s\n" msg
        else
           printf "Usage: listcc <source file>\n";;
