// Circular queue with generic item type, based on array  
// sestoft@itu.dk * 2009-10-25

using System;

class CircularQueue<T> {
  private readonly T[] items;
  private int count = 0;	// 0 <= count <= capacity
  private int deqAt = 0;	// 0 <= deqAt < capacity

  public CircularQueue(int capacity) {
    this.items = new T[capacity];
  }
  
  public T Dequeue() {
    if (count > 0) {
      count--;
      T result = items[deqAt];
      items[deqAt] = default(T);
      deqAt = (deqAt+1) % items.Length;
      return result;
    } else
      throw new ApplicationException("Queue empty");
  }

  public void Enqueue(T x) {
    if (count < items.Length) {
      items[(deqAt+count)%items.Length] = x;
      count++;
    } else
      throw new ApplicationException("Queue full");
  }
}

class TestCircularQueue {
  public static void Main(String[] args) {
    CircularQueue<double> q = new CircularQueue<double>(2);
    q.Enqueue(1.2);
    q.Enqueue(3.4);
    Console.WriteLine(q.Dequeue());
    q.Enqueue(5.6);
    Console.WriteLine(q.Dequeue());
    Console.WriteLine(q.Dequeue());
  }
}
