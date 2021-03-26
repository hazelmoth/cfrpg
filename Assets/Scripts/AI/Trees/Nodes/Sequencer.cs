using System.Collections.Generic;
using UnityEngine;

namespace AI.Trees.Nodes
{
    // Executes a bunch of Nodes in sequence. Immediately returns Failure if any
    // of those Nodes fails.
    public class Sequencer : Node
    {
        private IList<Node> sequence;
        private int current;
        private Status lastStatus;
    
        public Sequencer(IList<Node> nodes)
        {
            Debug.Assert(nodes.Count > 0);
            sequence = nodes;
        }

        protected override void Init()
        {
            current = 0;
            lastStatus = Status.Running;
        }

        protected override Status OnUpdate()
        {
            lastStatus = sequence[current].Update();
            if (lastStatus == Status.Success) current++;
        
            if (current >= sequence.Count) return Status.Success;
            if (lastStatus == Status.Failure) return Status.Failure;

            return Status.Running;
        }
    }
}
