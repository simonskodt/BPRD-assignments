int *sump;

void main(int n) {
    int arr[20];

    squares(n, arr);

    arrsum(n, arr, sump);

    print *sump;
}

void squares(int n, int arr[]) {
    if (n <= 20) {
        int i;
        i = 0;
        while (i < n) {
            arr[i] = i * i;
            print arr[i];
            i = i + 1;
        }
    }
}

void arrsum(int n, int arr[], int *sump) {
    int i;
    i = 0;
    int sum;
    sum = 0;
    while (i < n)
    {
        sum = sum + arr[i];
        i = i + 1;
    }
    *sump = sum;
}
