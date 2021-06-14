using UnityEngine;

namespace AI.Trees.Nodes
{
    // Repeatedly recreates and runs a Node for the given task, with a maximum
    // time limit before it restarts, regardless of whether the current Node has
    // finished running by then. Optionally returns Success when a node succeeds.
    public class ImpatientRepeater : Node
    {
        private readonly Task task;
        private readonly float maxRestartTime;
        private readonly bool finishOnSuccess;
        private Node current;
        private float lastStartTime;
    
        public ImpatientRepeater(Task task, float maxRestartTime, bool finishOnSuccess = false)
        {
            this.task = task;
            this.maxRestartTime = maxRestartTime;
            this.finishOnSuccess = finishOnSuccess;
            lastStartTime = Time.time;
        }

        protected override void Init()
        {
            Restart();
        }

        protected override void OnCancel()
        {
            if (current != null && !current.Stopped) current.Cancel();
        }

        protected override Status OnUpdate()
        {
            Status currentStatus = current.Update();

            if (finishOnSuccess && currentStatus == Status.Success)
            {
                return Status.Success;
            }
            if (currentStatus != Status.Running || Time.time - lastStartTime > maxRestartTime)
            {
                Restart();
            }
            return Status.Running;
        }

        private void Restart()
        {
            current = task.CreateNode();
            lastStartTime = Time.time;
        }
    }
}
