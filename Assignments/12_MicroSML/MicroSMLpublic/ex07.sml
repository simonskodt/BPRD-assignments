val c = 42
fun f x = x + g (x-1)
and g x = if x < 1 then c else x + f (x-1)

begin
  f 5
end
