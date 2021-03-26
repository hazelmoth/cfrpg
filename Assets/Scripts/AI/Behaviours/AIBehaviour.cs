using UnityEngine;

namespace AI.Behaviours
{
    // An abstract Behaviour which keeps track of whether the behaviour is running.
    public abstract class AIBehaviour : IAiBehaviour
    {
        protected Actor actor;

        protected AIBehaviour(Actor actor)
        {
            this.actor = actor;
        }

        public void Execute()
        {
            if (IsRunning)
            {
                Debug.LogWarning("Restarting already running behaviour.");
                Cancel();
            }
            IsRunning = true;
            OnExecute();
        }

        public void Cancel()
        {
            IsRunning = false;
            OnCancel();
        }

        public bool IsRunning { get; protected set; }

        protected abstract void OnExecute();
        protected abstract void OnCancel();
    }
}
