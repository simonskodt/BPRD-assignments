// micro-C example on alias for exam 2021
// Based on example ex3.c
void main(int n)
{
    int i;
    i = 0;
    alias j as i; // Make local variable j an alias of local variable i
    while (j < n)
    {
        print j;
        i = i + 1;
    }
}