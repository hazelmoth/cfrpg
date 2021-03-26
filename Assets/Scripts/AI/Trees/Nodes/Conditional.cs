using System;

namespace AI.Trees.Nodes
{
    public class Conditional : Node
    {
        private Node left, right;
        private Func<bool> condition;
    
        public Conditional(Node left, Node right, Func<bool> condition)
        {
            this.left = left;
            this.right = right;
            this.condition = condition;
        }

        protected override void Init() { }

        protected override Status OnUpdate()
        {
            return condition() ? right.Update() : left.Update();
        }
    }
}