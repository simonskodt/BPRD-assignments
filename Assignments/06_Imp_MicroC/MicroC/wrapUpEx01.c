/* This example used in Wrap Up session to show
     - abstract syntax
     - code generation for functions and function calls
     - code generation for L-values:
         AccVar, Variable access: x = e 
         AccDeref, Pointer dereferencing: *p 
         AccIndex, Array indexing: a[e]
     - code generation for using pointers as return values
 */
   
void main() {
  int a;

  AccVar();

  AccDeref();

  a=f(41);    

  g(&a);
}

void AccVar() {
  int i;

  i = 42;
}

void AccDeref() {
  int *i;

  *i = 42;
}

void AccIndex() {
  int i[2];

  i[0] = 42;
}

int f(int i) {
  return i+1;
}

void g(int *i) {
  *i = 42;
}
