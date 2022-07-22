using System;

namespace AI.Nodes
{
    /// A Node that returns Running until its condition is true, and then returns Success.
    public class WaitUntil : Node
    {
        private Func<bool> condition;

        public WaitUntil(Func<bool> condition)
        {
            this.condition = condition;
        }

        protected override void Init() { }

        protected override void OnCancel() { }

        protected override Status OnUpdate()
        {
            return condition() ? Status.Success : Status.Running;
        }
    }
}
