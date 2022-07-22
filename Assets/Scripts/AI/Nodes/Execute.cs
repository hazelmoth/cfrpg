using System;

namespace AI.Nodes
{
    /// A node that just executes a given function immediately, and returns success.
    public class Execute : Node
    {
        private readonly Action function;

        public Execute(Action function)
        {
            this.function = function;
        }

        protected override void Init() { }

        protected override void OnCancel() { }

        protected override Status OnUpdate()
        {
            function();
            return Status.Success;
        }
    }
}
