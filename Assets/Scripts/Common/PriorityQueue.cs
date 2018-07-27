using System;
using System.Collections.Generic;
using System.Diagnostics;

/**
 * Now we implement a priority queue.
 */
class PriorityQueue<T>
{
    IComparer<T> comparer;
    T[] heap;

    public PriorityQueue() : this(null) {}
    public PriorityQueue(int capacity) : this(capacity, null) {}
    public PriorityQueue(IComparer<T> comp) : this(16, comp) {}
    public PriorityQueue(int capacity, IComparer<T> comp) {
        comparer = (comp == null) ? Comparer<T>.Default : comp;
        heap = new T[capacity];
    }

    public int Count { get; private set; }

    public void Push(T v)
    {
        if (Count >= heap.Length)
            Array.Resize(ref heap, Count * 2);

        heap[Count] = v;
        Count++;

        SiftUp(Count);
    }

    public T Pop()
    {
        var v = heap[0];
        heap[0] = heap[--Count];
        if (Count > 0) SiftDown(1);
        return v;
    }

    public T Top()
    {
        if (Count > 0)
            return heap[0];
        else
            throw new InvalidOperationException("PriorityQueue is empty!");
    }

    void SiftDown(int n)
    {
        for (int i = n; i * 2 <= Count; )
        {
            int leftChild = i * 2;
            int rightChild = leftChild < Count ? leftChild + 1 : 0;
            bool moreThanLeft = comparer.Compare(heap[i - 1], heap[leftChild - 1]) > 0;
            bool moreThanRight = rightChild > 0 ? comparer.Compare(heap[i-1], heap[rightChild -1]) > 0 : false;
            if (moreThanLeft && moreThanRight)
            {
                if (comparer.Compare(heap[leftChild - 1], heap[rightChild - 1]) < 0)
                {
                    Swap(ref heap[i - 1], ref heap[leftChild - 1]);
                    i = leftChild;
                }
                else
                {
                    Swap(ref heap[i - 1], ref heap[rightChild - 1]);
                    i = rightChild;
                }
            }
            else if (moreThanLeft)
            {
                Swap(ref heap[i - 1], ref heap[leftChild - 1]);
                i = leftChild;
            }
            else if (moreThanRight)
            {
                Swap(ref heap[i - 1], ref heap[rightChild - 1]);
                i = rightChild;
            }
            else
            {
                break;
            }
        }
    }

    void Swap(ref T l, ref T r)
    {
        T tmp = l;
        l = r;
        r = tmp;
    }

    void SiftUp(int n)
    {
        for (int i = n; i > 1;)
        {
            int parent = i / 2;

            if (comparer.Compare(heap[i - 1], heap[parent - 1]) < 0)
            {
                Swap(ref heap[i-1], ref heap[parent - 1]);
                i = parent;
            }
            else
            {
                break;
            }
        }
    }
}