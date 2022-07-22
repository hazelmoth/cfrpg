using System;

namespace AI.Nodes
{
    /// Repeatedly recreates and runs a Node for the given Task, using the given
    /// constructor arguments. Always returns a status of Running. Optionally returns
    /// Success when a node succeeds, instead of restarting.
    public class Repeater : Node
    {
        private readonly Func<Node> task;
        private readonly bool finishOnSuccess;
        private Node current;

        public Repeater(Func<Node> task, bool finishOnSuccess = false)
        {
            this.task = task;
            this.finishOnSuccess = finishOnSuccess;
        }

        protected override void Init() { }

        protected override void OnCancel()
        {
            if (current != null && !current.Stopped) current.Cancel();
        }

        protected override Status OnUpdate()
        {
            current ??= task.Invoke();

            Status status = current.Update();
            if (status == Status.Running) return Status.Running;
            if (finishOnSuccess && status == Status.Success) return Status.Success;

            // Reset node to repeat
            current = null;
            return Status.Running;
        }
    }
}
