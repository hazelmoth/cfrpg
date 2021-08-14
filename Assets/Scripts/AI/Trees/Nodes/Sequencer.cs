using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using UnityEngine;

namespace AI.Trees.Nodes
{
    /// Executes multiple tasks in sequence, updating each Node until it returns Success.
    /// Nodes are created only when the respective task is reached in the sequence.
    /// Immediately returns Failure if any Node fails. Returns Success if all Nodes return
    /// Success.
    public class Sequencer : Node
    {
        private readonly IList<Func<Node>> sequence;
        private int index;
        private Node current;
        private Status lastStatus;

        public Sequencer(params Func<Node>[] nodes)
        {
            Debug.Assert(nodes.Length > 0);
            sequence = nodes.ToImmutableList();
        }

        protected override void Init()
        {
            index = 0;
            current = sequence[0].Invoke();
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
                current = sequence[index].Invoke();
            }

            return lastStatus == Status.Failure 
                ? Status.Failure 
                : Status.Running;
        }
    }
}