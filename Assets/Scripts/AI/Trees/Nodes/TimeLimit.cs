using System;
using UnityEngine;

namespace AI.Trees.Nodes
{
    /// Runs a task to completion and returns its status if it completes within
    /// a given time limit; otherwise, stops the task and returns failure.
    public class TimeLimit : Node
    {
        private readonly Func<Node> task;
        private readonly float timeLimit;
        private Node current;
        private float startTime;
    
        public TimeLimit(Func<Node> task, float timeLimit)
        {
            this.task = task;
            this.timeLimit = timeLimit;
        }

        protected override void Init()
        {
            current = task.Invoke();
            startTime = Time.time;
        }

        protected override void OnCancel()
        {
            if (current != null && !current.Stopped) current.Cancel();
        }

        protected override Status OnUpdate()
        {
            if (Time.time - startTime > timeLimit)
            {
                if (current != null && !current.Stopped) current.Cancel();
                return Status.Failure;
            }

            return current.Update();
        }
    }
}