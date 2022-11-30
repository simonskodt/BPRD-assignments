fun genList n =
  let
    fun loop n = fn acc ->
      if n < 0 then acc else loop (n-1) (n::acc)
  in
    loop n nil
  end

fun map f = fn xs ->
  if isnil xs then nil else f (hd xs) :: map f (tl xs)

begin
  let
    val xs = genList 10
  in
    print xs;
    print (map (fn x -> x + 1) xs)
  end	       
end
					     
