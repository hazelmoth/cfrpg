using System.Collections.Generic;
using AI.Nodes;
using UnityEngine;

namespace AI
{
    /// A MonoBehaviour that runs a behaviour node to completion.
    public class BasicNodeRunner : MonoBehaviour
    {
        private List<Node> runningNodes = new();

        public void RunNode(Node node)
        {
            runningNodes.Add(node);
        }

        private void Update()
        {
            runningNodes.RemoveAll(n => n.Stopped);
            runningNodes.ForEach(n => n.Update());
        }
    }
}
