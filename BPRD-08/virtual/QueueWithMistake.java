// Lock-based queue with memory management mistake
// sestoft@itu.dk * 2013-10-20

// The SentinelLockQueue is a simple lock-based concurrent queue, or
// first-in first-out buffer, implemented as a linked list of Nodes.
// Method queue.put(x) inserts item x and method queue.get() removes
// and returns an item provided the queue is not empty.

// The queue implementation in class SentinelLockQueue has an extra
// "sentinel" Node so that the head and tail fields always have a Node
// to point to, even when the queue is empty.  This means that the
// first integer item in the queue is not head.item but head.next.item.

// The queue implementation contains a programming mistake that
// usually causes the test program to run out of memory quite soon
// although this should not happen.  The mistake has nothing to do
// with concurrency or threads (but I did not know that when I spent
// several hours staring at it some years ago).

// The mistake actually appears in an example in Goetz et al: Java
// Concurrency in Practice, 2006, page 334 (ch 15) and the book's
// errata and online source code do not correct it.  But in all other
// respects it is an extremely useful and recommendable book!

class QueueWithMistake {
  public static void main(String[] args) {
    for (int threads=1; threads<20; threads++) 
      runThreads(threads, new SentinelLockQueue());
  }

  private static void runThreads(final int threads, final Queue queue) {
    final int iterations = 200000000; // Increase this constant if program does not run out of memory.
    final Timer timer = new Timer();
    Thread[] ts = new Thread[threads];
    queue.put(-6);
    queue.put(-5);
    queue.put(-4);
    queue.put(-3);
    queue.put(-2);
    queue.put(-1);
    for (int j=0; j<threads; j++)
      ts[j] = new Thread() {
          public void run() {
            for (int i=0; i<iterations; i++) {
              queue.put(i);
              queue.get();
            }
          }
        };
    for (int j=0; j<threads; j++)
      ts[j].start();
    try {
      for (int j=0; j<threads; j++)
	  ts[j].join();  // Wait for thread j to terminate.
    } catch (Exception exn) {
      System.out.println(exn);
    }
    System.out.printf("%-20s\t%4d\t%7.2f\t%s%n", 
                      queue.getClass().getName(), threads, timer.Check(), queue.get());
  }
}

interface Queue {
  boolean put(int item);
  int get();
}

// --------------------------------------------------
// Locking queue, with sentinel (dummy) node

class SentinelLockQueue implements Queue {  
  // With sentinel (dummy) node.
  // Invariants:
  //  * The node referred by tail is reachable from head.
  //  * If non-empty then head != tail, 
  //     and tail points to last item, and head.next to first item.
  //  * If empty then head == tail.

  private static class Node {
    final int item;
    volatile Node next;
    
    public Node(int item, Node next) {
      this.item = item;
      this.next = next;
    }
  }

  private final Node dummy = new Node(-444, null);
  private Node head = dummy, tail = dummy;
  
  public synchronized boolean put(int item) {
    Node node = new Node(item, null);
    tail.next = node;
    tail = node;
    return true;
  }

  public synchronized int get() {
    if (head.next == null) 
      return -999;
    Node first = head;
    head = first.next;
    return head.item;
  }
}

// Crude timing utility ----------------------------------------
   
class Timer {
  private long start, spent = 0;
  public Timer() { Play(); }
  public double Check() { return (System.nanoTime()-start+spent)/1E9; }
  public void Pause() { spent += System.nanoTime()-start; }
  public void Play() { start = System.nanoTime(); }
}
