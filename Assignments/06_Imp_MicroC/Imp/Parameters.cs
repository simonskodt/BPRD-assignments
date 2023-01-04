// Parameter passing: call-by-value and call-by-reference in C# * 2002-03-11
//
// Compile with:
//   csc Parameters.cs
// Run with: 
//   Parameters 11 22

using System;

class Parameters {
  public static void Main(String[] args) {
    int a = int.Parse(args[0]);
    int b = int.Parse(args[1]);
    // After the call to swapV, variables a and b are unchanged 
    Console.WriteLine("a = " + a + "  b = " + b);
    swapV(a, b);
    Console.WriteLine("a = " + a + "  b = " + b);
    Console.WriteLine("----------------------------------------");
    // After the call to swapR, the values of a and b have been swapped 
    Console.WriteLine("a = " + a + "  b = " + b);
    swapR(ref a, ref b);
    Console.WriteLine("a = " + a + "  b = " + b);
    int n = int.Parse(args[0]);
    int z = -1;
    square(n, ref z);
    Console.WriteLine("z = " + z);
    fac(n, ref z);
    Console.WriteLine("z = " + z);
  }

  static void swapV(int x, int y) {
    int tmp = x; x = y; y = tmp;
  }

  static void swapR(ref int x, ref int y) {
    int tmp = x; x = y; y = tmp;
  }

  static void square(int i, ref int r) {
    r = i * i;
  }

  static void fac(int n, ref int r) {
    if (n == 0)
      r = 1;
    else {
      int tmp = -1;
      fac(n-1, ref tmp);
      r = tmp * n;
    }
  }
}
