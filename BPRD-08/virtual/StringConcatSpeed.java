// Studying the overhead, including garbage collection,
// of repeated string concatenation
// sestoft@itu.dk * 2009-10-25

// Usage:
//    javac StringConcatSpeed.java
//    java -verbosegc StringConcatSpeed 

public class StringConcatSpeed {
  public static void main(String[] args) {
    final int count = 30000;

    System.out.println("Initialization: Building array of small strings");

    String[] ss = new String[count];
    for (int i=1; i<=count; i++)
      ss[i-1] = i + " ";

    System.out.println("\nConcatenate using StringBuilder:");
    stringBuilder(ss, count);
    System.out.println("\nPress return to continue...");
    try { System.in.read(); } catch (Exception e) { } 
    System.out.println("\nConcatenate using repeated string concatenation:");
    stringConcat(ss, count);
  }

  private static void stringBuilder(String[] ss, int n) {
    // This is fast for all n
    Timer t = new Timer(); 
    String res = null;
    StringBuilder buf = new StringBuilder();
    for (int i=0; i<n; i++) 
      buf.append(ss[i]);
    res = buf.toString();
    System.out.format("Result length:%7d;   time:%8.3f sec\n" , res.length(), t.Check());
  }

  private static void stringConcat(String[] ss, int n) {
    // This is *very* slow for large n
    Timer t = new Timer(); 
    String res = "";
    for (int i=0; i<n; i++) 
      res += ss[i];
    System.out.format("Result length:%7d;   time:%8.3f sec\n" , res.length(), t.Check());
  }
}

// Crude timing utility ----------------------------------------
   
class Timer {
  private long start;

  public Timer() {
    start = System.currentTimeMillis();
  }

  public double Check() {
    return (System.currentTimeMillis()-start)/1000.0;
  }
}
