int nFac;
int resFac;

void main(int n) {
    int i;
    i = 0;
    nFac=0;
    while (i < n) {
        resFac = fac(i);
        i = i + 1;
    }
    printStack 42;
}

int fac(int n) {
    nFac = nFac + 1;
    printStack nFac;
    if (n == 0)
        return 1;
    else
        return n * fac(n-1);
}