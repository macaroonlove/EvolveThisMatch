using System;
using System.Collections.Generic;

namespace FrameWork
{
    public class PriorityQueue<T>
    {
        private List<(T item, float priority)> heap = new List<(T, float)>();

        public int Count => heap.Count;

        public void Enqueue(T item, float priority)
        {
            heap.Add((item, priority));
            HeapifyUp(heap.Count - 1);
        }

        public T Dequeue()
        {
            if (heap.Count == 0) throw new InvalidOperationException("큐가 비어있음!");

            T item = heap[0].item;
            heap[0] = heap[^1];
            heap.RemoveAt(heap.Count - 1);
            HeapifyDown(0);

            return item;
        }

        private void HeapifyUp(int index)
        {
            while (index > 0)
            {
                int parent = (index - 1) / 2;
                if (heap[parent].priority <= heap[index].priority) break;

                (heap[parent], heap[index]) = (heap[index], heap[parent]);
                index = parent;
            }
        }

        private void HeapifyDown(int index)
        {
            int lastIndex = heap.Count - 1;
            while (true)
            {
                int left = 2 * index + 1;
                int right = 2 * index + 2;
                int smallest = index;

                if (left <= lastIndex && heap[left].priority < heap[smallest].priority) smallest = left;
                if (right <= lastIndex && heap[right].priority < heap[smallest].priority) smallest = right;
                if (smallest == index) break;

                (heap[index], heap[smallest]) = (heap[smallest], heap[index]);
                index = smallest;
            }
        }
    }
}