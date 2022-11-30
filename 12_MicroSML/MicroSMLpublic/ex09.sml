fun f x = if x < 0 then g 4 else f (x-1)
and g x = x  

begin
  print (f 2)
end
