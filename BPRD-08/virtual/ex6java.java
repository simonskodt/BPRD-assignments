// virtual/ex6java.java: Doubly-linked lists in Java

class ex6java extends Object {
  public static void main(String[] args) {
    LinkedList lst;
    lst = new LinkedList();
    lst.addLast(5);
    lst.addLast(7);
    Node node;
    node = lst.first;
  }
}

class Node extends Object {
  Node next;
  Node prev;
  int item;
}

class LinkedList extends Object {
  Node first;
  Node last;		// Invariant: first==null iff last==null

  void addLast(int item) {
    Node node;
    node = new Node();
    node.item = item;
    if (this.last == null) {
      this.first = node;
      this.last = node;
    } else {
      this.last.next = node;
      node.prev = this.last;
      this.last = node;
    }
  }

  void printForwards() {
    Node node;
    node = this.first;
    while (node != null) {
      InOut.print(node.item);
      node = node.next;
    }
  }

  void printBackwards() {
    Node node;
    node = this.last;
    while (node != null) {
      InOut.print(node.item);
      node = node.prev;
    }
  }
}

// InOut.java -- Definitions of print primitives

class InOut {
  public static void print(Object o) { 
    System.out.println(o); 
  }

  public static void print(int i) { 
    System.out.println(i); 
  }
}

