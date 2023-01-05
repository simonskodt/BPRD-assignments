#include <stdio.h>

// In C an array parameter holds the base address of the array:

void f(int a[], int b[]) {
  a = b;                                    // Legal!
  printf("%d %d\n", a[0], b[0]);            // Prints 22 22
}

// Other array variables do not have an lvalue:

int main() {
  int a[10];
  int b[10];
  *a = 11;                                  // Same as a[0] = 11
  *b = 22;                                  // Same as b[0] = 22
  // a = b;                                 // Illegal: incompatible types
  printf("%d %d\n", a[0], b[0]);            // Prints 11 22
  f(a, b);
  printf("%d %d\n", a[0], b[0]);            // Prints 11 22
  return 0;
}
