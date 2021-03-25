using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node_Conditional : Node
{
    private Node left, right;
    private Func<bool> condition;
    
    public Node_Conditional(Node left, Node right, Func<bool> condition)
    {
        this.left = left;
        this.right = right;
        this.condition = condition;
    }
    public override Status Update()
    {
        return condition() ? right.Update() : left.Update();
    }
}
