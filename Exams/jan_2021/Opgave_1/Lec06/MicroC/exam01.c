// micro-C example 01 for January 2021 exam
// Testing conditional expression as lvalue.
void main()
{
    int x;
    x = 1;
    int y;
    y = 2;
    (x < y ? x : y) = 3;
    (x < y ? x : y) = 4;
    print x;
    println; // Expected 3
    print y;
    println; // Expected 4
}