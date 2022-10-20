import java.util.Arrays;

public class Merge {
    public static void main(String[] args) {
        int[] xs = { 3, 5, 12 };
        int[] ys = { 2, 3, 4, 7 };
        System.out.println(Arrays.toString(xs));
        System.out.println(Arrays.toString(ys));
        System.out.println(Arrays.toString(merge(xs, ys)));
    }

    static int[] merge(int[] xs, int[] ys) {
        int[] orderedLst = new int[xs.length + ys.length];

        for (int i = 0; i < xs.length; i++)
            orderedLst[i] = xs[i];

        for (int i = 0, lstCounter = xs.length; i < ys.length; i++, lstCounter++)
            orderedLst[lstCounter] = ys[i];

        Arrays.sort(orderedLst);
        
        return orderedLst;
    }
}
