(* Polymorphic type inference for a higher-order functional language    *)
(* The operator (=) only requires that the arguments have the same type *)
(* sestoft@itu.dk 2010-01-07 *)

module TypeInference

(* If this looks depressingly complicated, read chapter 6 of PLCSD *)

open Absyn

(* Debugging *)
let debugP = false
let debug s = if debugP then printfn "DEBUG: %s" s else ()

(* Environment operations *)

type 'v env = (string * 'v) list

let rec lookup env x =
    match env with 
    | []        -> failwith (x + " not found")
    | (y, v)::r -> if x=y then v else lookup r x;;

(* Operations on sets of type variables, represented as lists.  
   Inefficient but simple.  Basically compares type variables 
   on their string names.  Correct so long as all type variable names
   are distinct. *)

let rec mem x vs = 
    match vs with
    | []      -> false
    | v :: vr -> x=v || mem x vr;;

(* union(xs, ys) is the set of all elements in xs or ys, without duplicates *)

let rec union (xs, ys) = 
    match xs with 
    | []    -> ys
    | x::xr -> if mem x ys then union(xr, ys)
               else x :: union(xr, ys);;

(* unique xs  is the set of members of xs, without duplicates *)

let rec unique xs = 
    match xs with
    | []    -> []
    | x::xr -> if mem x xr then unique xr else x :: unique xr;;

(* A type is int, bool, function, or type variable: *)

type typ =
     | TypI                                (* integers                   *)
     | TypB                                (* booleans                   *)
     | TypF of typ * typ                   (* (argumenttype, resulttype) *)
     | TypV of typevar                     (* type variable              *)

and tyvarkind =  
     | NoLink of string                    (* uninstantiated type var.   *)
     | LinkTo of typ                       (* instantiated to typ        *)

and typevar =
     (tyvarkind * int) ref                 (* kind and binding level     *)

(* A type scheme is a list of generalized type variables, and a type: *)

type typescheme = 
     | TypeScheme of typevar list * typ   (* type variables and type    *)

(* *)

let setTvKind tyvar newKind =
    let (kind, lvl) = !tyvar
    tyvar := (newKind, lvl)

let setTvLevel tyvar newLevel =
    let (kind, lvl) = !tyvar
    tyvar := (kind, newLevel)

(* Normalize a type; make type variable point directly to the
   associated type (if any).  This is the `find' operation, with path
   compression, in the union-find algorithm. *)
(* NHA
let rec normType t0 = 
    match t0 with
    | TypV tyvar ->
      match !tyvar with 
      | (LinkTo t1, _) -> let t2 = normType t1 
                          setTvKind tyvar (LinkTo t2); t2
      | _ -> t0
    |  _ -> t0;
*)
(* Find operation without path compression. *)
let rec normType t0 = 
    match t0 with
    | TypV tyvar ->
      match !tyvar with 
      | (LinkTo t1, _) -> let t2 = normType t1 
                          (*let _ = setTvKind tyvar (LinkTo t2)*)
                          t2
      | _ -> t0
    |  _ -> t0;

let rec freeTypeVars t : typevar list = 
    match normType t with
    | TypI        -> []
    | TypB        -> []
    | TypV tv     -> (*(printfn "%s" ("found free tyvar"));*) [tv]
    | TypF(t1,t2) -> union(freeTypeVars t1, freeTypeVars t2)

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
    pr t 

let rec showTEnv tenv =
  let showKind = function
    NoLink s -> "NoLink("+s+")"
  | LinkTo typ -> "LinkTo("+(showType typ)+")"
  let showTyvar tv =
    match !tv with
      (kind,lvl) -> "("+(showKind kind)+","+(lvl.ToString())+")"
  let showTypescheme = function
    TypeScheme(tvs,typ) -> "V" + (List.foldBack (fun tv a -> (showTyvar tv) + "." + a) tvs "") + (showType typ)
  List.foldBack (fun (v,ts) a -> v + "->" + (showTypescheme ts) + ";" + a) tenv ""
      
(* Unify two types, equating type variables with types as necessary *)

let rec unify t1 t2 : unit =
    let t1' = normType t1
    let t2' = normType t2
    let _ = debug ("Unify called with t1=" + (showType t1) + " and t2= " + (showType t2))
    match (t1', t2') with
    | (TypI, TypI) -> ()
    | (TypB, TypB) -> ()
    | (TypF(t11, t12), TypF(t21, t22)) -> (unify t11 t21; unify t12 t22)
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

(* Generate fresh type variables *)

let tyvarno = ref 0

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
    let _ = debug ("generalize with level: " + level.ToString())
    let notfreeincontext tyvar = 
        let (_, linkLevel) = !tyvar
        let _ = debug ("  linklevel = " + linkLevel.ToString())
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
    | TypI        -> TypI
    | TypB        -> TypB

(* Create a type from a type scheme (tvs, t) by instantiating all the
   type scheme's parameters tvs with fresh type variables *)

let specialize level (TypeScheme(tvs, t)) : typ =
    let bindfresh tv = (tv, TypV(newTypeVar level))
    match tvs with
    | [] -> t
    | _  -> let subst = List.map bindfresh tvs
            copyType subst t

(* A type environment maps a program variable name to a typescheme *)

type tenv = typescheme env

(* Type inference helper function:
   (typ lvl env e) returns the type of e in env at level lvl *)

let rec typ (lvl : int) (env : tenv) (e : expr) : typ =
    match e with
    | CstI i -> TypI
    | CstB b -> TypB
    | Var x  -> specialize lvl (lookup env x)
    | Prim(ope, e1, e2) ->
      let _ = debug ("Type Prim on e1: " + (showTEnv env))      
      let t1 = typ lvl env e1
      let _ = debug ("Type Prim on e2: " + (showTEnv env))      
      let t2 = typ lvl env e2
      match ope with
      | "*" -> (unify TypI t1; unify TypI t2; TypI)
      | "+" -> (unify TypI t1; unify TypI t2; TypI)
      | "-" -> (unify TypI t1; unify TypI t2; TypI)
      | "=" -> (unify t1 t2; TypB)
      | "<" -> (unify TypI t1; unify TypI t2; TypB)
      | "&" -> (unify TypB t1; unify TypB t2; TypB)
      | _   -> failwith ("unknown primitive " + ope) 
    | Let(x, eRhs, letBody) -> 
      let lvl1 = lvl + 1
      let resTy = typ lvl1 env eRhs
      let letEnv = (x, generalize lvl resTy) :: env
      typ lvl letEnv letBody
    | If(e1, e2, e3) ->
      let t2 = typ lvl env e2
      let t3 = typ lvl env e3
      unify TypB (typ lvl env e1);
      unify t2 t3;
      t2
    | Letfun(f, x, fBody, letBody) -> 
      let lvl1 = lvl + 1
      let fTyp = TypV(newTypeVar lvl1)
      let xTyp = TypV(newTypeVar lvl1)
      let fBodyEnv = (x, TypeScheme([], xTyp)) 
                      :: (f, TypeScheme([], fTyp)) :: env
      let rTyp = typ lvl1 fBodyEnv fBody
      let _    = unify fTyp (TypF(xTyp, rTyp))
      let bodyEnv = (f, generalize lvl fTyp) :: env
      let _ = debug ("Letfun letBodyEnv: " + (showTEnv bodyEnv))
      typ lvl bodyEnv letBody
    | Call(eFun, eArg) ->
      let _ = debug ("Type Call: " + (showTEnv env))
      let tf = typ lvl env eFun 
      let tx = typ lvl env eArg
      let tr = TypV(newTypeVar lvl)
      unify tf (TypF(tx, tr));
      tr

(* Type inference: tyinf e0 returns the type of e0, if any *)

let rec tyinf e0 = typ 0 [] e0

let inferType e = 
    (tyvarno := 0;
     showType (tyinf e));;

