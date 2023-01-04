// Studying the overhead, including garbage collection,
// of repeated string concatenation
// sestoft@itu.dk * 2009-10-25

// Usage:
//    csc /o StringConcatSpeed.cs
//    StringConcatSpeed 

using System;
using System.Text;		// For StringBuilder
using System.Diagnostics;       // For Stopwatch

public class StringConcatSpeed {
  public static void Main(String[] args) {
    const int count = 30000;

    Console.WriteLine("Initialization: Building array of small strings");

    String[] ss = new String[count];
    for (int i=1; i<=count; i++)
      ss[i-1] = i + " ";

    Console.WriteLine("\nConcatenate using StringBuilder:");
    stringBuilder(ss, count);
    Console.WriteLine("\nPress return to continue...");
    Console.In.Read(); 
    Console.WriteLine("\nConcatenate using repeated string concatenation:");
    stringConcat(ss, count);
  }

  private static void stringBuilder(String[] ss, int n) {
    // This is fast for all n
    Stopwatch t = new Stopwatch(); 
    t.Reset();
    t.Start();
    String res = null;
    StringBuilder buf = new StringBuilder();
    for (int i=0; i<n; i++) 
      buf.Append(ss[i]);
    res = buf.ToString();
    t.Stop();
    Console.WriteLine("Result length:{0,7};    time:{1,8:F3} sec\n" , 
		      res.Length, t.ElapsedMilliseconds/1000);
  }

  private static void stringConcat(String[] ss, int n) {
    // This is *very* slow for large n
    Stopwatch t = new Stopwatch(); 
    t.Reset();
    t.Start();
    String res = "";
    for (int i=0; i<n; i++) 
      res += ss[i];
    t.Stop();
    Console.WriteLine("Result length:{0,7};    time:{1,8:F3} sec\n" , 
		      res.Length, t.ElapsedMilliseconds/1000.0);
  }
}
