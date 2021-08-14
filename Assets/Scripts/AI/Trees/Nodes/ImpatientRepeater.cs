using System;
using UnityEngine;

namespace AI.Trees.Nodes
{
    /// Repeatedly recreates and runs a Node for the given task. Restarts the task when it
    /// returns success or failure, or when a maximum time limit is reached. Optionally
    /// returns Success when a node succeeds, instead of restarting.
    public class ImpatientRepeater : Node
    {
        private readonly Func<Node> task;
        private readonly float maxRestartTime;
        private readonly bool finishOnSuccess;
        private Node current;
        private float lastStartTime;
    
        public ImpatientRepeater(Func<Node> task, float maxRestartTime, bool finishOnSuccess = false)
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
            current = task.Invoke();
            lastStartTime = Time.time;
        }
    }
}
