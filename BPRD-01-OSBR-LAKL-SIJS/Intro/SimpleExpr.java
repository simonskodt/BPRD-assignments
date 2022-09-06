// File Intro/SimpleExpr.java
// Java representation of expressions as in lecture 1
// sestoft@itu.dk * 2010-08-29

import java.util.Map;
import java.util.HashMap;

abstract class Expr {
  abstract public String toString();
  abstract public int eval(Map<String, Integer> env);
  abstract public Expr simplify();
}

class CstI extends Expr {
  protected final int i;

  public CstI(int i) {
    this.i = i;
  }

  @Override
  public String toString() {
    return String.valueOf(this.i);
  }

  @Override
  public int eval(Map<String, Integer> env) {
    return i;
  }

  @Override
  public Expr simplify() {
    return new CstI(this.i);
  }
}

class Var extends Expr {
  protected final String name;

  public Var(String name) {
    this.name = name;
  }

  @Override
  public String toString() {
    return this.name;
  }

  @Override
  public int eval(Map<String, Integer> env) {
    return env.get(this.name);
  }

  @Override
  public Expr simplify() {
    return new Var(this.name);
  }
}

class Prim extends Expr {
  protected final String oper;
  protected final Expr e1, e2;

  public Prim(String oper, Expr e1, Expr e2) {
    this.oper = oper;
    this.e1 = e1;
    this.e2 = e2;
  }
  
  @Override
  public String toString() {
    return this.e1 + this.oper + this.e2;
  }

  @Override
  public int eval(Map<String, Integer> env) {
    if (oper.equals("+"))
      return e1.eval(env) + e2.eval(env);
    else if (oper.equals("*"))
      return e1.eval(env) * e2.eval(env);
    else if (oper.equals("-"))
      return e1.eval(env) - e2.eval(env);
    else
      throw new RuntimeException("unknown primitive");
  }

  @Override
  public Expr simplify() {
    throw new UnsupportedOperationException();
  }
}

public class SimpleExpr {
  public static void main(String[] args) {
    Expr e1 = new CstI(17);
    Expr e2 = new Prim("+", new CstI(3), new Var("a"));
    Expr e3 = new Prim("+", new Prim("*", new Var("b"), new CstI(9)),
        new Var("a"));
    Map<String, Integer> env0 = new HashMap<String, Integer>();
    env0.put("a", 3);
    env0.put("c", 78);
    env0.put("baf", 666);
    env0.put("b", 111);
    System.out.println(e1.eval(env0));
    System.out.println(e2.eval(env0));
    System.out.println(e3.eval(env0));

    // 1.4(ii)
    Expr e4 = new Add(e1, new CstI(6));
    Expr e5 = new Mul(e1, new CstI(3));
    Expr e6 = new Sub(e1, new CstI(5));
    System.out.println(e4.eval(env0));
    System.out.println(e5.eval(env0));
    System.out.println(e6.eval(env0));

    // 1.4(iv)
    Expr e7 = new Add(new CstI(0), new Var("x"));
    System.out.println(e7.toString());
    Expr e8 = new Mul(new CstI(1), new CstI(5));
    System.out.println(e8.simplify());
    Expr e9 = new Sub(new CstI(5), new CstI(5));
    System.out.println(e9.simplify());
    Expr e10 = new Add(e1, new CstI(0));
    System.out.println(e10.simplify());
  }
}

// 1.4(i)
abstract class Binop extends Expr {
  protected Expr e1, e2;
  protected String oper;

  public Binop(Expr e1, Expr e2, String oper) {
    this.e1 = e1;
    this.e2 = e2;
    this.oper = oper;
  }

  public String toString() {
    return e1.toString() + oper + e1.toString();
  }

  public abstract Expr simplify();
}

class Add extends Binop {
  public Add(Expr e1, Expr e2) {
    super(e1, e2, "+");
  }

  // 1.4(iii)
  @Override
  public int eval(Map<String, Integer> env) {
    return e1.eval(env) + e2.eval(env);
  }

  // 1.4(iv)
  @Override
  public Expr simplify() {
    if (e1.toString().equals("0")) {
      return e2;
    } else if (e2.toString().equals("0")) {
      return e1;
    } else {
      return new Add(e1, e2);
    }
  }
}

class Mul extends Binop {
  public Mul(Expr e1, Expr e2) {
    super(e1, e2, "*");
  }

  @Override
  public int eval(Map<String, Integer> env) {
    return e1.eval(env) * e2.eval(env);
  }

  @Override
  public Expr simplify() {
    if (e1.toString().equals("1")) {
      return e2;
    } else if (e2.toString().equals("1")) {
      return e1;
    } else if (e1.toString().equals("0") || e2.toString().equals("0")) {
      return new CstI(0);
    } else {
      return new Mul(e1, e2);
    }
  }
}

class Sub extends Binop {
  public Sub(Expr e1, Expr e2) {
    super(e1, e2, "-");
  }

  @Override
  public int eval(Map<String, Integer> env) {
    return e1.eval(env) - e2.eval(env);
  }

  @Override
  public Expr simplify() {
    if (e2.toString().equals("0")) {
      return e1;
    } else if (e1.toString().equals(e2.toString())) {
      return new CstI(0);
    } else {
      return new Sub(e1, e2);
    }
  }
}