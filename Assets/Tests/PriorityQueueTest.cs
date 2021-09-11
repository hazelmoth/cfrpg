using NUnit.Framework;

public class PriorityQueueTest
{
    [Test]
    public void Dequeue_Ints()
    {
        Assert.That(IntTest());
    }

    [Test]
    public void Dequeue_Strings()
    {
        Assert.That(StringTest());
    }

    // A simple queue/dequeue test; returns true iff test passes
    private static bool IntTest()
    {
        PriorityQueue<int> pq = new PriorityQueue<int>();
        pq.Enqueue(2);
        pq.Enqueue(4);
        pq.Enqueue(7);
        pq.Enqueue(1);
        pq.Enqueue(5);
        pq.Enqueue(6);
        pq.Enqueue(3);
        // test dequeue
        if (pq.Dequeue() != 1) return false;
        if (pq.Dequeue() != 2) return false;
        if (pq.Dequeue() != 3) return false;
        if (pq.Dequeue() != 4) return false;
        if (pq.Dequeue() != 5) return false;
        if (pq.Dequeue() != 6) return false;
        if (pq.Dequeue() != 7) return false;
        if (pq.Count != 0) return false;
        return true;
    }

    // A test for the priority queue using a list of strings
    private static bool StringTest()
    {
        PriorityQueue<string> pq = new PriorityQueue<string>();
        pq.Enqueue("ee");
        pq.Enqueue("cc");
        pq.Enqueue("ff");
        pq.Enqueue("aa");
        pq.Enqueue("bb");
        pq.Enqueue("gg");
        pq.Enqueue("dd");
        // test dequeue
        if (pq.Dequeue() != "aa") return false;
        if (pq.Dequeue() != "bb") return false;
        if (pq.Dequeue() != "cc") return false;
        if (pq.Dequeue() != "dd") return false;
        if (pq.Dequeue() != "ee") return false;
        if (pq.Dequeue() != "ff") return false;
        if (pq.Dequeue() != "gg") return false;
        if (pq.Count != 0) return false;
        return true;
    }
}
