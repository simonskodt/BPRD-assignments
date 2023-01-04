// micro-C example 26

void main(int n) {    // Pass 1
  int i;              // (BDec code to allocate i, i->loc i + varenv) 
  i=0;                // (BStmt i=0, varenv)
  print i;            // (BStmt print i, varenv)
  int j;              // (BDec code to allocate j, j->loc j + varenv)
  j=42;               // (BStmt j=42, varenv)
  print j;            // (BStmt print j, varenv)
  println;            // (BStmt println, varenv)
}
