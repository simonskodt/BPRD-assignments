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
        for (i = 0; i < n; i = i + 1) {
            arr[i] = i * i;
            print arr[i];
        }
    }
}

void arrsum(int n, int arr[], int *sump) {
    int i;
    int sum;
    sum = 0;
    for (i = 0; i < n; i = i + 1)
    {
        sum = sum + arr[i];
    }
    *sump = sum;
}
