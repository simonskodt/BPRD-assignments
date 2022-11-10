// http://www.c-sharpcorner.com/article/resurrection-and-the-net-garbage-collector/
// mcs compiler on Mac.
using System;
public class B { 
  static public A IntA; 
}

public class A { 
  private int x = 10; 
  public void DoIt() { 
    Console.WriteLine( "Value : {0}", x ); 
  } 

  ~A() {
  Console.WriteLine( "Enter destructor" );
  B.IntA = this;
  Console.WriteLine( "Exit destructor with value : {0}", x ); 
  } 
}

public class Test { 
  static void Main() { 
    A a = new A(); 
    a.DoIt(); 
    a = null; 
    GC.Collect(); 
    B.IntA.DoIt(); 
  }
}