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

After instantiating the `SentinelLockQueue`:

<p align="center">
    <img src=setup.png />
</p>

After calling `put(i)`:

<p align="center">
    <img src=put.png />
</p>

After calling `get()`:

<p align="center">
    <img src=get.png />
</p>

Here, the dummy `Node` is not referred to from the `SentinelLockQueue`, which should lead the the dummy node being garbage collected. However, since the dummy node is not explicitly set to `null`, then it is unnecessarily maintained.
