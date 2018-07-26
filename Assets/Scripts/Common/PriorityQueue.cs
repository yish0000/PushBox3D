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

namespace Skyiv.Util
{
    class PriorityQueue<T>
    {
        IComparer<T> comparer;
        T[] heap;

        public int Count { get; private set; }

        public PriorityQueue() : this(null) { }
        public PriorityQueue(int capacity) : this(capacity, null) { }
        public PriorityQueue(IComparer<T> comparer) : this(16, comparer) { }

        public PriorityQueue(int capacity, IComparer<T> comparer)
        {
            this.comparer = (comparer == null) ? Comparer<T>.Default : comparer;
            this.heap = new T[capacity];
        }

        public void Push(T v)
        {
            if (Count >= heap.Length) Array.Resize(ref heap, Count * 2);
            heap[Count] = v;
            SiftUp(Count++);
        }

        public T Pop()
        {
            var v = Top();
            heap[0] = heap[--Count];
            if (Count > 0) SiftDown(0);
            return v;
        }

        public T Top()
        {
            if (Count > 0) return heap[0];
            throw new InvalidOperationException("优先队列为空");
        }

        void SiftUp(int n)
        {
            var v = heap[n];
            for (var n2 = n / 2; n > 0 && comparer.Compare(v, heap[n2]) > 0; n = n2, n2 /= 2) heap[n] = heap[n2];
            heap[n] = v;
        }

        void SiftDown(int n)
        {
            var v = heap[n];
            for (var n2 = n * 2; n2 < Count; n = n2, n2 *= 2)
            {
                if (n2 + 1 < Count && comparer.Compare(heap[n2 + 1], heap[n2]) > 0) n2++;
                if (comparer.Compare(v, heap[n2]) >= 0) break;
                heap[n] = heap[n2];
            }
            heap[n] = v;
        }
    }
}