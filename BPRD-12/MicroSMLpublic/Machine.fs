(* File MicroSML/Machine.fs 

   Instructions and code emission for a stack-based
   abstract machine supporting micro-SML including closures
   and exceptions.
   sestoft@itu.dk 2009-10-18
   nh@itu.dk 2015-12-07

   An implementation of the machine is found in file MicroSML/msmlmachine.c.
   
 *)

module Machine

type label = string

type instr =
  | Label of label                     (* symbolic label; pseudo-instruc. *)
  | CSTI of int                        (* constant                        *)
  | ADD                                (* addition                        *)
  | SUB                                (* subtraction                     *)
  | MUL                                (* multiplication                  *)
  | DIV                                (* division                        *)
  | MOD                                (* modulus                         *)
  | EQ                                 (* equality: s[sp-1] == s[sp]      *)
  | LT                                 (* less than: s[sp-1] < s[sp]      *)
  | NOT                                (* logical negation:  s[sp] != 0   *)
  | DUP                                (* duplicate stack top             *)
  | SWAP                               (* swap s[sp-1] and s[sp]          *)
  | LDI                                (* get s[s[sp]]                    *)
  | STI                                (* set s[s[sp-1]]                  *)
  | GETBP                              (* get bp                          *)
  | GETSP                              (* get sp                          *)
  | INCSP of int                       (* increase stack top by m         *)
  | GOTO of label                      (* go to label                     *)
  | IFZERO of label                    (* go to label if s[sp] == 0       *)
  | IFNZRO of label                    (* go to label if s[sp] != 0       *)
  | CALL of int * label                (* move m args up 2, push pc, bp and jump *)
  | CLOSCALL of int                    (* move m args up 2, push pc, bp and jump to addr in closure *)
  | TCLOSCALL of int                   (* move m args down to bp, and jump to addr in closure    *)
  | TCALL of int * label               (* move m args down to bp, jump    *)
  | RET of int                         (* pop m and return to s[sp]       *)
  | PRINTI                             (* print s[sp] as integer          *)
  | PRINTB                             (* print s[sp] as true/false       *)
  | PRINTC                             (* print s[sp] as character        *)
  | PRINTL                             (* print s[sp] as list             *)
  | LDARGS                             (* load command line args on stack *)
  | STOP                               (* halt the abstract machine       *)
  | NIL                                (* load nil on stack               *)
  | CONS                               (* create cons cell and load ref.  *)
  | CAR                                (* get first field of cons cell    *)
  | CDR                                (* get second field of cons cell   *)
  | SETCAR                             (* set first field of cons cell    *)
  | SETCDR                             (* set second field of cons cell   *)
  | PUSHLAB of label                   (* push label on stack             *)
  | ACLOS of int                       (* allocate a closure              *)
  | HEAPSTI of int                     (* STI operation on heap allocated object *)
  | HEAPLDI of int                     (* LDI operation on heap allocated object *)
  | THROW                              (* Search for exception handle and execute affiliated exception code *)
  | PUSHHDLR of label                  (* Push exception handler on stack.                                  *)
  | POPHDLR                            (* Pop exception handler                                             *)

(* Generate new distinct labels *)

let resetLabels, newLabelWName = 
  let lastlab = ref -1
  ((fun () -> lastlab := 0),
    (fun name -> (lastlab := 1 + !lastlab; (if name="" then "" else name+"_")+"L" + (!lastlab).ToString())))
let newLabel() = newLabelWName ""

(* Simple environment operations *)

type 'data env = (string * 'data) list

let rec lookup env x = 
  match env with 
  | []         -> failwith ("Machine.lookup: " + x + " not found")
  | (y, v)::yr -> if x=y then v else lookup yr x

(* An instruction list is emitted in two phases:
   * pass 1 builds an environment labenv mapping labels to addresses 
   * pass 2 emits the code to file, using the environment labenv to 
     resolve labels
 *)

(* These numeric instruction codes must agree with Machine.java: *)

let CODECSTI     = 0 
let CODEADD      = 1 
let CODESUB      = 2 
let CODEMUL      = 3 
let CODEDIV      = 4 
let CODEMOD      = 5 
let CODEEQ       = 6 
let CODELT       = 7 
let CODENOT      = 8 
let CODEDUP      = 9 
let CODESWAP     = 10 
let CODELDI      = 11 
let CODESTI      = 12 
let CODEGETBP    = 13 
let CODEGETSP    = 14 
let CODEINCSP    = 15 
let CODEGOTO     = 16
let CODEIFZERO   = 17
let CODEIFNZRO   = 18 
let CODECALL     = 19
let CODETCALL    = 20
let CODERET      = 21
let CODEPRINTI   = 22 
let CODEPRINTC   = 23
let CODELDARGS   = 24
let CODESTOP     = 25;
let CODENIL      = 26;
let CODECONS     = 27;
let CODECAR      = 28;
let CODECDR      = 29;
let CODESETCAR   = 30;
let CODESETCDR   = 31;
let CODEPUSHLAB  = 32;
let CODEHEAPSTI  = 33;
let CODEACLOS    = 34;
let CODECLOSCALL = 35;
let CODEHEAPLDI  = 36;
let CODEPRINTB   = 37;
let CODETCLOSCALL = 38;
let CODEPRINTL    = 39;
let CODETHROW     = 40;
let CODEPUSHHDLR  = 41;
let CODEPOPHDLR   = 42;

(* Bytecode emission, first pass: build environment that maps 
   each label to an integer address in the bytecode.
 *)

let sizeInst instr = 
  match instr with
  | Label lab      -> 0
  | CSTI i         -> 2
  | ADD            -> 1
  | SUB            -> 1
  | MUL            -> 1
  | DIV            -> 1
  | MOD            -> 1
  | EQ             -> 1
  | LT             -> 1
  | NOT            -> 1
  | DUP            -> 1
  | SWAP           -> 1
  | LDI            -> 1
  | STI            -> 1
  | GETBP          -> 1
  | GETSP          -> 1
  | INCSP m        -> 2
  | GOTO lab       -> 2
  | IFZERO lab     -> 2
  | IFNZRO lab     -> 2
  | CALL(m,lab)    -> 3
  | CLOSCALL m     -> 2
  | TCLOSCALL m    -> 2
  | TCALL(m,lab)   -> 3
  | RET m          -> 2
  | PRINTI         -> 1
  | PRINTB         -> 1
  | PRINTC         -> 1
  | PRINTL         -> 1    
  | LDARGS         -> 1
  | STOP           -> 1
  | NIL            -> 1
  | CONS           -> 1
  | CAR            -> 1
  | CDR            -> 1
  | SETCAR         -> 1
  | SETCDR         -> 1
  | PUSHLAB _      -> 2
  | ACLOS _        -> 2
  | HEAPSTI _      -> 2
  | HEAPLDI _      -> 2
  | THROW          -> 1
  | PUSHHDLR _     -> 2
  | POPHDLR        -> 1

let makelabenv (addr, labenv) instr =
  let size = sizeInst instr      
  match instr with
  | Label lab -> (addr, (lab, addr) :: labenv)
  | _         -> (addr+size, labenv)

(* Bytecode emission, second pass: output bytecode as integers *)
let emitints getlab instr ints = 
  match instr with
  | Label lab      -> ints
  | CSTI i         -> CODECSTI   :: i :: ints
  | ADD            -> CODEADD    :: ints
  | SUB            -> CODESUB    :: ints
  | MUL            -> CODEMUL    :: ints
  | DIV            -> CODEDIV    :: ints
  | MOD            -> CODEMOD    :: ints
  | EQ             -> CODEEQ     :: ints
  | LT             -> CODELT     :: ints
  | NOT            -> CODENOT    :: ints
  | DUP            -> CODEDUP    :: ints
  | SWAP           -> CODESWAP   :: ints
  | LDI            -> CODELDI    :: ints
  | STI            -> CODESTI    :: ints
  | GETBP          -> CODEGETBP  :: ints
  | GETSP          -> CODEGETSP  :: ints
  | INCSP m        -> CODEINCSP  :: m :: ints
  | GOTO lab       -> CODEGOTO   :: getlab lab :: ints
  | IFZERO lab     -> CODEIFZERO :: getlab lab :: ints
  | IFNZRO lab     -> CODEIFNZRO :: getlab lab :: ints
  | CALL(m,lab)    -> CODECALL   :: m :: getlab lab :: ints
  | CLOSCALL m     -> CODECLOSCALL :: m :: ints
  | TCLOSCALL m    -> CODETCLOSCALL :: m :: ints
  | TCALL(m,lab)   -> CODETCALL  :: m :: getlab lab :: ints
  | RET m          -> CODERET    :: m :: ints
  | PRINTI         -> CODEPRINTI :: ints
  | PRINTB         -> CODEPRINTB :: ints
  | PRINTC         -> CODEPRINTC :: ints
  | PRINTL         -> CODEPRINTL :: ints  
  | LDARGS         -> CODELDARGS :: ints
  | STOP           -> CODESTOP   :: ints
  | NIL            -> CODENIL    :: ints
  | CONS           -> CODECONS   :: ints
  | CAR            -> CODECAR    :: ints
  | CDR            -> CODECDR    :: ints
  | SETCAR         -> CODESETCAR :: ints
  | SETCDR         -> CODESETCDR :: ints
  | PUSHLAB lab    -> CODEPUSHLAB :: getlab lab :: ints
  | ACLOS n        -> CODEACLOS :: n :: ints
  | HEAPSTI n      -> CODEHEAPSTI :: n :: ints
  | HEAPLDI n      -> CODEHEAPLDI :: n :: ints
  | THROW          -> CODETHROW :: ints
  | PUSHHDLR lab   -> CODEPUSHHDLR :: getlab lab :: ints
  | POPHDLR        -> CODEPOPHDLR :: ints

let ppInst (addr,strs) instr =
  let indent s = (addr + sizeInst instr,"  " + (addr.ToString().PadLeft(4)) + ": " + s :: strs)
  match instr with
  | Label lab      -> (addr, "LABEL " + lab :: strs)
  | CSTI i         -> indent ("CSTI " + i.ToString())
  | ADD            -> indent "ADD"
  | SUB            -> indent "SUB"
  | MUL            -> indent "MUL"
  | DIV            -> indent "DIV"
  | MOD            -> indent "MOD"
  | EQ             -> indent "EQ" 
  | LT             -> indent "LT" 
  | NOT            -> indent "NOT"
  | DUP            -> indent "DUP"
  | SWAP           -> indent "SWAP"
  | LDI            -> indent "LDI" 
  | STI            -> indent "STI" 
  | GETBP          -> indent "GETBP"
  | GETSP          -> indent "GETSP" 
  | INCSP m        -> indent ("INCSP " + m.ToString())
  | GOTO lab       -> indent ("GOTO " + lab)
  | IFZERO lab     -> indent ("IFZERO " + lab)
  | IFNZRO lab     -> indent ("IFNZRO " + lab)
  | CALL(m,lab)    -> indent ("CALL " + m.ToString() + " " + lab)
  | CLOSCALL m     -> indent ("CLOSCALL " + m.ToString())
  | TCLOSCALL m    -> indent ("TCLOSCALL " + m.ToString())
  | TCALL(m,lab)   -> indent ("TCALL " + m.ToString() + " " + lab)
  | RET m          -> indent ("RET " + m.ToString())
  | PRINTI         -> indent "PRINTI"
  | PRINTB         -> indent "PRINTB"
  | PRINTC         -> indent "PRINTC"
  | PRINTL         -> indent "PRINTL"  
  | LDARGS         -> indent "LDARGS"
  | STOP           -> indent "STOP"  
  | NIL            -> indent "NIL"   
  | CONS           -> indent "CONS"  
  | CAR            -> indent "CAR"   
  | CDR            -> indent "CDR"   
  | SETCAR         -> indent "SETCAR"
  | SETCDR         -> indent "SETCDR"
  | PUSHLAB lab    -> indent ("PUSHLAB " + lab)
  | ACLOS n        -> indent ("ACLOS " + n.ToString()) 
  | HEAPSTI n      -> indent ("HEAPSTI " + n.ToString())
  | HEAPLDI n      -> indent ("HEAPLDI " + n.ToString())
  | THROW          -> indent "THROW"
  | PUSHHDLR lab   -> indent ("PUSHHDLR " + lab)
  | POPHDLR        -> indent "POPHDLR"

(* Convert instruction list to int list in two passes:
   Pass 1: build label environment
   Pass 2: output instructions using label environment
 *)

let code2ints (code : instr list) : int list =
  let (_, labenv) = List.fold makelabenv (0, []) code
  let getlab lab = lookup labenv lab
  List.foldBack (emitints getlab) code []

let ppInsts (code : instr list) : string =
  String.concat "\n" (List.rev (snd (List.fold ppInst (0,[]) code)))
