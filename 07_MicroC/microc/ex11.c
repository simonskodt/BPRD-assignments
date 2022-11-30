// micro-C example 11 -- n queens problem * 1996-12-20, 2009-10-01

// Finding and printing the 2680 solutions to the 11 queens problem
// takes 8.1 secs when executed using the micro-C interpreter "eval"
// written in F#, when running on a 2.5 GHz Intel i7.

// It takes 0.960 sec when the micro-C program is compiled
// (unoptimized) to bytecode and interpreted on the Machine.java
// bytecode interpreter run on a 2.5 GHz Intel i7; and 0.790 sec when
// interpreted on the machine.c bytecode interpreter.

// It takes 0.110 sec when the micro-C program is compiled
// (unoptimized) to (inefficient) assembly code, on the 2.5 GHz i7.


void main(int n) {
  int i; 
  int u;
  int used[100];
  int diag1[100];
  int diag2[100];
  int col[100];

  u = 1;
  while (u <= n) {
    used[u] = false;
    u = u+1;
  }

  u = 1;
  while (u <= 2 * n) {
    diag1[u] = diag2[u] = false;
    u = u+1;
  }

  i = u = 1;
  while (i > 0) {
    while (i <= n && i != 0) {
      while (u <= n && (used[u] || diag1[u-i+n] || diag2[u+i]))
	u = u + 1;
      if (u <= n) { // not used[u]; fill col[i] then try col[i+1]
	col[i] = u; 
	diag1[u-i+n] = diag2[u+i] = used[u] = true; 
	i = i+1; u = 1;
      } else {			// backtrack; try to find a new col[i-1]
	i = i-1;
	if (i > 0) { 
	  u = col[i]; 
	  diag1[u-i+n] = diag2[u+i] = used[u] = false; 
	  u = u+1;
	} 
      }
    }

    if (i > n) {                // output solution, then backtrack
      int j;
      j = 1;
      while (j <= n) {
	print col[j];  
	j = j+1;
      }
      println;
      i = i-1; 
      if (i > 0) { 
	u = col[i]; 
	diag1[u-i+n] = diag2[u+i] = used[u] = false; 
	u = u+1;
      }
    }
  }
}
