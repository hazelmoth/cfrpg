using System;
using System.Reflection;
using UnityEngine;
using AI.Trees.Nodes;

namespace AI.Trees
{
    /*
     * A Task defines the parameters for creating and running a Node. 
     * Tasks allow for easy repetition by repeatedly recreating a Node with
     * the same parameters.
    */
    public class Task
    {
        public readonly Type nodeType;
        public readonly object[] args;

        public Task(Type nodeType, object[] args)
        {
            Debug.Assert(nodeType.IsSubclassOf(typeof(Node)));
            
            this.nodeType = nodeType;
            this.args = args;
        }

        // Creates a Node with the parameters defined in this Task.
        public Node CreateNode()
        {
            // Use BindingFlags to allow optional parameters.
            return (Node)Activator.CreateInstance(nodeType, BindingFlags.OptionalParamBinding, null, args, null);
        }
    }
}