# Assignment 09

In the following readme, our non-code answers are found.

## Exercise 10.1

### Part (i)

Instructions | Effect |
------------ | ----------- |
`ADD` | Untag the two top elements of the stack, and then add these two together. Tag them, and assign them to the stackpointer minus one and decrement the stackpointer by one. |
`CSTI i` | Tag the next program counter (which is the int constant), and assign it to stackpointer + 1. Afterward, increment the stackpointer. |
`NIL` | Assign 0 to the stackpointer + 1, and increment the stackpointer by one. `NIL` does not influence program state.
`IFZERO` | First, get the `v` value from the stackpointer - 1. Then evaluate whether `v` is true or false (any non-zero value is true). If it evaluates true, assign `pc` to `pc`, if not true, then assign `pc` to `pc+1`.
`CONS` | Allocates a word pointer of size 2. It takes the two top elements from the stack, inserts them into this word. Then it puts this word as the stackpointer-1 element and decrements the stackpointer.
`CAR` | Assign a word pointer to current stackpointer. If pointer equals zero, return minus 1. Otherwise return `p[1]` which is the first word of the block.
`SETCAR` | Assign word `v` to stackpointer-1. Assign a word pointer to current stackpointer. Assign `p[1]` to `v`.

### Part (ii)

Macro | Result |
----- | ------ |
`#define Length(hdr) (((hdr)>>2)&0x003FFFFFFFFFFFFF)` | ```00tt tttt ttnn nnnn nnnn nnnn nnnn nnnn nnnn nnnn nnnn nnnn nnnn nnnn nnnn nngg```<br>`->`<br>```0000 tttt tttt nnnn nnnn nnnn nnnn nnnn nnnn nnnn nnnn nnnn nnnn nnnn nnnn nnnn```<br>`&`<br>```0000 0000 0011 1111 1111 1111 1111 1111 1111 1111 1111 1111 1111 1111 1111 1111``` |
`#define Color(hdr) ((hdr)&3)` | ```00tt tttt ttnn nnnn nnnn nnnn nnnn nnnn nnnn nnnn nnnn nnnn nnnn nnnn nnnn nngg```<br>`&`<br>```0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0011```<br><br>This gives us the two least significant bits, which are used to represent the color in GC. |
`#define Paint(hdr, color) (((hdr)&(~3))\|(color))` | ```00tt tttt ttnn nnnn nnnn nnnn nnnn nnnn nnnn nnnn nnnn nnnn nnnn nnnn nnnn nngg```<br>`&`<br>```0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000```<br>`=`<br>```0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000```<br>`\|`<br>```color```<br><br>This allows us to delete the current color of the header, and apply (or "paint") our own color.

### Part (iii)

`allocate` is only called in the `CONS` instruction

```c
    case CONS: {
      word* p = allocate(CONSTAG, 2, s, sp);
      p[1] = (word)s[sp - 1];
      p[2] = (word)s[sp];
      s[sp - 1] = (word)p;
      sp--;
    } break;
```

### Part (iv)

`collect` is called in the `allocate` function if there is no free space.

## Exercise 10.2
Can be seen in file `listmachine.c`

Also copied here, because `listmachine.c` is modified in next exercise.

```c
void mark(word *p)
{
  if (inHeap(p))
  {
    p[0] = Paint(p[0], White);
    int i;
    for (i = 1; i <= Length(p[0]); i++)
      mark((word *)p[i]);
  }
}

void markPhase(word s[], word sp)
{
  for (int i = 0; i <= sp; i++)
  {
    if (inHeap((word *)s[i]))
    {
      mark((word *)s[i]);
    }
  }
}

void sweepPhase()
{
  word *p = heap;
  word *prev = 0;

  while (p < afterHeap)
  {

    // If block is black, paint white
    if (Color(p[0]) == Black)
    {
      p[0] = Paint(p[0], White);
      prev = p;
    }

    // If block is black, paint blue
    if (Color(p[0]) == White)
    {
      p[0] = Paint(p[0], Blue);

      p[1] = (word) freelist;
      freelist = p;

      prev = p;
    }

    p = p + Length(p[0]) + 1; // at last in while loop, point p to next block in heap.
  }
}
```

## Exercise 10.3

Can be seen in file `listmachine.c`.
