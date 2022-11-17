/* File ListC/ListVM/ListVM/listmachine.c
A unified-stack abstract machine and garbage collector
for list-C, a variant of micro-C with cons cells.
sestoft@itu.dk * 2009-11-17, 2012-02-08

nh@itu.dk 2019-10-21: Created 64 bit support where the tagging is
unchanged 32 bits fitting the book. The additional 32 bits are simply
not used

nh@itu.dk 2020-11-01: Modified 64 bit version to utilize all 64 bit by
extending the length part of the tag to use 54 bits.

Compile on Unix like system:
gcc -Wall listmachine.c -o listmachine

To execute a program file using this abstract machine, do:
./listmachine <programfile> <arg1> <arg2> ...
To get also a trace of the program execution:
./listmachine -trace <programfile> <arg1> <arg2> ...

This code assumes -- and checks -- that values of type
long, unsigned long and unsigned long* have size 64 bits.
*/

/*
Data representation in the stack s[...] and the heap:
* Integers are tagged with a 1 bit in the least significant
position, regardless of whether they represent program integers,
return addresses, array base addresses or old base pointers
(into the stack).
* All heap references are word-aligned, that is, the two least
significant bits of a heap reference are 00.
* Integer constants and code addresses in the program array
p[...] are not tagged.
The distinction between integers and references is necessary for
the garbage collector to be precise (not conservative).

The heap consists of 64-bit words, and the heap is divided into
blocks.  A block has a one-word header block[0] followed by the
block's contents: zero or more words block[i], i=1..n.

A header has the form for32 bits
ttttttttnnnnnnnnnnnnnnnnnnnnnnnngg
and for 64 bits
ttttttttnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnngg
where
  - tttttttt is the block tag, all 0 for cons cells
  - nn....nn is the block length (excluding header). 22 bits for 32 bit
             and 54 bits for 64 bit.
  - gg       is the block's color

The block color has this meaning:
gg=00=White: block is dead (after mark, before sweep)
gg=01=Grey:  block is live, children not marked (during mark)
gg=11=Black: block is live (after mark, before sweep)
gg=11=Blue:  block is on the freelist or orphaned

A block of length zero is an orphan; it cannot be used
for data and cannot be on the freelist.  An orphan is
created when allocating all but the last word of a free block.
*/

#include <stdlib.h>
#include <string.h>
#include <stdio.h>
#include <time.h>

// Check Windows
#if _WIN32 || _WIN64
#define WIN
#endif

// Check if compiled with gcc
#if __GNUC__
#define GCC
#endif

#ifdef GCC
#define PPCOMP "Compiled with gcc."
#else
#define PPCOMP "Not compiled with gcc."
#endif

#if _WIN64 || __x86_64__ || __ppc64__
#define ENV64
#define PPARCH "64 bit architecture."
#else
#define ENV32
#define PPARCH "32 bit architecture."
#endif

#if defined(ENV32)
typedef int word;
typedef unsigned int uword;
#elif defined(ENV64)
typedef long long word;
typedef unsigned long long uword;
#else
#error "Error: Do not know if 32 or 64 bit"
#endif

// Get the user time in milli-seconds
int getUserTime();

// Read instructions from a file, return array of instructions
word *readfile(char *filename);

#if defined(ENV32)
#define WORD_FMT "%d"
#define UWORD_FMT "%u"
#elif defined(_WIN64)
#define WORD_FMT "%I64d"
#define UWORD_FMT "%I64u"
#else
#define WORD_FMT "%lld"
#define UWORD_FMT "%llu"
#endif

#if defined(WIN)
#include "utils_win.c"
#else
#include "utils_unix.c"
#endif

#if defined(ENV32)
#define IsInt(v) (((v)&1) == 1)
#define Tag(v) (((v) << 1) | 1)
#define Untag(v) ((v) >> 1)
#elif defined(ENV64)
#define IsInt(v) (((v)&1) == 1)
#define Tag(v) (((v) << 1) | 1)
#define Untag(v) ((v) >> 1)
#endif

#define White 0
#define Grey 1
#define Black 2
#define Blue 3

#if defined(ENV32)
#define BlockTag(hdr) ((hdr) >> 24)
#define Length(hdr) (((hdr) >> 2) & 0x003FFFFF)
#define Color(hdr) ((hdr)&3)
#define Paint(hdr, color) (((hdr) & (~3)) | (color))
#elif defined(ENV64)
#define BlockTag(hdr) (((hdr) >> 56))
#define Length(hdr) (((hdr) >> 2) & 0x003FFFFFFFFFFFFF)
#define Color(hdr) ((hdr)&3)
#define Paint(hdr, color) (((hdr) & (0xFFFFFFFFFFFFFFFC)) | (color))
#endif

#define CONSTAG 0

// Heap size in words

#define HEAPSIZE 10000

word *heap;
word *afterHeap;
word *freelist;

// These numeric instruction codes must agree with ListC/Machine.fs:
// (Use #define because const int does not define a constant in C)

#define CSTI 0
#define ADD 1
#define SUB 2
#define MUL 3
#define DIV 4
#define MOD 5
#define EQ 6
#define LT 7
#define NOT 8
#define DUP 9
#define SWAP 10
#define LDI 11
#define STI 12
#define GETBP 13
#define GETSP 14
#define INCSP 15
#define GOTO 16
#define IFZERO 17
#define IFNZRO 18
#define CALL 19
#define TCALL 20
#define RET 21
#define PRINTI 22
#define PRINTC 23
#define LDARGS 24
#define STOP 25
#define NIL 26
#define CONS 27
#define CAR 28
#define CDR 29
#define SETCAR 30
#define SETCDR 31

#define STACKSIZE 1000

// Print the stack machine instruction at p[pc]

void printInstruction(word p[], word pc)
{
  switch (p[pc])
  {
  case CSTI:
    printf("CSTI " WORD_FMT, p[pc + 1]);
    break;
  case ADD:
    printf("ADD");
    break;
  case SUB:
    printf("SUB");
    break;
  case MUL:
    printf("MUL");
    break;
  case DIV:
    printf("DIV");
    break;
  case MOD:
    printf("MOD");
    break;
  case EQ:
    printf("EQ");
    break;
  case LT:
    printf("LT");
    break;
  case NOT:
    printf("NOT");
    break;
  case DUP:
    printf("DUP");
    break;
  case SWAP:
    printf("SWAP");
    break;
  case LDI:
    printf("LDI");
    break;
  case STI:
    printf("STI");
    break;
  case GETBP:
    printf("GETBP");
    break;
  case GETSP:
    printf("GETSP");
    break;
  case INCSP:
    printf("INCSP " WORD_FMT, p[pc + 1]);
    break;
  case GOTO:
    printf("GOTO " WORD_FMT, p[pc + 1]);
    break;
  case IFZERO:
    printf("IFZERO " WORD_FMT, p[pc + 1]);
    break;
  case IFNZRO:
    printf("IFNZRO " WORD_FMT, p[pc + 1]);
    break;
  case CALL:
    printf("CALL " WORD_FMT " " WORD_FMT, p[pc + 1], p[pc + 2]);
    break;
  case TCALL:
    printf("TCALL " WORD_FMT " " WORD_FMT " " WORD_FMT,
           p[pc + 1], p[pc + 2], p[pc + 3]);
    break;
  case RET:
    printf("RET " WORD_FMT, p[pc + 1]);
    break;
  case PRINTI:
    printf("PRINTI");
    break;
  case PRINTC:
    printf("PRINTC");
    break;
  case LDARGS:
    printf("LDARGS");
    break;
  case STOP:
    printf("STOP");
    break;
  case NIL:
    printf("NIL");
    break;
  case CONS:
    printf("CONS");
    break;
  case CAR:
    printf("CAR");
    break;
  case CDR:
    printf("CDR");
    break;
  case SETCAR:
    printf("SETCAR");
    break;
  case SETCDR:
    printf("SETCDR");
    break;
  default:
    printf("<unknown>");
    break;
  }
}

// Print current stack (marking heap references by #) and current instruction

void printStackAndPc(word s[], word bp, word sp, word p[], word pc)
{
  printf("[ ");
  word i;
  for (i = 0; i <= sp; i++)
    if (IsInt(s[i]))
      printf(WORD_FMT " ", Untag(s[i]));
    else
      printf("#" WORD_FMT " ", s[i]);
  printf("]");
  printf("{" WORD_FMT ":", pc);
  printInstruction(p, pc);
  printf("}\n");
}

word *allocate(unsigned int tag, uword length, word s[], word sp);

// The machine: execute the code starting at p[pc]

int execcode(word p[], word s[], word iargs[], int iargc, int /* boolean */ trace)
{

  word bp = -999; // Base pointer, for local variable access
  word sp = -1;   // Stack top pointer
  word pc = 0;    // Program counter: next instruction
  for (;;)
  {
    if (trace)
      printStackAndPc(s, bp, sp, p, pc);
    switch (p[pc++])
    {
    case CSTI:
      s[sp + 1] = Tag(p[pc++]);
      sp++;
      break;
    case ADD:
      s[sp - 1] = Tag(Untag(s[sp - 1]) + Untag(s[sp]));
      sp--;
      break;
    case SUB:
      s[sp - 1] = Tag(Untag(s[sp - 1]) - Untag(s[sp]));
      sp--;
      break;
    case MUL:
      s[sp - 1] = Tag(Untag(s[sp - 1]) * Untag(s[sp]));
      sp--;
      break;
    case DIV:
      s[sp - 1] = Tag(Untag(s[sp - 1]) / Untag(s[sp]));
      sp--;
      break;
    case MOD:
      s[sp - 1] = Tag(Untag(s[sp - 1]) % Untag(s[sp]));
      sp--;
      break;
    case EQ:
      s[sp - 1] = Tag(s[sp - 1] == s[sp] ? 1 : 0);
      sp--;
      break;
    case LT:
      s[sp - 1] = Tag(s[sp - 1] < s[sp] ? 1 : 0);
      sp--;
      break;
    case NOT:
    {
      word v = s[sp];
      s[sp] = Tag((IsInt(v) ? Untag(v) == 0 : v == 0) ? 1 : 0);
    }
    break;
    case DUP:
      s[sp + 1] = s[sp];
      sp++;
      break;
    case SWAP:
    {
      word tmp = s[sp];
      s[sp] = s[sp - 1];
      s[sp - 1] = tmp;
    }
    break;
    case LDI: // load indirect
      s[sp] = s[Untag(s[sp])];
      break;
    case STI: // store indirect, keep value on top
      s[Untag(s[sp - 1])] = s[sp];
      s[sp - 1] = s[sp];
      sp--;
      break;
    case GETBP:
      s[sp + 1] = Tag(bp);
      sp++;
      break;
    case GETSP:
      s[sp + 1] = Tag(sp);
      sp++;
      break;
    case INCSP:
      sp = sp + p[pc++];
      break;
    case GOTO:
      pc = p[pc];
      break;
    case IFZERO:
    {
      word v = s[sp--];
      pc = (IsInt(v) ? Untag(v) == 0 : v == 0) ? p[pc] : pc + 1;
    }
    break;
    case IFNZRO:
    {
      word v = s[sp--];
      pc = (IsInt(v) ? Untag(v) != 0 : v != 0) ? p[pc] : pc + 1;
    }
    break;
    case CALL:
    {
      word argc = p[pc++];
      int i;
      for (i = 0; i < argc; i++)   // Make room for return address
        s[sp - i + 2] = s[sp - i]; // and old base pointer
      s[sp - argc + 1] = Tag(pc + 1);
      sp++;
      s[sp - argc + 1] = Tag(bp);
      sp++;
      bp = sp + 1 - argc;
      pc = p[pc];
    }
    break;
    case TCALL:
    {
      word argc = p[pc++]; // Number of new arguments
      word pop = p[pc++];  // Number of variables to discard
      word i;
      for (i = argc - 1; i >= 0; i--) // Discard variables
        s[sp - i - pop] = s[sp - i];
      sp = sp - pop;
      pc = p[pc];
    }
    break;
    case RET:
    {
      word res = s[sp];
      sp = sp - p[pc];
      bp = Untag(s[--sp]);
      pc = Untag(s[--sp]);
      s[sp] = res;
    }
    break;
    case PRINTI:
      printf(WORD_FMT " ", IsInt(s[sp]) ? Untag(s[sp]) : s[sp]);
      break;
    case PRINTC:
      printf("%c", (char)Untag(s[sp]));
      break;
    case LDARGS:
    {
      int i;
      for (i = 0; i < iargc; i++) // Push commandline arguments
        s[++sp] = Tag(iargs[i]);
    }
    break;
    case STOP:
      return 0;
    case NIL:
      s[sp + 1] = 0;
      sp++;
      break;
    case CONS:
    {
      word *p = allocate(CONSTAG, 2, s, sp);
      p[1] = (word)s[sp - 1];
      p[2] = (word)s[sp];
      s[sp - 1] = (word)p;
      sp--;
    }
    break;
    case CAR:
    {
      word *p = (word *)s[sp];
      if (p == 0)
      {
        printf("Cannot take car of null\n");
        return -1;
      }
      s[sp] = (word)(p[1]);
    }
    break;
    case CDR:
    {
      word *p = (word *)s[sp];
      if (p == 0)
      {
        printf("Cannot take cdr of null\n");
        return -1;
      }
      s[sp] = (word)(p[2]);
    }
    break;
    case SETCAR:
    {
      word v = (word)s[sp--];
      word *p = (word *)s[sp];
      p[1] = v;
    }
    break;
    case SETCDR:
    {
      word v = (word)s[sp--];
      word *p = (word *)s[sp];
      p[2] = v;
    }
    break;
    default:
      printf("Illegal instruction " WORD_FMT " at address " WORD_FMT "\n",
             p[pc - 1], pc - 1);
      return -1;
    }
  }
}

// Read program from file, and execute it

int execute(int argc, char **argv, int /* boolean */ trace)
{
  word *p = readfile(argv[trace ? 2 : 1]);            // program bytecodes: int[]
  word *s = (word *)malloc(sizeof(word) * STACKSIZE); // stack: int[]
  int iargc = trace ? argc - 3 : argc - 2;
  word *iargs = (word *)malloc(sizeof(word) * iargc); // program inputs: int[]
  int i;
  for (i = 0; i < iargc; i++) // Convert commandline arguments
    iargs[i] = (word)atoi(argv[trace ? i + 3 : i + 2]);
  // Measure cpu time for executing the program

  int t1 = getUserTimeMs();
  int res = execcode(p, s, iargs, iargc, trace); // Execute program proper
  int t2 = getUserTimeMs();
  int runtime = t2 - t1;
  printf("\nUsed %d cpu milli-seconds\n", runtime);

  return res;
}

// Garbage collection and heap allocation

word mkheader(uword tag, uword length, unsigned int color)
{
#if defined(ENV32)
  return (tag << 24) | (length << 2) | color;
#elif defined(ENV64)
  return (tag << 56) | (length << 2) | color;
#endif
}

int inHeap(word *p)
{
  return heap <= p && p < afterHeap;
}

// Call this after a GC to get heap statistics:

void heapStatistics()
{
  word blocks = 0, free = 0, orphans = 0,
       blocksSize = 0, freeSize = 0, largestFree = 0;
  word *heapPtr = heap;
  while (heapPtr < afterHeap)
  {
    if (Length(heapPtr[0]) > 0)
    {
      blocks++;
      blocksSize += Length(heapPtr[0]);
    }
    else
      orphans++;
    word *nextBlock = heapPtr + Length(heapPtr[0]) + 1;
    if (nextBlock > afterHeap)
    {
      printf("HEAP ERROR: block at heap[" WORD_FMT "] extends beyond heap\n",
             (word)(heapPtr - heap));
      exit(-1);
    }
    heapPtr = nextBlock;
  }
  word *freePtr = freelist;
  while (freePtr != 0)
  {
    free++;
    int length = Length(freePtr[0]);
    if (freePtr < heap || afterHeap < freePtr + length + 1)
    {
      printf("HEAP ERROR: freelist item " WORD_FMT " (at heap[" WORD_FMT "], length %d) is outside heap\n",
             free, (word)(freePtr - heap), length);
      exit(-1);
    }
    freeSize += length;
    largestFree = length > largestFree ? length : largestFree;
    if (Color(freePtr[0]) != Blue)
      printf("Non-blue block at heap[" UWORD_FMT "] on freelist\n", (uword)freePtr);
    freePtr = (word *)freePtr[1];
  }
  printf("Heap: " WORD_FMT " blocks (" WORD_FMT " words); of which " WORD_FMT " free (" WORD_FMT " words, largest " WORD_FMT " words); " WORD_FMT " orphans\n",
         blocks, blocksSize, free, freeSize, largestFree, orphans);
}

void initheap()
{
  heap = (word *)malloc(sizeof(word) * HEAPSIZE);
  afterHeap = &heap[HEAPSIZE];
  // Initially, entire heap is one block on the freelist:
  heap[0] = mkheader(0, HEAPSIZE - 1, Blue);
  heap[1] = (word)0;
  freelist = &heap[0];
}

void printHeap()
{
  word *heapPtr = heap;
  while (heapPtr < afterHeap)
  {
    printf("Color at heap block: %lld\n", Color(heapPtr[0]));
    word *nextBlock = heapPtr + Length(heapPtr[0]) + 1;
    heapPtr = nextBlock;
  }
}

void printFreeList()
{
  word *freePtr = freelist;
  while (freePtr != 0)
  {
    int length = Length(freePtr[0]);

    printf("Color of freelist block %lld\n", Color(freePtr[0]));
    freePtr = (word *)freePtr[1];
  }
}

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

    if (Color(p[0]) == White)
    {

      int l = Length(p[0]);
      while (1)
      {
        // check for adjacent free blocks
        if (p[l + 1] == White)
        {
          l += Length(p[l + 1]);
        }
        else
        {
          break;
        }
      }

      p[0] = mkheader(BlockTag(p[0]), l, Blue);

      // Make blue block point to freelist location, and freelist point at p.
      // Essentially puts p in front of the existing freelist.
      p[1] = (word)freelist;
      freelist = p;

      prev = p;
    }

    p = p + Length(p[0]) + 1; // at last in while loop, point p to next block in heap.
  }
}

void collect(word s[], word sp)
{
  markPhase(s, sp);
  // printf("\nfirst heap\n");
  // printHeap();
  // printf("\nfirst freelist\n");
  // printFreeList();
  heapStatistics();

  sweepPhase();
  // printf("\nsecond\n");
  // printHeap();
  // printf("\nsecond freelist\n");
  // printFreeList();
  heapStatistics();
}

word *allocate(unsigned int tag, uword length, word s[], word sp)
{
  int attempt = 1;
  do
  {
    word *free = freelist;
    word **prev = &freelist;
    while (free != 0)
    {
      word rest = Length(free[0]) - length;
      if (rest >= 0)
      {
        if (rest == 0) // Exact fit with free block
          *prev = (word *)free[1];
        else if (rest == 1)
        { // Create orphan (unusable) block
          *prev = (word *)free[1];
          free[length + 1] = mkheader(0, rest - 1, Blue);
        }
        else
        { // Make previous free block point to rest of this block
          *prev = &free[length + 1];
          free[length + 1] = mkheader(0, rest - 1, Blue);
          free[length + 2] = free[1];
        }
        free[0] = mkheader(tag, length, White);
        return free;
      }
      prev = (word **)&free[1];
      free = (word *)free[1];
    }
    // No free space, do a garbage collection and try again
    if (attempt == 1)
      collect(s, sp);
  } while (attempt++ == 1);
  printf("Out of memory\n");
  exit(1);
}

// Read code from file and execute it

int main(int argc, char **argv)
{
#if defined(ENV64)
  if (sizeof(word) != 8 ||
      sizeof(word *) != 8 ||
      sizeof(uword) != 8)
  {
    printf("Size of word, word* is not 64 bit, cannot run\n");
    return -1;
  }
#elif defined(ENV32)
  if (sizeof(word) != 4 ||
      sizeof(word *) != 4 ||
      sizeof(uword) != 4)
  {
    printf("Size of word, word* is not 32 bit, cannot run\n");
    return -1;
  }
#else
  printf("Size of word, word* is neither 32 nor 64 bit, cannot run\n");
  return -1;
#endif

  if (argc < 2)
  {
    printf("listmachine for " PPARCH "\n");
    printf(PPCOMP "\n");
    printf("Usage: listmachine [-trace] <programfile> <arg1> ...\n");
    return -1;
  }
  else
  {
    int trace = argc >= 3 && 0 == strncmp(argv[1], "-trace", 7);
    initheap();
    return execute(argc, argv, trace);
  }
}
