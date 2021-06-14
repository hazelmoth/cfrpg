using System;

namespace AI.Trees.Nodes
{
    // A conditional node that starts a new child Node every time it switches branches.
    public class RestartingConditional : Node
    {
        private readonly Task left, right;
        private readonly Func<bool> condition;
        private Node current;
        private bool conditionMetLastFrame;

        public RestartingConditional(Func<bool> condition, Task left, Task right)
        {
            this.left = left;
            this.right = right;
            this.condition = condition;
        }

        protected override void Init()
        {
            conditionMetLastFrame = condition();
            current = conditionMetLastFrame ? left.CreateNode() : right.CreateNode();
        }

        protected override Status OnUpdate()
        {
            bool conditionMet = condition();
        
            if (conditionMet != conditionMetLastFrame)
            {
                current.Cancel();
                // We're switching branches. Make a new node.
                current = conditionMet ? left.CreateNode() : right.CreateNode();
            }
            return current.Update();
        }

        protected override void OnCancel()
        {
            current.Cancel();
        }
    }
}
