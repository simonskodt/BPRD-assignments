// Implementation file for parser generated by fsyacc
module FunPar
#nowarn "64";; // turn off warnings that type variables used in production annotations are instantiated to concrete type
open FSharp.Text.Lexing
open FSharp.Text.Parsing.ParseHelpers
# 1 "FunPar.fsy"

 (* File Fun/FunPar.fsy 
    Parser for micro-ML, a small functional language; one-argument functions.
    sestoft@itu.dk * 2009-10-19
  *)

 open Absyn;

# 15 "FunPar.fs"
// This type is the type of tokens accepted by the parser
type token = 
  | EOF
  | LPAR
  | RPAR
  | EQ
  | NE
  | GT
  | LT
  | GE
  | LE
  | PLUS
  | MINUS
  | TIMES
  | DIV
  | MOD
  | DPLUS
  | LBRA
  | RBRA
  | COMMA
  | ELSE
  | END
  | FALSE
  | IF
  | IN
  | LET
  | NOT
  | THEN
  | TRUE
  | CSTBOOL of (bool)
  | NAME of (string)
  | CSTINT of (int)
// This type is used to give symbolic names to token indexes, useful for error messages
type tokenId = 
    | TOKEN_EOF
    | TOKEN_LPAR
    | TOKEN_RPAR
    | TOKEN_EQ
    | TOKEN_NE
    | TOKEN_GT
    | TOKEN_LT
    | TOKEN_GE
    | TOKEN_LE
    | TOKEN_PLUS
    | TOKEN_MINUS
    | TOKEN_TIMES
    | TOKEN_DIV
    | TOKEN_MOD
    | TOKEN_DPLUS
    | TOKEN_LBRA
    | TOKEN_RBRA
    | TOKEN_COMMA
    | TOKEN_ELSE
    | TOKEN_END
    | TOKEN_FALSE
    | TOKEN_IF
    | TOKEN_IN
    | TOKEN_LET
    | TOKEN_NOT
    | TOKEN_THEN
    | TOKEN_TRUE
    | TOKEN_CSTBOOL
    | TOKEN_NAME
    | TOKEN_CSTINT
    | TOKEN_end_of_input
    | TOKEN_error
// This type is used to give symbolic names to token indexes, useful for error messages
type nonTerminalId = 
    | NONTERM__startMain
    | NONTERM_Main
    | NONTERM_Expr
    | NONTERM_SetExpr
    | NONTERM_AtExpr
    | NONTERM_AppExpr
    | NONTERM_Const

// This function maps tokens to integer indexes
let tagOfToken (t:token) = 
  match t with
  | EOF  -> 0 
  | LPAR  -> 1 
  | RPAR  -> 2 
  | EQ  -> 3 
  | NE  -> 4 
  | GT  -> 5 
  | LT  -> 6 
  | GE  -> 7 
  | LE  -> 8 
  | PLUS  -> 9 
  | MINUS  -> 10 
  | TIMES  -> 11 
  | DIV  -> 12 
  | MOD  -> 13 
  | DPLUS  -> 14 
  | LBRA  -> 15 
  | RBRA  -> 16 
  | COMMA  -> 17 
  | ELSE  -> 18 
  | END  -> 19 
  | FALSE  -> 20 
  | IF  -> 21 
  | IN  -> 22 
  | LET  -> 23 
  | NOT  -> 24 
  | THEN  -> 25 
  | TRUE  -> 26 
  | CSTBOOL _ -> 27 
  | NAME _ -> 28 
  | CSTINT _ -> 29 

// This function maps integer indexes to symbolic token ids
let tokenTagToTokenId (tokenIdx:int) = 
  match tokenIdx with
  | 0 -> TOKEN_EOF 
  | 1 -> TOKEN_LPAR 
  | 2 -> TOKEN_RPAR 
  | 3 -> TOKEN_EQ 
  | 4 -> TOKEN_NE 
  | 5 -> TOKEN_GT 
  | 6 -> TOKEN_LT 
  | 7 -> TOKEN_GE 
  | 8 -> TOKEN_LE 
  | 9 -> TOKEN_PLUS 
  | 10 -> TOKEN_MINUS 
  | 11 -> TOKEN_TIMES 
  | 12 -> TOKEN_DIV 
  | 13 -> TOKEN_MOD 
  | 14 -> TOKEN_DPLUS 
  | 15 -> TOKEN_LBRA 
  | 16 -> TOKEN_RBRA 
  | 17 -> TOKEN_COMMA 
  | 18 -> TOKEN_ELSE 
  | 19 -> TOKEN_END 
  | 20 -> TOKEN_FALSE 
  | 21 -> TOKEN_IF 
  | 22 -> TOKEN_IN 
  | 23 -> TOKEN_LET 
  | 24 -> TOKEN_NOT 
  | 25 -> TOKEN_THEN 
  | 26 -> TOKEN_TRUE 
  | 27 -> TOKEN_CSTBOOL 
  | 28 -> TOKEN_NAME 
  | 29 -> TOKEN_CSTINT 
  | 32 -> TOKEN_end_of_input
  | 30 -> TOKEN_error
  | _ -> failwith "tokenTagToTokenId: bad token"

/// This function maps production indexes returned in syntax errors to strings representing the non terminal that would be produced by that production
let prodIdxToNonTerminal (prodIdx:int) = 
  match prodIdx with
    | 0 -> NONTERM__startMain 
    | 1 -> NONTERM_Main 
    | 2 -> NONTERM_Expr 
    | 3 -> NONTERM_Expr 
    | 4 -> NONTERM_Expr 
    | 5 -> NONTERM_Expr 
    | 6 -> NONTERM_Expr 
    | 7 -> NONTERM_Expr 
    | 8 -> NONTERM_Expr 
    | 9 -> NONTERM_Expr 
    | 10 -> NONTERM_Expr 
    | 11 -> NONTERM_Expr 
    | 12 -> NONTERM_Expr 
    | 13 -> NONTERM_Expr 
    | 14 -> NONTERM_Expr 
    | 15 -> NONTERM_Expr 
    | 16 -> NONTERM_Expr 
    | 17 -> NONTERM_Expr 
    | 18 -> NONTERM_Expr 
    | 19 -> NONTERM_SetExpr 
    | 20 -> NONTERM_SetExpr 
    | 21 -> NONTERM_AtExpr 
    | 22 -> NONTERM_AtExpr 
    | 23 -> NONTERM_AtExpr 
    | 24 -> NONTERM_AtExpr 
    | 25 -> NONTERM_AtExpr 
    | 26 -> NONTERM_AppExpr 
    | 27 -> NONTERM_AppExpr 
    | 28 -> NONTERM_Const 
    | 29 -> NONTERM_Const 
    | _ -> failwith "prodIdxToNonTerminal: bad production index"

let _fsyacc_endOfInputTag = 32 
let _fsyacc_tagOfErrorTerminal = 30

// This function gets the name of a token as a string
let token_to_string (t:token) = 
  match t with 
  | EOF  -> "EOF" 
  | LPAR  -> "LPAR" 
  | RPAR  -> "RPAR" 
  | EQ  -> "EQ" 
  | NE  -> "NE" 
  | GT  -> "GT" 
  | LT  -> "LT" 
  | GE  -> "GE" 
  | LE  -> "LE" 
  | PLUS  -> "PLUS" 
  | MINUS  -> "MINUS" 
  | TIMES  -> "TIMES" 
  | DIV  -> "DIV" 
  | MOD  -> "MOD" 
  | DPLUS  -> "DPLUS" 
  | LBRA  -> "LBRA" 
  | RBRA  -> "RBRA" 
  | COMMA  -> "COMMA" 
  | ELSE  -> "ELSE" 
  | END  -> "END" 
  | FALSE  -> "FALSE" 
  | IF  -> "IF" 
  | IN  -> "IN" 
  | LET  -> "LET" 
  | NOT  -> "NOT" 
  | THEN  -> "THEN" 
  | TRUE  -> "TRUE" 
  | CSTBOOL _ -> "CSTBOOL" 
  | NAME _ -> "NAME" 
  | CSTINT _ -> "CSTINT" 

// This function gets the data carried by a token as an object
let _fsyacc_dataOfToken (t:token) = 
  match t with 
  | EOF  -> (null : System.Object) 
  | LPAR  -> (null : System.Object) 
  | RPAR  -> (null : System.Object) 
  | EQ  -> (null : System.Object) 
  | NE  -> (null : System.Object) 
  | GT  -> (null : System.Object) 
  | LT  -> (null : System.Object) 
  | GE  -> (null : System.Object) 
  | LE  -> (null : System.Object) 
  | PLUS  -> (null : System.Object) 
  | MINUS  -> (null : System.Object) 
  | TIMES  -> (null : System.Object) 
  | DIV  -> (null : System.Object) 
  | MOD  -> (null : System.Object) 
  | DPLUS  -> (null : System.Object) 
  | LBRA  -> (null : System.Object) 
  | RBRA  -> (null : System.Object) 
  | COMMA  -> (null : System.Object) 
  | ELSE  -> (null : System.Object) 
  | END  -> (null : System.Object) 
  | FALSE  -> (null : System.Object) 
  | IF  -> (null : System.Object) 
  | IN  -> (null : System.Object) 
  | LET  -> (null : System.Object) 
  | NOT  -> (null : System.Object) 
  | THEN  -> (null : System.Object) 
  | TRUE  -> (null : System.Object) 
  | CSTBOOL _fsyacc_x -> Microsoft.FSharp.Core.Operators.box _fsyacc_x 
  | NAME _fsyacc_x -> Microsoft.FSharp.Core.Operators.box _fsyacc_x 
  | CSTINT _fsyacc_x -> Microsoft.FSharp.Core.Operators.box _fsyacc_x 
let _fsyacc_gotos = [| 0us; 65535us; 1us; 65535us; 0us; 1us; 24us; 65535us; 0us; 2us; 6us; 7us; 8us; 9us; 10us; 11us; 12us; 13us; 32us; 14us; 33us; 15us; 34us; 16us; 35us; 17us; 36us; 18us; 37us; 19us; 38us; 20us; 39us; 21us; 40us; 22us; 41us; 23us; 42us; 24us; 43us; 25us; 47us; 25us; 50us; 26us; 55us; 27us; 56us; 28us; 59us; 29us; 60us; 30us; 62us; 31us; 2us; 65535us; 43us; 44us; 47us; 48us; 26us; 65535us; 0us; 4us; 4us; 64us; 5us; 65us; 6us; 4us; 8us; 4us; 10us; 4us; 12us; 4us; 32us; 4us; 33us; 4us; 34us; 4us; 35us; 4us; 36us; 4us; 37us; 4us; 38us; 4us; 39us; 4us; 40us; 4us; 41us; 4us; 42us; 4us; 43us; 4us; 47us; 4us; 50us; 4us; 55us; 4us; 56us; 4us; 59us; 4us; 60us; 4us; 62us; 4us; 24us; 65535us; 0us; 5us; 6us; 5us; 8us; 5us; 10us; 5us; 12us; 5us; 32us; 5us; 33us; 5us; 34us; 5us; 35us; 5us; 36us; 5us; 37us; 5us; 38us; 5us; 39us; 5us; 40us; 5us; 41us; 5us; 42us; 5us; 43us; 5us; 47us; 5us; 50us; 5us; 55us; 5us; 56us; 5us; 59us; 5us; 60us; 5us; 62us; 5us; 26us; 65535us; 0us; 51us; 4us; 51us; 5us; 51us; 6us; 51us; 8us; 51us; 10us; 51us; 12us; 51us; 32us; 51us; 33us; 51us; 34us; 51us; 35us; 51us; 36us; 51us; 37us; 51us; 38us; 51us; 39us; 51us; 40us; 51us; 41us; 51us; 42us; 51us; 43us; 51us; 47us; 51us; 50us; 51us; 55us; 51us; 56us; 51us; 59us; 51us; 60us; 51us; 62us; 51us; |]
let _fsyacc_sparseGotoTableRowOffsets = [|0us; 1us; 3us; 28us; 31us; 58us; 83us; |]
let _fsyacc_stateToProdIdxsTableElements = [| 1us; 0us; 1us; 0us; 12us; 1us; 6us; 7us; 8us; 9us; 10us; 11us; 12us; 13us; 14us; 15us; 16us; 1us; 1us; 2us; 2us; 26us; 2us; 3us; 27us; 1us; 4us; 12us; 4us; 6us; 7us; 8us; 9us; 10us; 11us; 12us; 13us; 14us; 15us; 16us; 1us; 4us; 12us; 4us; 6us; 7us; 8us; 9us; 10us; 11us; 12us; 13us; 14us; 15us; 16us; 1us; 4us; 12us; 4us; 6us; 7us; 8us; 9us; 10us; 11us; 12us; 13us; 14us; 15us; 16us; 1us; 5us; 12us; 5us; 6us; 7us; 8us; 9us; 10us; 11us; 12us; 13us; 14us; 15us; 16us; 12us; 6us; 6us; 7us; 8us; 9us; 10us; 11us; 12us; 13us; 14us; 15us; 16us; 12us; 6us; 7us; 7us; 8us; 9us; 10us; 11us; 12us; 13us; 14us; 15us; 16us; 12us; 6us; 7us; 8us; 8us; 9us; 10us; 11us; 12us; 13us; 14us; 15us; 16us; 12us; 6us; 7us; 8us; 9us; 9us; 10us; 11us; 12us; 13us; 14us; 15us; 16us; 12us; 6us; 7us; 8us; 9us; 10us; 10us; 11us; 12us; 13us; 14us; 15us; 16us; 12us; 6us; 7us; 8us; 9us; 10us; 11us; 11us; 12us; 13us; 14us; 15us; 16us; 12us; 6us; 7us; 8us; 9us; 10us; 11us; 12us; 12us; 13us; 14us; 15us; 16us; 12us; 6us; 7us; 8us; 9us; 10us; 11us; 12us; 13us; 13us; 14us; 15us; 16us; 12us; 6us; 7us; 8us; 9us; 10us; 11us; 12us; 13us; 14us; 14us; 15us; 16us; 12us; 6us; 7us; 8us; 9us; 10us; 11us; 12us; 13us; 14us; 15us; 15us; 16us; 12us; 6us; 7us; 8us; 9us; 10us; 11us; 12us; 13us; 14us; 15us; 16us; 16us; 12us; 6us; 7us; 8us; 9us; 10us; 11us; 12us; 13us; 14us; 15us; 16us; 19us; 12us; 6us; 7us; 8us; 9us; 10us; 11us; 12us; 13us; 14us; 15us; 16us; 20us; 12us; 6us; 7us; 8us; 9us; 10us; 11us; 12us; 13us; 14us; 15us; 16us; 23us; 12us; 6us; 7us; 8us; 9us; 10us; 11us; 12us; 13us; 14us; 15us; 16us; 23us; 12us; 6us; 7us; 8us; 9us; 10us; 11us; 12us; 13us; 14us; 15us; 16us; 24us; 12us; 6us; 7us; 8us; 9us; 10us; 11us; 12us; 13us; 14us; 15us; 16us; 24us; 12us; 6us; 7us; 8us; 9us; 10us; 11us; 12us; 13us; 14us; 15us; 16us; 25us; 1us; 6us; 1us; 7us; 1us; 8us; 1us; 9us; 1us; 10us; 1us; 11us; 1us; 12us; 1us; 13us; 1us; 14us; 1us; 15us; 1us; 16us; 2us; 17us; 18us; 3us; 17us; 18us; 20us; 2us; 17us; 18us; 1us; 17us; 1us; 17us; 2us; 17us; 20us; 1us; 17us; 1us; 20us; 1us; 21us; 1us; 22us; 2us; 23us; 24us; 2us; 23us; 24us; 1us; 23us; 1us; 23us; 1us; 23us; 1us; 24us; 1us; 24us; 1us; 24us; 1us; 24us; 1us; 25us; 1us; 25us; 1us; 26us; 1us; 27us; 1us; 28us; 1us; 29us; |]
let _fsyacc_stateToProdIdxsTableRowOffsets = [|0us; 2us; 4us; 17us; 19us; 22us; 25us; 27us; 40us; 42us; 55us; 57us; 70us; 72us; 85us; 98us; 111us; 124us; 137us; 150us; 163us; 176us; 189us; 202us; 215us; 228us; 241us; 254us; 267us; 280us; 293us; 306us; 319us; 321us; 323us; 325us; 327us; 329us; 331us; 333us; 335us; 337us; 339us; 341us; 344us; 348us; 351us; 353us; 355us; 358us; 360us; 362us; 364us; 366us; 369us; 372us; 374us; 376us; 378us; 380us; 382us; 384us; 386us; 388us; 390us; 392us; 394us; 396us; |]
let _fsyacc_action_rows = 68
let _fsyacc_actionTableElements = [|8us; 32768us; 1us; 62us; 10us; 12us; 15us; 43us; 21us; 6us; 23us; 53us; 27us; 67us; 28us; 52us; 29us; 66us; 0us; 49152us; 12us; 32768us; 0us; 3us; 3us; 37us; 4us; 38us; 5us; 39us; 6us; 40us; 7us; 41us; 8us; 42us; 9us; 32us; 10us; 33us; 11us; 34us; 12us; 35us; 13us; 36us; 0us; 16385us; 5us; 16386us; 1us; 62us; 23us; 53us; 27us; 67us; 28us; 52us; 29us; 66us; 5us; 16387us; 1us; 62us; 23us; 53us; 27us; 67us; 28us; 52us; 29us; 66us; 8us; 32768us; 1us; 62us; 10us; 12us; 15us; 43us; 21us; 6us; 23us; 53us; 27us; 67us; 28us; 52us; 29us; 66us; 12us; 32768us; 3us; 37us; 4us; 38us; 5us; 39us; 6us; 40us; 7us; 41us; 8us; 42us; 9us; 32us; 10us; 33us; 11us; 34us; 12us; 35us; 13us; 36us; 25us; 8us; 8us; 32768us; 1us; 62us; 10us; 12us; 15us; 43us; 21us; 6us; 23us; 53us; 27us; 67us; 28us; 52us; 29us; 66us; 12us; 32768us; 3us; 37us; 4us; 38us; 5us; 39us; 6us; 40us; 7us; 41us; 8us; 42us; 9us; 32us; 10us; 33us; 11us; 34us; 12us; 35us; 13us; 36us; 18us; 10us; 8us; 32768us; 1us; 62us; 10us; 12us; 15us; 43us; 21us; 6us; 23us; 53us; 27us; 67us; 28us; 52us; 29us; 66us; 11us; 16388us; 3us; 37us; 4us; 38us; 5us; 39us; 6us; 40us; 7us; 41us; 8us; 42us; 9us; 32us; 10us; 33us; 11us; 34us; 12us; 35us; 13us; 36us; 8us; 32768us; 1us; 62us; 10us; 12us; 15us; 43us; 21us; 6us; 23us; 53us; 27us; 67us; 28us; 52us; 29us; 66us; 3us; 16389us; 11us; 34us; 12us; 35us; 13us; 36us; 3us; 16390us; 11us; 34us; 12us; 35us; 13us; 36us; 3us; 16391us; 11us; 34us; 12us; 35us; 13us; 36us; 0us; 16392us; 0us; 16393us; 0us; 16394us; 9us; 16395us; 5us; 39us; 6us; 40us; 7us; 41us; 8us; 42us; 9us; 32us; 10us; 33us; 11us; 34us; 12us; 35us; 13us; 36us; 9us; 16396us; 5us; 39us; 6us; 40us; 7us; 41us; 8us; 42us; 9us; 32us; 10us; 33us; 11us; 34us; 12us; 35us; 13us; 36us; 5us; 16397us; 9us; 32us; 10us; 33us; 11us; 34us; 12us; 35us; 13us; 36us; 5us; 16398us; 9us; 32us; 10us; 33us; 11us; 34us; 12us; 35us; 13us; 36us; 5us; 16399us; 9us; 32us; 10us; 33us; 11us; 34us; 12us; 35us; 13us; 36us; 5us; 16400us; 9us; 32us; 10us; 33us; 11us; 34us; 12us; 35us; 13us; 36us; 11us; 16403us; 3us; 37us; 4us; 38us; 5us; 39us; 6us; 40us; 7us; 41us; 8us; 42us; 9us; 32us; 10us; 33us; 11us; 34us; 12us; 35us; 13us; 36us; 11us; 16404us; 3us; 37us; 4us; 38us; 5us; 39us; 6us; 40us; 7us; 41us; 8us; 42us; 9us; 32us; 10us; 33us; 11us; 34us; 12us; 35us; 13us; 36us; 12us; 32768us; 3us; 37us; 4us; 38us; 5us; 39us; 6us; 40us; 7us; 41us; 8us; 42us; 9us; 32us; 10us; 33us; 11us; 34us; 12us; 35us; 13us; 36us; 22us; 56us; 12us; 32768us; 3us; 37us; 4us; 38us; 5us; 39us; 6us; 40us; 7us; 41us; 8us; 42us; 9us; 32us; 10us; 33us; 11us; 34us; 12us; 35us; 13us; 36us; 19us; 57us; 12us; 32768us; 3us; 37us; 4us; 38us; 5us; 39us; 6us; 40us; 7us; 41us; 8us; 42us; 9us; 32us; 10us; 33us; 11us; 34us; 12us; 35us; 13us; 36us; 22us; 60us; 12us; 32768us; 3us; 37us; 4us; 38us; 5us; 39us; 6us; 40us; 7us; 41us; 8us; 42us; 9us; 32us; 10us; 33us; 11us; 34us; 12us; 35us; 13us; 36us; 19us; 61us; 12us; 32768us; 2us; 63us; 3us; 37us; 4us; 38us; 5us; 39us; 6us; 40us; 7us; 41us; 8us; 42us; 9us; 32us; 10us; 33us; 11us; 34us; 12us; 35us; 13us; 36us; 8us; 32768us; 1us; 62us; 10us; 12us; 15us; 43us; 21us; 6us; 23us; 53us; 27us; 67us; 28us; 52us; 29us; 66us; 8us; 32768us; 1us; 62us; 10us; 12us; 15us; 43us; 21us; 6us; 23us; 53us; 27us; 67us; 28us; 52us; 29us; 66us; 8us; 32768us; 1us; 62us; 10us; 12us; 15us; 43us; 21us; 6us; 23us; 53us; 27us; 67us; 28us; 52us; 29us; 66us; 8us; 32768us; 1us; 62us; 10us; 12us; 15us; 43us; 21us; 6us; 23us; 53us; 27us; 67us; 28us; 52us; 29us; 66us; 8us; 32768us; 1us; 62us; 10us; 12us; 15us; 43us; 21us; 6us; 23us; 53us; 27us; 67us; 28us; 52us; 29us; 66us; 8us; 32768us; 1us; 62us; 10us; 12us; 15us; 43us; 21us; 6us; 23us; 53us; 27us; 67us; 28us; 52us; 29us; 66us; 8us; 32768us; 1us; 62us; 10us; 12us; 15us; 43us; 21us; 6us; 23us; 53us; 27us; 67us; 28us; 52us; 29us; 66us; 8us; 32768us; 1us; 62us; 10us; 12us; 15us; 43us; 21us; 6us; 23us; 53us; 27us; 67us; 28us; 52us; 29us; 66us; 8us; 32768us; 1us; 62us; 10us; 12us; 15us; 43us; 21us; 6us; 23us; 53us; 27us; 67us; 28us; 52us; 29us; 66us; 8us; 32768us; 1us; 62us; 10us; 12us; 15us; 43us; 21us; 6us; 23us; 53us; 27us; 67us; 28us; 52us; 29us; 66us; 8us; 32768us; 1us; 62us; 10us; 12us; 15us; 43us; 21us; 6us; 23us; 53us; 27us; 67us; 28us; 52us; 29us; 66us; 8us; 32768us; 1us; 62us; 10us; 12us; 15us; 43us; 21us; 6us; 23us; 53us; 27us; 67us; 28us; 52us; 29us; 66us; 2us; 32768us; 16us; 45us; 17us; 50us; 1us; 16402us; 14us; 46us; 1us; 32768us; 15us; 47us; 8us; 32768us; 1us; 62us; 10us; 12us; 15us; 43us; 21us; 6us; 23us; 53us; 27us; 67us; 28us; 52us; 29us; 66us; 2us; 32768us; 16us; 49us; 17us; 50us; 0us; 16401us; 8us; 32768us; 1us; 62us; 10us; 12us; 15us; 43us; 21us; 6us; 23us; 53us; 27us; 67us; 28us; 52us; 29us; 66us; 0us; 16405us; 0us; 16406us; 1us; 32768us; 28us; 54us; 2us; 32768us; 3us; 55us; 28us; 58us; 8us; 32768us; 1us; 62us; 10us; 12us; 15us; 43us; 21us; 6us; 23us; 53us; 27us; 67us; 28us; 52us; 29us; 66us; 8us; 32768us; 1us; 62us; 10us; 12us; 15us; 43us; 21us; 6us; 23us; 53us; 27us; 67us; 28us; 52us; 29us; 66us; 0us; 16407us; 1us; 32768us; 3us; 59us; 8us; 32768us; 1us; 62us; 10us; 12us; 15us; 43us; 21us; 6us; 23us; 53us; 27us; 67us; 28us; 52us; 29us; 66us; 8us; 32768us; 1us; 62us; 10us; 12us; 15us; 43us; 21us; 6us; 23us; 53us; 27us; 67us; 28us; 52us; 29us; 66us; 0us; 16408us; 8us; 32768us; 1us; 62us; 10us; 12us; 15us; 43us; 21us; 6us; 23us; 53us; 27us; 67us; 28us; 52us; 29us; 66us; 0us; 16409us; 0us; 16410us; 0us; 16411us; 0us; 16412us; 0us; 16413us; |]
let _fsyacc_actionTableRowOffsets = [|0us; 9us; 10us; 23us; 24us; 30us; 36us; 45us; 58us; 67us; 80us; 89us; 101us; 110us; 114us; 118us; 122us; 123us; 124us; 125us; 135us; 145us; 151us; 157us; 163us; 169us; 181us; 193us; 206us; 219us; 232us; 245us; 258us; 267us; 276us; 285us; 294us; 303us; 312us; 321us; 330us; 339us; 348us; 357us; 366us; 369us; 371us; 373us; 382us; 385us; 386us; 395us; 396us; 397us; 399us; 402us; 411us; 420us; 421us; 423us; 432us; 441us; 442us; 451us; 452us; 453us; 454us; 455us; |]
let _fsyacc_reductionSymbolCounts = [|1us; 2us; 1us; 1us; 6us; 2us; 3us; 3us; 3us; 3us; 3us; 3us; 3us; 3us; 3us; 3us; 3us; 7us; 3us; 1us; 3us; 1us; 1us; 7us; 8us; 3us; 2us; 2us; 1us; 1us; |]
let _fsyacc_productionToNonTerminalTable = [|0us; 1us; 2us; 2us; 2us; 2us; 2us; 2us; 2us; 2us; 2us; 2us; 2us; 2us; 2us; 2us; 2us; 2us; 2us; 3us; 3us; 4us; 4us; 4us; 4us; 4us; 5us; 5us; 6us; 6us; |]
let _fsyacc_immediateActions = [|65535us; 49152us; 65535us; 16385us; 65535us; 65535us; 65535us; 65535us; 65535us; 65535us; 65535us; 65535us; 65535us; 65535us; 65535us; 65535us; 65535us; 65535us; 65535us; 65535us; 65535us; 65535us; 65535us; 65535us; 65535us; 65535us; 65535us; 65535us; 65535us; 65535us; 65535us; 65535us; 65535us; 65535us; 65535us; 65535us; 65535us; 65535us; 65535us; 65535us; 65535us; 65535us; 65535us; 65535us; 65535us; 65535us; 65535us; 65535us; 65535us; 16401us; 65535us; 16405us; 16406us; 65535us; 65535us; 65535us; 65535us; 16407us; 65535us; 65535us; 65535us; 16408us; 65535us; 16409us; 16410us; 16411us; 16412us; 16413us; |]
let _fsyacc_reductions ()  =    [| 
# 279 "FunPar.fs"
        (fun (parseState : FSharp.Text.Parsing.IParseState) ->
            let _1 = (let data = parseState.GetInput(1) in (Microsoft.FSharp.Core.Operators.unbox data : Absyn.expr)) in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
                      raise (FSharp.Text.Parsing.Accept(Microsoft.FSharp.Core.Operators.box _1))
                   )
                 : '_startMain));
# 288 "FunPar.fs"
        (fun (parseState : FSharp.Text.Parsing.IParseState) ->
            let _1 = (let data = parseState.GetInput(1) in (Microsoft.FSharp.Core.Operators.unbox data : Absyn.expr)) in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
# 34 "FunPar.fsy"
                                                               _1 
                   )
# 34 "FunPar.fsy"
                 : Absyn.expr));
# 299 "FunPar.fs"
        (fun (parseState : FSharp.Text.Parsing.IParseState) ->
            let _1 = (let data = parseState.GetInput(1) in (Microsoft.FSharp.Core.Operators.unbox data : Absyn.expr)) in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
# 38 "FunPar.fsy"
                                                               _1                     
                   )
# 38 "FunPar.fsy"
                 : Absyn.expr));
# 310 "FunPar.fs"
        (fun (parseState : FSharp.Text.Parsing.IParseState) ->
            let _1 = (let data = parseState.GetInput(1) in (Microsoft.FSharp.Core.Operators.unbox data : Absyn.expr)) in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
# 39 "FunPar.fsy"
                                                               _1                     
                   )
# 39 "FunPar.fsy"
                 : Absyn.expr));
# 321 "FunPar.fs"
        (fun (parseState : FSharp.Text.Parsing.IParseState) ->
            let _2 = (let data = parseState.GetInput(2) in (Microsoft.FSharp.Core.Operators.unbox data : Absyn.expr)) in
            let _4 = (let data = parseState.GetInput(4) in (Microsoft.FSharp.Core.Operators.unbox data : Absyn.expr)) in
            let _6 = (let data = parseState.GetInput(6) in (Microsoft.FSharp.Core.Operators.unbox data : Absyn.expr)) in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
# 40 "FunPar.fsy"
                                                               If(_2, _4, _6)         
                   )
# 40 "FunPar.fsy"
                 : Absyn.expr));
# 334 "FunPar.fs"
        (fun (parseState : FSharp.Text.Parsing.IParseState) ->
            let _2 = (let data = parseState.GetInput(2) in (Microsoft.FSharp.Core.Operators.unbox data : Absyn.expr)) in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
# 41 "FunPar.fsy"
                                                               Prim("-", CstI 0, _2)  
                   )
# 41 "FunPar.fsy"
                 : Absyn.expr));
# 345 "FunPar.fs"
        (fun (parseState : FSharp.Text.Parsing.IParseState) ->
            let _1 = (let data = parseState.GetInput(1) in (Microsoft.FSharp.Core.Operators.unbox data : Absyn.expr)) in
            let _3 = (let data = parseState.GetInput(3) in (Microsoft.FSharp.Core.Operators.unbox data : Absyn.expr)) in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
# 42 "FunPar.fsy"
                                                               Prim("+",  _1, _3)     
                   )
# 42 "FunPar.fsy"
                 : Absyn.expr));
# 357 "FunPar.fs"
        (fun (parseState : FSharp.Text.Parsing.IParseState) ->
            let _1 = (let data = parseState.GetInput(1) in (Microsoft.FSharp.Core.Operators.unbox data : Absyn.expr)) in
            let _3 = (let data = parseState.GetInput(3) in (Microsoft.FSharp.Core.Operators.unbox data : Absyn.expr)) in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
# 43 "FunPar.fsy"
                                                               Prim("-",  _1, _3)     
                   )
# 43 "FunPar.fsy"
                 : Absyn.expr));
# 369 "FunPar.fs"
        (fun (parseState : FSharp.Text.Parsing.IParseState) ->
            let _1 = (let data = parseState.GetInput(1) in (Microsoft.FSharp.Core.Operators.unbox data : Absyn.expr)) in
            let _3 = (let data = parseState.GetInput(3) in (Microsoft.FSharp.Core.Operators.unbox data : Absyn.expr)) in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
# 44 "FunPar.fsy"
                                                               Prim("*",  _1, _3)     
                   )
# 44 "FunPar.fsy"
                 : Absyn.expr));
# 381 "FunPar.fs"
        (fun (parseState : FSharp.Text.Parsing.IParseState) ->
            let _1 = (let data = parseState.GetInput(1) in (Microsoft.FSharp.Core.Operators.unbox data : Absyn.expr)) in
            let _3 = (let data = parseState.GetInput(3) in (Microsoft.FSharp.Core.Operators.unbox data : Absyn.expr)) in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
# 45 "FunPar.fsy"
                                                               Prim("/",  _1, _3)     
                   )
# 45 "FunPar.fsy"
                 : Absyn.expr));
# 393 "FunPar.fs"
        (fun (parseState : FSharp.Text.Parsing.IParseState) ->
            let _1 = (let data = parseState.GetInput(1) in (Microsoft.FSharp.Core.Operators.unbox data : Absyn.expr)) in
            let _3 = (let data = parseState.GetInput(3) in (Microsoft.FSharp.Core.Operators.unbox data : Absyn.expr)) in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
# 46 "FunPar.fsy"
                                                               Prim("%",  _1, _3)     
                   )
# 46 "FunPar.fsy"
                 : Absyn.expr));
# 405 "FunPar.fs"
        (fun (parseState : FSharp.Text.Parsing.IParseState) ->
            let _1 = (let data = parseState.GetInput(1) in (Microsoft.FSharp.Core.Operators.unbox data : Absyn.expr)) in
            let _3 = (let data = parseState.GetInput(3) in (Microsoft.FSharp.Core.Operators.unbox data : Absyn.expr)) in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
# 47 "FunPar.fsy"
                                                               Prim("=",  _1, _3)     
                   )
# 47 "FunPar.fsy"
                 : Absyn.expr));
# 417 "FunPar.fs"
        (fun (parseState : FSharp.Text.Parsing.IParseState) ->
            let _1 = (let data = parseState.GetInput(1) in (Microsoft.FSharp.Core.Operators.unbox data : Absyn.expr)) in
            let _3 = (let data = parseState.GetInput(3) in (Microsoft.FSharp.Core.Operators.unbox data : Absyn.expr)) in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
# 48 "FunPar.fsy"
                                                               Prim("<>", _1, _3)     
                   )
# 48 "FunPar.fsy"
                 : Absyn.expr));
# 429 "FunPar.fs"
        (fun (parseState : FSharp.Text.Parsing.IParseState) ->
            let _1 = (let data = parseState.GetInput(1) in (Microsoft.FSharp.Core.Operators.unbox data : Absyn.expr)) in
            let _3 = (let data = parseState.GetInput(3) in (Microsoft.FSharp.Core.Operators.unbox data : Absyn.expr)) in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
# 49 "FunPar.fsy"
                                                               Prim(">",  _1, _3)     
                   )
# 49 "FunPar.fsy"
                 : Absyn.expr));
# 441 "FunPar.fs"
        (fun (parseState : FSharp.Text.Parsing.IParseState) ->
            let _1 = (let data = parseState.GetInput(1) in (Microsoft.FSharp.Core.Operators.unbox data : Absyn.expr)) in
            let _3 = (let data = parseState.GetInput(3) in (Microsoft.FSharp.Core.Operators.unbox data : Absyn.expr)) in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
# 50 "FunPar.fsy"
                                                               Prim("<",  _1, _3)     
                   )
# 50 "FunPar.fsy"
                 : Absyn.expr));
# 453 "FunPar.fs"
        (fun (parseState : FSharp.Text.Parsing.IParseState) ->
            let _1 = (let data = parseState.GetInput(1) in (Microsoft.FSharp.Core.Operators.unbox data : Absyn.expr)) in
            let _3 = (let data = parseState.GetInput(3) in (Microsoft.FSharp.Core.Operators.unbox data : Absyn.expr)) in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
# 51 "FunPar.fsy"
                                                               Prim(">=", _1, _3)     
                   )
# 51 "FunPar.fsy"
                 : Absyn.expr));
# 465 "FunPar.fs"
        (fun (parseState : FSharp.Text.Parsing.IParseState) ->
            let _1 = (let data = parseState.GetInput(1) in (Microsoft.FSharp.Core.Operators.unbox data : Absyn.expr)) in
            let _3 = (let data = parseState.GetInput(3) in (Microsoft.FSharp.Core.Operators.unbox data : Absyn.expr)) in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
# 52 "FunPar.fsy"
                                                               Prim("<=", _1, _3)     
                   )
# 52 "FunPar.fsy"
                 : Absyn.expr));
# 477 "FunPar.fs"
        (fun (parseState : FSharp.Text.Parsing.IParseState) ->
            let _2 = (let data = parseState.GetInput(2) in (Microsoft.FSharp.Core.Operators.unbox data : 'SetExpr)) in
            let _6 = (let data = parseState.GetInput(6) in (Microsoft.FSharp.Core.Operators.unbox data : 'SetExpr)) in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
# 53 "FunPar.fsy"
                                                                     Prim("++", _2, _6)     
                   )
# 53 "FunPar.fsy"
                 : Absyn.expr));
# 489 "FunPar.fs"
        (fun (parseState : FSharp.Text.Parsing.IParseState) ->
            let _2 = (let data = parseState.GetInput(2) in (Microsoft.FSharp.Core.Operators.unbox data : 'SetExpr)) in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
# 54 "FunPar.fsy"
                                                               Set(_2)                
                   )
# 54 "FunPar.fsy"
                 : Absyn.expr));
# 500 "FunPar.fs"
        (fun (parseState : FSharp.Text.Parsing.IParseState) ->
            let _1 = (let data = parseState.GetInput(1) in (Microsoft.FSharp.Core.Operators.unbox data : Absyn.expr)) in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
# 59 "FunPar.fsy"
                                                                [_1]                   
                   )
# 59 "FunPar.fsy"
                 : 'SetExpr));
# 511 "FunPar.fs"
        (fun (parseState : FSharp.Text.Parsing.IParseState) ->
            let _1 = (let data = parseState.GetInput(1) in (Microsoft.FSharp.Core.Operators.unbox data : 'SetExpr)) in
            let _3 = (let data = parseState.GetInput(3) in (Microsoft.FSharp.Core.Operators.unbox data : Absyn.expr)) in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
# 60 "FunPar.fsy"
                                                                _1 @ [_3]              
                   )
# 60 "FunPar.fsy"
                 : 'SetExpr));
# 523 "FunPar.fs"
        (fun (parseState : FSharp.Text.Parsing.IParseState) ->
            let _1 = (let data = parseState.GetInput(1) in (Microsoft.FSharp.Core.Operators.unbox data : Absyn.expr)) in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
# 64 "FunPar.fsy"
                                                               _1                     
                   )
# 64 "FunPar.fsy"
                 : Absyn.expr));
# 534 "FunPar.fs"
        (fun (parseState : FSharp.Text.Parsing.IParseState) ->
            let _1 = (let data = parseState.GetInput(1) in (Microsoft.FSharp.Core.Operators.unbox data : string)) in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
# 65 "FunPar.fsy"
                                                               Var _1                 
                   )
# 65 "FunPar.fsy"
                 : Absyn.expr));
# 545 "FunPar.fs"
        (fun (parseState : FSharp.Text.Parsing.IParseState) ->
            let _2 = (let data = parseState.GetInput(2) in (Microsoft.FSharp.Core.Operators.unbox data : string)) in
            let _4 = (let data = parseState.GetInput(4) in (Microsoft.FSharp.Core.Operators.unbox data : Absyn.expr)) in
            let _6 = (let data = parseState.GetInput(6) in (Microsoft.FSharp.Core.Operators.unbox data : Absyn.expr)) in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
# 66 "FunPar.fsy"
                                                               Let(_2, _4, _6)        
                   )
# 66 "FunPar.fsy"
                 : Absyn.expr));
# 558 "FunPar.fs"
        (fun (parseState : FSharp.Text.Parsing.IParseState) ->
            let _2 = (let data = parseState.GetInput(2) in (Microsoft.FSharp.Core.Operators.unbox data : string)) in
            let _3 = (let data = parseState.GetInput(3) in (Microsoft.FSharp.Core.Operators.unbox data : string)) in
            let _5 = (let data = parseState.GetInput(5) in (Microsoft.FSharp.Core.Operators.unbox data : Absyn.expr)) in
            let _7 = (let data = parseState.GetInput(7) in (Microsoft.FSharp.Core.Operators.unbox data : Absyn.expr)) in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
# 67 "FunPar.fsy"
                                                               Letfun(_2, _3, _5, _7) 
                   )
# 67 "FunPar.fsy"
                 : Absyn.expr));
# 572 "FunPar.fs"
        (fun (parseState : FSharp.Text.Parsing.IParseState) ->
            let _2 = (let data = parseState.GetInput(2) in (Microsoft.FSharp.Core.Operators.unbox data : Absyn.expr)) in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
# 68 "FunPar.fsy"
                                                               _2                     
                   )
# 68 "FunPar.fsy"
                 : Absyn.expr));
# 583 "FunPar.fs"
        (fun (parseState : FSharp.Text.Parsing.IParseState) ->
            let _1 = (let data = parseState.GetInput(1) in (Microsoft.FSharp.Core.Operators.unbox data : Absyn.expr)) in
            let _2 = (let data = parseState.GetInput(2) in (Microsoft.FSharp.Core.Operators.unbox data : Absyn.expr)) in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
# 72 "FunPar.fsy"
                                                               Call(_1, _2)           
                   )
# 72 "FunPar.fsy"
                 : Absyn.expr));
# 595 "FunPar.fs"
        (fun (parseState : FSharp.Text.Parsing.IParseState) ->
            let _1 = (let data = parseState.GetInput(1) in (Microsoft.FSharp.Core.Operators.unbox data : Absyn.expr)) in
            let _2 = (let data = parseState.GetInput(2) in (Microsoft.FSharp.Core.Operators.unbox data : Absyn.expr)) in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
# 73 "FunPar.fsy"
                                                               Call(_1, _2)           
                   )
# 73 "FunPar.fsy"
                 : Absyn.expr));
# 607 "FunPar.fs"
        (fun (parseState : FSharp.Text.Parsing.IParseState) ->
            let _1 = (let data = parseState.GetInput(1) in (Microsoft.FSharp.Core.Operators.unbox data : int)) in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
# 77 "FunPar.fsy"
                                                               CstI(_1)               
                   )
# 77 "FunPar.fsy"
                 : Absyn.expr));
# 618 "FunPar.fs"
        (fun (parseState : FSharp.Text.Parsing.IParseState) ->
            let _1 = (let data = parseState.GetInput(1) in (Microsoft.FSharp.Core.Operators.unbox data : bool)) in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
# 78 "FunPar.fsy"
                                                               CstB(_1)               
                   )
# 78 "FunPar.fsy"
                 : Absyn.expr));
|]
# 630 "FunPar.fs"
let tables () : FSharp.Text.Parsing.Tables<_> = 
  { reductions= _fsyacc_reductions ();
    endOfInputTag = _fsyacc_endOfInputTag;
    tagOfToken = tagOfToken;
    dataOfToken = _fsyacc_dataOfToken; 
    actionTableElements = _fsyacc_actionTableElements;
    actionTableRowOffsets = _fsyacc_actionTableRowOffsets;
    stateToProdIdxsTableElements = _fsyacc_stateToProdIdxsTableElements;
    stateToProdIdxsTableRowOffsets = _fsyacc_stateToProdIdxsTableRowOffsets;
    reductionSymbolCounts = _fsyacc_reductionSymbolCounts;
    immediateActions = _fsyacc_immediateActions;
    gotos = _fsyacc_gotos;
    sparseGotoTableRowOffsets = _fsyacc_sparseGotoTableRowOffsets;
    tagOfErrorTerminal = _fsyacc_tagOfErrorTerminal;
    parseError = (fun (ctxt:FSharp.Text.Parsing.ParseErrorContext<_>) -> 
                              match parse_error_rich with 
                              | Some f -> f ctxt
                              | None -> parse_error ctxt.Message);
    numTerminals = 33;
    productionToNonTerminalTable = _fsyacc_productionToNonTerminalTable  }
let engine lexer lexbuf startState = (tables ()).Interpret(lexer, lexbuf, startState)
let Main lexer lexbuf : Absyn.expr =
    Microsoft.FSharp.Core.Operators.unbox ((tables ()).Interpret(lexer, lexbuf, 0))
