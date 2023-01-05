void main() {
    print (0 within [print 1,print 2]); // Expected output: 1 2 0
    print (3 within [print 1,print 2]); // Expected output: 1 2 0
    print (print 42 within [print 40,print 44]); // Expected output: 40 44 1 1
    print ((print 42) within [print 40,print 44]); // Expected output: 42 40 44 1
}