// The factorial function in continuation-passing style, in Java.
// sestoft@itu.dk * 2009-10-24

// A Cont object is a continuation: it has a method k that given the
// result of a subcomputation will return the final result:

interface Cont {
  int k(int v);
}

// Initial call to facc, factorial in continuation-passing style, with
// the identity continuation as argument:

class Factorial {
  public static void main(String[] args) {
    int n = Integer.parseInt(args[0]);
    System.out.println(facc(n, 
                            new Cont() {
                                public int k(int v) {
                                  return v;
                                }
                              }));
  }

  // A continuation-passing version of factorial.  This is a straight
  // translation of the functional (F#) version:

  static int facc(final int n, final Cont cont) {
    if (n == 0) 
      return cont.k(1);
    else
      return facc(n-1, 
                  new Cont() {
                      public int k(int v) {
                        return cont.k(n * v);
                      }
                    });
  }
}
