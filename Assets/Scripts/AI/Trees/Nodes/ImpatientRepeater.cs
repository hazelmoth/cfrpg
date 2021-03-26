using System;
using System.Reflection;
using UnityEngine;

namespace AI.Trees.Nodes
{
    // Repeatedly recreates and runs a Node of the given type, with a maximum
    // time limit before it restarts, regardless of whether the current Node
    // has finished running by then.
    public class ImpatientRepeater : Node
    {
        private readonly Type nodeType;
        private readonly object[] args;
        private readonly float maxRestartTime;
        private Node current;
        private float lastStartTime;
    
        public ImpatientRepeater(Type nodeType, object[] args, float maxRestartTime)
        {
            this.nodeType = nodeType;
            this.args = args;
            this.maxRestartTime = maxRestartTime;
            lastStartTime = Time.time;
        }

        protected override void Init()
        {
            Restart();
        }

        protected override Status OnUpdate()
        {
            if (current.Update() != Status.Running || Time.time - lastStartTime > maxRestartTime)
            {
                Restart();
            }
            return Status.Running;
        }

        private void Restart()
        {
            current = (Node)Activator.CreateInstance(nodeType, BindingFlags.OptionalParamBinding, null, args, null);
            lastStartTime = Time.time;
        }
    }
}
