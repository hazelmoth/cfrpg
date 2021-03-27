using System.Collections.Generic;
using UnityEngine;

namespace AI.Trees.Nodes
{
    // Executes multiple Tasks in sequence. Nodes are created only when the
    // respective Task is reached in the sequence. Immediately returns Failure
    // if any of those Nodes fails.
    public class Sequencer : Node
    {
        private readonly IList<Task> sequence;
        private int index;
        private Node current;
        private Status lastStatus;
    
        public Sequencer(IList<Task> nodes)
        {
            Debug.Log("Constructing sequencer.");
            Debug.Assert(nodes.Count > 0);
            sequence = nodes;
        }

        protected override void Init()
        {
            Debug.Log("Sequencer initializing.");
            index = 0;
            current = sequence[0].CreateNode();
            lastStatus = Status.Running;
        }

        protected override Status OnUpdate()
        {
            lastStatus = current.Update();
            
            if (lastStatus == Status.Success)
            {
                index++;
                Debug.Log($"Sequencer moving to index {index} of {sequence.Count-1}.");
                if (index >= sequence.Count) return Status.Success;
                current = sequence[index].CreateNode();
            }

            if (lastStatus == Status.Failure)
            {
                Debug.Log("Sequencer failed on index " + index + ", " + current.GetType().FullName);
                return Status.Failure;
            }

            return Status.Running;
        }
    }
}