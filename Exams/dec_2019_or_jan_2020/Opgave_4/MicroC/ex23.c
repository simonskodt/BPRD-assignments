// micro-C example 23
// Call-by-value and call-by-reference

void main() {
  int a;
  print &a;
  a = 11;
  int b;
  print &b;  
  b = 22;
  swapV(a,b);
  print a;
  print b;
  swapR(&a,&b);
  print a;
  print b;
}

void swapV(int x, int y) {
  int tmp;
  print &tmp;
  print &x;
  print &y;
  tmp = x;
  x=y;
  y=tmp;
}

void swapR(int *x, int *y) {
  int tmp;
  print &tmp;
  print x;
  print y;
  tmp = *x;
  *x=*y;
  *y=tmp;
}
