#include <sys/time.h>
#include <sys/resource.h>

int getUserTimeMs() {
  struct rusage ru;
  getrusage(RUSAGE_SELF, &ru);
  struct timeval t = ru.ru_utime;
  return (t.tv_sec * 1000 + t.tv_usec / 1000);
}

word* readfile(char* filename) {
  int capacity = 1, size = 0;
  word *program = (word*)malloc(sizeof(word)*capacity); 
  FILE *inp = fopen(filename, "r");
  if (inp == NULL) {
    printf("File %s not found.\n", filename);
    exit(-1);
  }
  int instr;
  while (fscanf(inp, "%d", &instr) == 1) {
    if (size >= capacity) { 
      word* buffer = (word*)malloc(sizeof(word) * 2 * capacity);
      int i;
      for (i=0; i<capacity; i++)
        buffer[i] = program[i];
      free(program);
      program = buffer;
      capacity *= 2;
    }
    program[size++] = instr;
  }
  fclose(inp);
  return program;
}

