// micro-C example 13, in Java

class ex13 {
  public static void main(String[] args) {
    int n = Integer.parseInt(args[0]);
    int y;
    y = 1889;
    while (y < n) {
      y = y + 1;
      if (y % 4 == 0 && (y % 100 != 0 || y % 400 == 0))
	InOut.printi(y);
    }
    InOut.printc((char)10);
  }
}

class InOut {
  public static void printi(int i) { 
    System.out.print(i + " ");
  }

  public static void printc(char c) { 
    System.out.print(c);
  }
}
