// micro-C example on tail calls.

int id(int b) {
  return b;
}

int f(int a) {
  int b;
  b = a + 1;
  return id(b);
}

void main(int n) {
  print f(2);
}

