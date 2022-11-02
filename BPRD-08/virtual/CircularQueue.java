// Circular queue with generic item type, based on array  
// sestoft@itu.dk * 2009-10-25

import java.util.ArrayList;

class CircularQueue<T> {
  private final ArrayList<T> items;
  private int count = 0;	// 0 <= count <= capacity
  private int deqAt = 0;	// 0 <= deqAt < capacity

  public CircularQueue(int capacity) {
    this.items = new ArrayList<T>(capacity);
    for (int i=0; i<capacity; i++)
      this.items.add(null);
  }
  
  public T dequeue() {
    if (count > 0) {
      count--;
      T result = items.get(deqAt);
      items.set(deqAt, null);
      deqAt = (deqAt+1) % items.size();
      return result;
    } else
      throw new RuntimeException("Queue empty");
  }

  public void enqueue(T x) {
    if (count < items.size()) {
      items.set((deqAt+count)%items.size(), x);
      count++;
    } else
      throw new RuntimeException("Queue full");
  }


  public static void main(String[] args) {
    CircularQueue<Double> q = new CircularQueue<Double>(2);
    q.enqueue(1.2);
    q.enqueue(3.4);
    System.out.println(q.dequeue());
    q.enqueue(5.6);
    System.out.println(q.dequeue());
    System.out.println(q.dequeue());
  }
}
