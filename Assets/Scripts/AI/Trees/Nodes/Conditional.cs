using System;

namespace AI.Trees.Nodes
{
    // A Node which runs one of two child Nodes depending on whether a condition is met.
    public class Conditional : Node
    {
        private Node left, right;
        private Func<bool> condition;
    
        public Conditional(Func<bool> condition, Node left, Node right)
        {
            this.left = left;
            this.right = right;
            this.condition = condition;
        }

        protected override void Init() { }

        protected override void OnCancel()
        {
            if (left.Started && !left.Stopped) left.Cancel();
            if (right.Started && !right.Stopped) right.Cancel();
        }

        protected override Status OnUpdate()
        {
            return condition() ? right.Update() : left.Update();
        }
    }
}