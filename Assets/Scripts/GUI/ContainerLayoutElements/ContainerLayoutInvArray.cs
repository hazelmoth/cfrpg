using UnityEngine;

namespace GUI.ContainerLayoutElements
{
    public class ContainerLayoutInvArray : IContainerLayoutElement
    {
        public int startIndex; // inclusive
        public int endIndex; // inclusive

        // Constructs an inv array with slots from the given start index to the given end
        // index, inclusive.
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
}
