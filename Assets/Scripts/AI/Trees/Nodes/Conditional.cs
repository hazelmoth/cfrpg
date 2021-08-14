using System;

namespace AI.Trees.Nodes
{
    /// A 2-branch conditional Node. Starts a new child Node every time it switches
    /// branches. If a child node returns Success or Failure, this Node will return that
    /// same state.
    public class Conditional : Node
    {
        private readonly Func<Node> left, right;
        private readonly Func<bool> condition;
        private Node current;
        private bool conditionMetLastFrame;

        public Conditional(Func<bool> condition, Func<Node> left, Func<Node> right)
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
