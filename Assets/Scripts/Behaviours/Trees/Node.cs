using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Node
{
    // Runs this tree for a single frame, and returns its current state.
    public abstract Status Update();

    public enum Status
    {
        Success, // The behaviour has finished successfully
        Failure, // The behaviour has finished unsuccessfully
        Running  // The behaviour is still running.
    }
}
