(* Polymorphic type inference for a higher-order functional language    *)
(* The operator (=) only requires that the arguments have the same type *)
(* sestoft@itu.dk 2010-01-07 *)
(* nh@itu.dk 2015-01-12 *)

module TypeInference

(* If this looks depressingly complicated, read chapter 6 of PLCSD *)

open Absyn

(* Environment operations *)

type 'v env = (string * 'v) list

let rec lookup env x =
  match env with 
  | []        -> failwith ("TypeInference.lookup: " + x + " not found")
  | (y, v)::r -> if x=y then v else lookup r x

(* Operations on sets of type variables, represented as lists.  
   Inefficient but simple.  Basically compares type variables 
   on their string names.  Correct so long as all type variable names
   are distinct. *)

let rec mem x vs = 
  match vs with
  | []      -> false
  | v :: vr -> x=v || mem x vr

(* union(xs, ys) is the set of all elements in xs or ys, without duplicates *)

let rec union (xs, ys) = 
  match xs with 
  | []    -> ys
  | x::xr -> if mem x ys then union(xr, ys)
             else x :: union(xr, ys)

(* unique xs  is the set of members of xs, without duplicates *)

let rec unique xs = 
  match xs with
  | []    -> []
  | x::xr -> if mem x xr then unique xr else x :: unique xr

(* A type is int, bool, function, or type variable: *)

type typ =
  | TypI                                (* integers                   *)
  | TypB                                (* booleans                   *)
  | TypF of typ * typ                   (* (argumenttype, resulttype) *)
  | TypV of typevar                     (* type variable              *)
  | TypL of typ                         (* lists                      *)
  | TypE                                (* Exception type             *)

and tyvarkind =  
  | NoLink of string                    (* uninstantiated type var.   *)
  | LinkTo of typ                       (* instantiated to typ        *)

and typevar =
  (tyvarkind * int) ref                 (* kind and binding level     *)

(* A type scheme is a list of generalized type variables, and a type: *)

type typescheme = 
  | TypeScheme of typevar list * typ   (* type variables and type    *)

let setTvKind tyvar newKind =
  let (kind, lvl) = !tyvar
  tyvar := (newKind, lvl)

let setTvLevel tyvar newLevel =
  let (kind, lvl) = !tyvar
  tyvar := (kind, newLevel)

(* Normalize a type; make type variable point directly to the
   associated type (if any).  This is the `find' operation, with path
   compression, in the union-find algorithm. *)

let rec normType t0 = 
  match t0 with
  | TypV tyvar ->
    match !tyvar with 
    | (LinkTo t1, _) -> let t2 = normType t1 
                        setTvKind tyvar (LinkTo t2); t2
    | _ -> t0
  |  _ -> t0;

let rec resolveType t0 = 
  match t0 with
  | TypV tyvar ->
    match !tyvar with 
    | (LinkTo t1, _) -> resolveType t1 
    | _ -> t0
  | TypF(t1,t2) -> TypF(resolveType t1, resolveType t2)
  | TypL t -> TypL (resolveType t)
  | _ -> t0;

let rec freeTypeVars t : typevar list = 
  match normType t with
  | TypI        -> []
  | TypB        -> []
  | TypV tv     -> [tv]
  | TypF(t1,t2) -> union(freeTypeVars t1, freeTypeVars t2)
  | TypL t      -> freeTypeVars t
  | TypE        -> []

let occurCheck tyvar tyvars =                     
  if mem tyvar tyvars then failwith "type error: circularity" else ()

let pruneLevel maxLevel tvs = 
  let reducelevel tyvar = 
    let (_, level) = !tyvar
    setTvLevel tyvar (min level maxLevel)
  List.iter reducelevel tvs 

(* Make type variable tyvar equal to type t (by making tyvar link to t),
   but first check that tyvar does not occur in t, and reduce the level
   of all type variables in t to that of tyvar.  This is the `union'
   operation in the union-find algorithm.  *)

let rec linkVarToType tyvar t =
  let (_, level) = !tyvar
  let fvs = freeTypeVars t
  occurCheck tyvar fvs;
  pruneLevel level fvs;
  setTvKind tyvar (LinkTo t)

let rec typeToString t : string =
  match t with
  | TypI         -> "int"
  | TypB         -> "bool"
  | TypV _       -> failwith "typeToString impossible"
  | TypF(t1, t2) -> "function"
  | TypL t       -> "list"
  | TypE         -> "exn"
            
(* Unify two types, equating type variables with types as necessary *)

let rec unify t1 t2 : unit =
  let t1' = normType t1
  let t2' = normType t2
  match (t1', t2') with
  | (TypI, TypI) -> ()
  | (TypB, TypB) -> ()
  | (TypF(t11, t12), TypF(t21, t22)) -> (unify t11 t21; unify t12 t22)
  | (TypL t1, TypL t2) -> unify t1 t2
  | (TypE, TypE) -> ()
  | (TypV tv1, TypV tv2) -> 
    let (_, tv1level) = !tv1
    let (_, tv2level) = !tv2
    if tv1 = tv2                then () 
    else if tv1level < tv2level then linkVarToType tv1 t2'
                                else linkVarToType tv2 t1'
  | (TypV tv1, _       ) -> linkVarToType tv1 t2'
  | (_,        TypV tv2) -> linkVarToType tv2 t1'
  | (TypI,     t) -> failwith ("type error: int and " + typeToString t)
  | (TypB,     t) -> failwith ("type error: bool and " + typeToString t)
  | (TypF _,   t) -> failwith ("type error: function and " + typeToString t)
  | (TypL _,   t) -> failwith ("type error: list and " + typeToString t)
  | (TypE,     t) -> failwith ("type error: exception and " + typeToString t)
  
(* Generate fresh type variables *)

let tyvarno = ref 0
let resetTyvarno() = tyvarno := 0

let newTypeVar level : typevar = 
  let rec mkname i res = 
    if i < 26 then char(97+i) :: res
    else mkname (i/26-1) (char(97+i%26) :: res)
  let intToName i = new System.String(Array.ofList('\'' :: mkname i []))
  tyvarno := !tyvarno + 1;
  ref (NoLink (intToName (!tyvarno)), level)

(* Generalize over type variables not free in the context; that is,
   over those whose level is higher than the current level: *)

let rec generalize level (t : typ) : typescheme =
  let notfreeincontext tyvar = 
    let (_, linkLevel) = !tyvar 
    linkLevel > level
  let tvs = List.filter notfreeincontext (freeTypeVars t)
  TypeScheme(unique tvs, t)  // The unique call seems unnecessary because freeTypeVars has no duplicates??

(* Copy a type, replacing bound type variables as dictated by tvenv,
   and non-bound ones by a copy of the type linked to *)

let rec copyType subst t : typ = 
  match t with
  | TypV tyvar ->
    let (* Could this be rewritten so that loop does only the substitution *)
      rec loop subst1 =          
      match subst1 with 
      | (tyvar1, type1) :: rest -> if tyvar1 = tyvar then type1 else loop rest
      | [] -> match !tyvar with
              | (NoLink _, _)  -> t
              | (LinkTo t1, _) -> copyType subst t1
    loop subst
  | TypF(t1,t2) -> TypF(copyType subst t1, copyType subst t2)
  | TypL t      -> TypL(copyType subst t)  
  | TypI        -> TypI
  | TypB        -> TypB
  | TypE        -> TypE

(* Create a type from a type scheme (tvs, t) by instantiating all the
   type scheme's parameters tvs with fresh type variables *)

let specialize level (TypeScheme(tvs, t)) : typ =
  let bindfresh tv = (tv, TypV(newTypeVar level))
  match tvs with
  | [] -> t
  | _  -> let subst = List.map bindfresh tvs
          copyType subst t

(* Pretty-print type, using names 'a, 'b, ... for type variables *)

let rec showType t : string =
  let rec pr t = 
    match normType t with
    | TypI         -> "int"
    | TypB         -> "bool"
    | TypV tyvar   -> 
      match !tyvar with
      | (NoLink name, _) -> name
      | _                -> failwith "showType impossible"
    | TypF(t1, t2) -> "(" + pr t1 + " -> " + pr t2 + ")"
    | TypL t       -> "(" + pr t + " list" + ")"
    | TypE         -> "exn"
  pr t 

(* A type environment maps a program variable name to a typescheme *)

type tenv = typescheme env

(* Type inference helper function:
   (typ lvl env e) returns the type of e in env at level lvl *)

let rec typExpr (lvl : int) (env : tenv) (e : expr<'a>) : typ * expr<typ> =
  match e with
  | CstI (i,_) -> (TypI, CstI (i,Some TypI))
  | CstB (b,_) -> (TypB, CstB (b,Some TypB))
  | CstN _     ->
    let lTyp = TypL(TypV(newTypeVar lvl))
    (lTyp, CstN (Some lTyp))
  | Var (x,_)  ->
    let typX = specialize lvl (lookup env x)
    (typX,Var(x,Some typX))
  | Prim1(ope,e1,_) -> 
    let (t1,e1') = typExpr lvl env e1
    match ope with
    (* Print may consume any type and the result is just the type. *)
    (* This assumes that print actually works on any type. *)
    | "print" -> (t1,Prim1(ope,e1',Some t1)) 
    | "hd" -> let tv = TypV(newTypeVar lvl)
              unify (TypL tv) t1;
              (tv,Prim1(ope,e1',Some tv))
    | "tl" -> let tv = TypV(newTypeVar lvl)        
              unify (TypL tv) t1;
              (t1,Prim1(ope,e1',Some t1))
    | "isnil" -> let tv = TypV(newTypeVar lvl)
                 unify (TypL tv) t1;
                 (TypB,Prim1(ope,e1',Some TypB))
    | _ -> failwith ("typ of Prim1 " + ope + " not implemented")
  | Prim2(ope,e1,e2,_) -> 
    let (t1,e1') = typExpr lvl env e1
    let (t2,e2') = typExpr lvl env e2
    let tr = match ope with
             | "*" -> (unify TypI t1; unify TypI t2; TypI)
             | "+" -> (unify TypI t1; unify TypI t2; TypI)
             | "-" -> (unify TypI t1; unify TypI t2; TypI)
             | "%" -> (unify TypI t1; unify TypI t2; TypI)
             | "=" -> (unify t1 t2; TypB)
             | "<" -> (unify TypI t1; unify TypI t2; TypB)
             | ">" -> (unify TypI t1; unify TypI t2; TypB)
             | "<=" -> (unify TypI t1; unify TypI t2; TypB)
             | ">=" -> (unify TypI t1; unify TypI t2; TypB)
             | "&" -> (unify TypB t1; unify TypB t2; TypB)
             | "::" -> (unify (TypL t1) t2; t2)
             | _   -> failwith ("typExpr: unknown primitive " + ope)
    (tr,Prim2(ope,e1',e2',Some tr))
  | AndAlso(e1,e2,_) ->
    let (t1,e1') = typExpr lvl env e1
    let (t2,e2') = typExpr lvl env e2
    unify TypB t1;
    unify TypB t2;
    (TypB,AndAlso(e1',e2',Some TypB))
  | OrElse(e1,e2,_) ->
    let (t1,e1') = typExpr lvl env e1
    let (t2,e2') = typExpr lvl env e2
    unify TypB t1;
    unify TypB t2;
    (TypB,OrElse(e1',e2',Some TypB))
  | Seq(e1,e2,_) -> (* e1 may have any type *)
    let (t1,e1') = typExpr lvl env e1
    let (t2,e2') = typExpr lvl env e2
    (t2,Seq(e1',e2',Some t2))
  | Let(valdecs,letBody) ->
    let (valdecs',letEnv) = typValdecs lvl env valdecs
    let (tLetBody,letBody') = typExpr lvl letEnv letBody
    (tLetBody, Let(List.rev valdecs',letBody'))    
  | If(e1, e2, e3) ->
    let (t2,e2') = typExpr lvl env e2
    let (t3,e3') = typExpr lvl env e3
    let (t1,e1') = typExpr lvl env e1
    unify TypB t1;
    unify t2 t3;
    (t2,If(e1',e2',e3'))
  | Fun(x,fBody,_) -> 
    let lvl1 = lvl + 1
    let xTyp = TypV(newTypeVar lvl1)
    let fBodyEnv = (x, TypeScheme([], xTyp)) :: env
    let (rTyp,fBody') = typExpr lvl1 fBodyEnv fBody
    let tr = TypF(xTyp, rTyp)
    (tr,Fun(x,fBody',Some tr))
  | Call(eFun,eArg,tOpt,_) -> 
    let (tf,eFun') = typExpr lvl env eFun 
    let (tx,eArg') = typExpr lvl env eArg
    let tr = TypV(newTypeVar lvl)
    unify tf (TypF(tx, tr));
    (tr,Call(eFun',eArg',tOpt,Some tr))
  | Raise(e,_) ->
    let (te,e') = typExpr lvl env e
    let tr = TypV(newTypeVar lvl)
    unify te TypE;         (* e must be of type exn *)
    (tr,Raise(e',Some tr)) (* Arbitrary result type *)
  | TryWith(e1,ExnVar x,e2) ->
    let (t1,e1') = typExpr lvl env e1
    let (t2,e2') = typExpr lvl env e2
    let typX = specialize lvl (lookup env x)
    unify t1 t2;
    unify typX TypE;
    (t1, TryWith(e1',ExnVar x,e2'))
and typValdec (lvl:int) (env:tenv) (t:valdec<'a>) : tenv * valdec<typ> =
  match t with
  | Fundecs fs -> 
    let lvl1 = lvl + 1
    let genFunTypes (f, x, fBody) =
      let fTyp = TypV(newTypeVar lvl1)
      let xTyp = TypV(newTypeVar lvl1)
      let xTypeScheme = (x, TypeScheme([], xTyp))
      let fTypeScheme = (f, TypeScheme([], fTyp))
      (xTyp, fTyp, xTypeScheme, fTypeScheme,f,x,fBody)
    let funTypes = List.map genFunTypes fs
    let funBodyEnv = List.foldBack 
                       (fun (_,_,_,fTypeScheme,_,_,_) env -> fTypeScheme :: env) funTypes env  
    let fs' = List.map (fun (xTyp,fTyp,xTypeScheme,_,f,x,fBody) ->
                          let (rTyp,fBody') = typExpr lvl1 (xTypeScheme :: funBodyEnv) fBody
                          unify fTyp (TypF(xTyp,rTyp));
                          (f,x,fBody')) funTypes                           
    let bodyEnv = List.foldBack 
                    (fun (_,fTyp,_,_,f,_,_) env -> (f,generalize lvl fTyp) :: env) funTypes env
    (bodyEnv,Fundecs fs')
  | Valdec (x,eRhs) -> 
    let lvl1 = lvl + 1
    let (resTy,eRhs') = typExpr lvl1 env eRhs
    let letEnv = (x, generalize lvl resTy) :: env
    (letEnv,Valdec(x,eRhs'))
  | Exn(ExnVar x,_) -> ((x,generalize lvl TypE) :: env,Exn(ExnVar x,Some TypE))
and typValdecs (lvl:int) (env:tenv) (valdecs:valdec<'a> list) : valdec<typ> list * tenv =
  List.fold (fun (valdecs,env) valdec -> let (env',valdec') = typValdec lvl env valdec
                                         (valdec'::valdecs,env')) ([],env) valdecs
and typProg (lvl:int) (env:tenv) (t:program<'a>) : typ * tenv * program<typ> =
  match t with
  | Prog(valdecs,e) -> 
    let (valdecs',env') = typValdecs lvl env valdecs
    let (typ,e') = typExpr lvl env' e
    (typ,env',Prog(List.rev valdecs',e'))

(* Type inference: tyinf e0 returns the type of e0, if any *)


let inferProg p = 
  resetTyvarno();
  typProg 0 [] p

let inferTopdec t = 
  resetTyvarno();
  typValdec 0 [] t

let inferExpr e = 
  resetTyvarno();
  typExpr 0 [] e

let getTypExpr e = (normType << Option.get << Absyn.getOptExpr) e
