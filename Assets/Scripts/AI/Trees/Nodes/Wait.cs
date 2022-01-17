using System.Collections;
using System.Collections.Generic;
using AI.Trees.Nodes;
using UnityEngine;

/// A Node that does nothing for a given duration. Always returns Success afterwards.
/// If given time is zero, returns Success immediately.
public class Wait : Node
{
    private float duration;
    private float startTime;

    public Wait(float duration)
    {
        this.duration = duration;
    }
    
    protected override void Init()
    {
        startTime = Time.time;
    }

    protected override void OnCancel()
    { }

    protected override Status OnUpdate()
    {
        return (Time.time - startTime > duration) ? Status.Success : Status.Running;
    }
}
