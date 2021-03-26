using System;
using System.Reflection;

namespace AI.Trees.Nodes
{
    // Repeatedly recreates and runs a Node of the given type, using the given
    // constructor arguments.
    public class Repeater : Node
    {
        private Node current;
        private readonly Type nodeType;
        private readonly object[] args;
    
        public Repeater(Type nodeType, object[] args)
        {
            this.nodeType = nodeType;
            this.args = args;
        }

        protected override void Init() { }

        protected override Status OnUpdate()
        {
            current ??= (Node)Activator.CreateInstance(nodeType, BindingFlags.OptionalParamBinding, null, args, null);

            if (current.Update() != Status.Running)
            {
                current = null;
            }
            return Status.Running;
        }
    }
}
