using System.Collections.Generic;
using UnityEngine;

// Behaviour of an actor who is not a trader or in any faction.
namespace Behaviours
{
    public class DrifterBehaviour : SequentialBehaviour
    {
        public DrifterBehaviour(Actor actor) : base(actor) { }

        protected override IEnumerator<IAiBehaviour> GetBehaviours()
        {
            yield return new MoveRandomlyBehaviour(actor, 10, null);
            yield return new NavigateNextToObjectBehaviour(actor, Object.FindObjectOfType<BreakableTree>().gameObject,
                actor.CurrentScene, null);
        }
    }
}
