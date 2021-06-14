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
            Debug.Assert(nodes.Count > 0);
            sequence = nodes;
        }

        protected override void Init()
        {
            index = 0;
            current = sequence[0].CreateNode();
            lastStatus = Status.Running;
        }

        protected override void OnCancel()
        {
            if (current != null && !current.Stopped) current.Cancel();
        }

        protected override Status OnUpdate()
        {
            lastStatus = current.Update();
            
            if (lastStatus == Status.Success)
            {
                index++;
                if (index >= sequence.Count) return Status.Success;
                current = sequence[index].CreateNode();
            }

            if (lastStatus == Status.Failure)
            {
                return Status.Failure;
            }

            return Status.Running;
        }
    }
}