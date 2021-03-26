using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// An abstract Behaviour which just runs a bunch of sub-behaviours in sequence.
namespace AI.Behaviours
{
    public abstract class SequentialBehaviour : IAiBehaviour
    {
        protected Actor actor; // The actor on which this behaviour is running.
    
        private IAiBehaviour current; // The currently running Behaviour.
        private Coroutine coroutine;

        public SequentialBehaviour(Actor actor)
        {
            this.actor = actor;
        }
    
        // Returns the behaviours to be executed, in order.
        protected abstract IEnumerator<IAiBehaviour> GetBehaviours();
    
        public void Execute()
        {
            IsRunning = true;
            coroutine = GlobalCoroutineObject.Instance.StartCoroutine(BehaviourCoroutine());
        }

        public void Cancel()
        {
            current?.Cancel();
            if (coroutine != null) GlobalCoroutineObject.Instance.StopCoroutine(coroutine);
            IsRunning = false;
        }

        public bool IsRunning { get; private set; }
    
        private IEnumerator BehaviourCoroutine()
        {
            IEnumerator<IAiBehaviour> behaviours = GetBehaviours();
        
            // Execute every behaviour in order, waiting for each to finish.
            do
            {
                current = behaviours.Current;
                if (current == null) continue;
            
                current.Execute();
                while (current.IsRunning) yield return null;
            } while (behaviours.MoveNext());
        
            // Finished.
            IsRunning = false;
        }
    }
}
