#include <windows.h>

int getUserTimeMs() {
  HANDLE hProcess = GetCurrentProcess();
  FILETIME ftCreation, ftExit, ftKernel, ftUser;
  SYSTEMTIME stUser;
  GetProcessTimes(hProcess, &ftCreation, &ftExit, &ftKernel, &ftUser);
  FileTimeToSystemTime(&ftUser, &stUser);
  int result = stUser.wSecond * 1000 + stUser.wMilliseconds;
  return(result);
}

word* readfile(char* filename) {
  int capacity = 1, size = 0;
  word *program = (word*)malloc(sizeof(word)*capacity);
  FILE *inp;
  fopen_s(&inp, filename, "r");
  if (inp == NULL) {
    printf("File %s not found.\n", filename);
    exit(-1);
  }
  int instr;
  while (fscanf_s(inp, "%d", &instr) == 1) {
    if (size >= capacity) {
      word* buffer = (word*)malloc(sizeof(word) * 2 * capacity);
      int i;
      for (i = 0; i < capacity; i++)
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
