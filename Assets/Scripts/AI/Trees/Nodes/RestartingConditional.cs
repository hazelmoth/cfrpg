using System;

namespace AI.Trees.Nodes
{
    // A conditional node that starts a new child Node every time it switches branches.
    public class RestartingConditional : Node
    {
        private readonly Func<Node> left, right;
        private readonly Func<bool> condition;
        private Node current;
        private bool conditionMetLastFrame;

        public RestartingConditional(Func<bool> condition, Func<Node> left, Func<Node> right)
        {
            this.left = left;
            this.right = right;
            this.condition = condition;
        }

        protected override void Init()
        {
            conditionMetLastFrame = condition();
            current = conditionMetLastFrame ? left.Invoke() : right.Invoke();
        }

        protected override Status OnUpdate()
        {
            bool conditionMet = condition();
        
            if (conditionMet != conditionMetLastFrame)
            {
                current.Cancel();
                // We're switching branches. Make a new node.
                current = conditionMet ? left.Invoke() : right.Invoke();
            }

            conditionMetLastFrame = conditionMet;
            return current.Update();
        }

        protected override void OnCancel()
        {
            current.Cancel();
        }
    }
}
