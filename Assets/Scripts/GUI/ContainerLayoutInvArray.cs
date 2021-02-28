using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerLayoutInvArray : IContainerLayoutElement
{
    public int startIndex; // inclusive
    public int endIndex; // inclusive

    public ContainerLayoutInvArray (int start, int end)
    {
        startIndex = start;
        endIndex = end;
    }

    public GameObject Create(out float pivotDelta)
    {
        throw new System.NotImplementedException();
    }
}
