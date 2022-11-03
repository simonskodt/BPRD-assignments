// Selection sort (slow algorithm!) in C#
// sestoft@dina.kvl.dk * 2007-03-21

using System;

class Selsort {
  public static void Main(String[] args) {
    int count = 10;
    int[] arr = new int[count];
    for (int i=0; i<count; i++)
      arr[i] = rnd.Next(1000000);
    SelectionSort(arr);
    for (int i=0; i<count; i++)
      Console.Write(arr[i] + " ");
    Console.WriteLine();
  }

  public static readonly Random rnd = new Random(); 

  public static void SelectionSort(int[] arr) {
    for (int i = 0; i < arr.Length; i++) {
      int least = i;                                      
      for (int j = i+1; j < arr.Length; j++)
	if (arr[j] < arr[least])
	  least = j;
      int tmp = arr[i]; arr[i] = arr[least]; arr[least] = tmp;
    }
  }
}
