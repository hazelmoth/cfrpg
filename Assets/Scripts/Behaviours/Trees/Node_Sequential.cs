using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node_Sequential : Node
{
    private IList<Node> sequence;
    private int current;
    private Status lastStatus;
    
    public Node_Sequential(IList<Node> nodes)
    {
        Debug.Assert(nodes.Count > 0);
        sequence = nodes;
        current = 0;
        lastStatus = Status.Running;
    }
    
    public override Status Update()
    {
        lastStatus = sequence[current].Update();
        if (lastStatus == Status.Success) current++;
        
        if (current >= sequence.Count) return Status.Success;
        if (lastStatus == Status.Failure) return Status.Failure;

        return Status.Running;
    }
}
