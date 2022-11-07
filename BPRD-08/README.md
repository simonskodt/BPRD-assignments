# BPRD-08

Assignment 8 in 'Programmer som data'.

## Exercises

### Exercise 8.1

**Part (i)**

Found in file `SelctionSort.il`.

**Part (ii)**

Found in file `SelectionSort.jvmbytecode`.

### Exercise 8.3

The problem is in the `get()` method. The head pointer is moved to `head.next`, but the old head is never set to null. The first object still points to the head, which will not be garbage collected. The solution is to set the first object to null. We will illustrate this below:
