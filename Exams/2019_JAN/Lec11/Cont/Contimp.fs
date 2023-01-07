(* File Cont/Contimp.fs

   Naive imperative language with loops and exceptions.
   Interpreted using continuations.
   sestoft@itu.dk 2009-09-24

   This file contains two interpreters:

     * coExec1 allows exceptions to be thrown, but not caught; this
       is implemented using a single continuation (which is used by
       ordinary computations but ignored when an exception is thrown)

     * coExec2 allows exceptions to be thrown and caught; this is
       implemented using two continuations: the success continuation
       is used by ordinary computations, and the error continuation is
       used when an exception is thrown.  
*)

module Contimp

(* A naive store is a map from names (strings) to values (ints) *)

type naivestore = Map<string,int>

let emptystore : Map<string,int> = Map.empty

let getSto (store : naivestore) x = store.Item x

let setSto (store : naivestore) (k, v) = store.Add(k, v)


(* A computation may terminate normally or throw an exception: *)

type answer =
  | Terminate 
  | Abort of string

type exn = 
  | Exn of string

type expr = 
  | CstI of int
  | Var of string
  | Prim of string * expr * expr

type stmt = 
  | Asgn of string * expr
  | If of expr * stmt * stmt
  | Block of stmt list
  | For of string * expr * expr * stmt
  | While of expr * stmt
  | Print of expr
  | Throw of exn
  | TryCatch of stmt * exn * stmt

(* Evaluation of expressions without side effects and exceptions *)

let rec eval e (store : naivestore) : int =
    match e with
    | CstI i -> i
    | Var x  -> getSto store x
    | Prim(ope, e1, e2) ->
      let i1 = eval e1 store
      let i2 = eval e2 store
      match ope with
      | "*"  -> i1 * i2
      | "+"  -> i1 + i2
      | "-"  -> i1 - i2
      | "==" -> if i1 = i2 then 1 else 0
      | "<"  -> if i1 < i2 then 1 else 0
      | _    -> failwith "unknown primitive"

(* This interpreter coExec1 takes the following arguments:

   * A statement stmt to execute.
   * A naive store mapping names to values.
   * A success continuation cont, for normal termination.  By
     discarding the continuation, it can terminate abnormally (when
     executing a Throw statement), but it cannot catch thrown
     exceptions (because it has no error continuation).  
*)

let rec coExec1 stmt store (cont : naivestore -> answer) : answer =
    match stmt with
    | Asgn(x, e) -> 
      cont (setSto store (x, eval e store))
    | If(e1, stmt1, stmt2) -> 
      if eval e1 store <> 0 then 
          coExec1 stmt1 store cont
      else
          coExec1 stmt2 store cont
    | Block stmts -> 
      let rec loop ss sto = 
              match ss with 
              | []     -> cont sto
              | s1::sr -> coExec1 s1 sto (fun sto -> loop sr sto) 
      loop stmts store
    | For(x, estart, estop, body) -> 
      let start = eval estart store
      let stop  = eval estop  store
      let rec loop i sto = 
              if i > stop then cont sto
              else coExec1 body (setSto sto (x, i)) (fun sto -> loop (i+1) sto)
      loop start store 
    | While(e, body) -> 
      let rec loop sto =
              if eval e sto = 0 then cont sto
              else coExec1 body sto loop
      loop store 
    | Print e -> 
      (printf "%d\n" (eval e store); cont store)
    | Throw (Exn s) -> 
      Abort ("Uncaught exception: " + s)
    | TryCatch _ -> 
      Abort "TryCatch is not implemented"

let run1 stmt : answer = 
    coExec1 stmt emptystore (fun _ -> Terminate)


(* This interpreter coExec2 takes the following arguments:

   * A statement stmt to execute.
   * A naive store mapping names to values.
   * A success continuation cont, for normal termination.  By
     discarding the continuation, it can terminate abnormally (when
     executing a Throw statement), but it cannot catch thrown
     exceptions (because it has no error continuation).  
   * An error continuation econt for abnormal termination.  The error
     continuation receives the exception and the store, and decides
     whether it wants to catch the exception or not.  In the former
     case it executes the handler's statement body; in the latter case
     it re-raises the exception, by applying the handler's own error
     continuation.  
*)

let rec coExec2 stmt (store : naivestore)
         (cont : naivestore -> answer) 
         (econt : exn * naivestore -> answer) : answer =
    match stmt with
    | Asgn(x, e) -> 
      cont (setSto store (x, eval e store))
    | If(e1, stmt1, stmt2) -> 
      if eval e1 store <> 0 then 
        coExec2 stmt1 store cont econt
      else
        coExec2 stmt2 store cont econt
    | Block stmts -> 
      let rec loop ss sto = 
              match ss with 
              | []     -> cont sto
              | s1::sr -> 
                coExec2 s1 sto (fun sto -> loop sr sto) econt
      loop stmts store 
    | For(x, estart, estop, stmt) -> 
      let start = eval estart store
      let stop  = eval estop  store
      let rec loop i sto = 
              if i > stop then cont sto
              else coExec2 stmt (setSto sto (x, i)) 
                                (fun sto -> loop (i+1) sto) 
                                econt
      loop start store 
    | While(e, stmt) -> 
      let rec loop sto =
              if eval e sto = 0 then cont sto
              else coExec2 stmt sto (fun sto -> loop sto) econt
      loop store 
    | Print e -> 
      (printf "%d\n" (eval e store); cont store)
    | Throw exn -> 
      econt(exn, store)
    | TryCatch(stmt1, exn, stmt2) ->
      let econt1 (exn1, sto1) =
          if exn1 = exn then coExec2 stmt2 sto1 cont econt
                        else econt (exn1, sto1)
      coExec2 stmt1 store cont econt1 

let run2 stmt : answer = 
    coExec2 stmt emptystore 
            (fun _ -> Terminate) 
            (fun (Exn s, _) -> Abort ("Uncaught exception: " + s))

(* Example programs *)

(* Abruptly terminating a for loop *)

let ex1 = 
    For("i", CstI 0, CstI 10,
        If(Prim("==", Var "i", CstI 7),
           Throw (Exn "seven"),
           Print (Var "i")));

(* Abruptly terminating a while loop *)

let ex2 = 
    Block[Asgn("i", CstI 0);
          While (CstI 1,
                 Block[Asgn("i", Prim("+", Var "i", CstI 1));
                       Print (Var "i");
                       If(Prim("==", Var "i", CstI 7),
                          Throw (Exn "seven"),
                          Block [])]);
          Print (CstI 333333)];

(* Abruptly terminating a while loop, and handling the exception *)

let ex3 = 
    Block[Asgn("i", CstI 0);
          TryCatch(Block[While (CstI 1,
                                Block[Asgn("i", Prim("+", Var "i", CstI 1));
                                      Print (Var "i");
                                      If(Prim("==", Var "i", CstI 7),
                                         Throw (Exn "seven"),
                                         Block [])]);
                         Print (CstI 111111)],
                   Exn "seven",
                   Print (CstI 222222));
          Print (CstI 333333)];

