using System.Collections.Generic;
using AI.Trees.Nodes;
using UnityEngine;

namespace AI.Trees
{
    /// A MonoBehaviour that runs a behaviour node to completion.
    public class BasicNodeRunner : MonoBehaviour
    {
        private List<Node> runningNodes = new List<Node>();

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
