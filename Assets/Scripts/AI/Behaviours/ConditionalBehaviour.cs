using System;
using System.Collections;
using UnityEngine;

// A Behaviour which takes a Func<IAIBehaviour> and executes whichever behaviour
// that function returns at a given time.
namespace AI.Behaviours
{
    public class ConditionalBehaviour : AIBehaviour
    {
        private Coroutine coroutine;
        private IAiBehaviour activeBehaviour;
        private Func<IAiBehaviour> condition;

        public ConditionalBehaviour(Actor actor, Func<IAiBehaviour> condition) : base(actor)
        {
            this.condition = condition;
        }

        private IEnumerator Coroutine ()
        {
            IAiBehaviour toExec = condition();
            yield return null;
            throw new NotImplementedException();
        }

        protected override void OnExecute()
        {
            coroutine = GlobalCoroutineObject.Instance.StartCoroutine(Coroutine());
        }

        protected override void OnCancel()
        {
            throw new NotImplementedException();
        }
    }
}
