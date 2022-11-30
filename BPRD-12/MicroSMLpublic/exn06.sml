(* Generative exceptions example *)

begin
  let
    val x = 0
    exception GTZERO
    exception LTZERO
  in
    raise (if x > 0 then GTZERO else LTZERO)
  end
end
