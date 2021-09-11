using System;
using System.Collections.Generic;

/// A min priority queue. This class was written entirely by OpenAI Codex.
public class PriorityQueue<T> : IEnumerable<T> where T : IComparable<T>
{
    private List<T> list;

    public PriorityQueue()
    {
        list = new List<T>();
    }

    public void Enqueue(T item)
    {
        list.Add(item);
        int ci = list.Count - 1; // child index; start at end
        while (ci > 0)
        {
            int pi = (ci - 1) / 2; // parent index
            if (list[ci].CompareTo(list[pi]) >= 0) break; // child item is larger than (or equal) parent so we're done
            T tmp = list[ci]; list[ci] = list[pi]; list[pi] = tmp;
            ci = pi;
        }
    }

    public T Dequeue()
    {
        // assumes pq is not empty; up to calling code
        int li = list.Count - 1; // last index (before removal)
        T frontItem = list[0];   // fetch the front
        list[0] = list[li];
        list.RemoveAt(li);

        --li; // last index (after removal)
        int pi = 0; // parent index. start at front of pq
        while (true)
        {
            int ci = pi * 2 + 1; // left child index of parent
            if (ci > li) break;  // no children so done
            int rc = ci + 1;     // right child
            if (rc <= li && list[rc].CompareTo(list[ci]) < 0) // if there is a rc (ci + 1), and it is smaller than left child, use the rc instead
            {
                ci = rc;
            }
            if (list[pi].CompareTo(list[ci]) <= 0) break; // parent is smaller than (or equal to) smallest child so done
            T tmp = list[pi]; list[pi] = list[ci]; list[ci] = tmp; // swap parent and child
            pi = ci;
        }
        return frontItem;
    }

    public T Peek()
    {
        T frontItem = list[0];
        return frontItem;
    }

    public int Count => list.Count;

    public override string ToString()
    {
        string s = "";
        for (int i = 0; i < list.Count; ++i)
            s += list[i].ToString() + " ";
        s += "count = " + list.Count;
        return s;
    }

    public IEnumerator<T> GetEnumerator()
    {
        return list.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return list.GetEnumerator();
    }
}
