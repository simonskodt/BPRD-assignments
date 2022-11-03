// Selection sort (slow algorithm!) Java
// sestoft@dina.kvl.dk * 2007-03-21

import java.util.Random;

class Selsort {
  public static void main(String[] args) {
    int count = 10;
    int[] arr = new int[count];
    for (int i=0; i<count; i++)
      arr[i] = rnd.nextInt(1000000);
    SelectionSort(arr);
    for (int i=0; i<count; i++)
      System.out.print(arr[i] + " ");
    System.out.println();
  }

  public static final Random rnd = new Random(); 

  public static void SelectionSort(int[] arr) {
    for (int i = 0; i < arr.length; i++) {
      int least = i;                                      
      for (int j = i+1; j < arr.length; j++)
	if (arr[j] < arr[least])
	  least = j;
      int tmp = arr[i]; arr[i] = arr[least]; arr[least] = tmp;
    }
  }
}
