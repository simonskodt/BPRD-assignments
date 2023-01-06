# Assignments

```c
// micro-C December 2019 example 2
int g;
void main() {
g=42;
int n;
n=1234;
int *p;
p = &n;
int *pn[1];
pn[0] = &g;
int a[2];
a[0] = 2;
a[1] = 4;
printCurFrame;
}
```

Output:

```bash
[ ]{0: INCSP 1}
...
[ 42 6 -999 1234 3 0 5 2 4 7 ]{89: PRINTCURFRM}
Current Stack Frame (bp=3):
n at bp[0] = 1234
p at bp[1] = 3
pn at bp[3] = 5
a at bp[6] = 7
[ 42 6 -999 1234 3 0 5 2 4 7 ]{104: INCSP -7}
[ 42 6 -999 ]{106: RET -1}
[ 42 -999 ]{6: STOP}
```

Fill out:

```txt
Stack Addr | Value | Description
         0 | 42    | value of g
         1 | 6     | Retur adresse fra main.
         2 | -999  | old bp
  bp->   3 | 1234  | value of n
         4 | 3     | value of p = &n
         5 | 0     | pn[0] = &g
         6 | 5     | pointer to first place in array
         7 | 2     | a[0] = 2
         8 | 4     | a[1] = 4
         9 | 7     | pointer to first place in array
```
