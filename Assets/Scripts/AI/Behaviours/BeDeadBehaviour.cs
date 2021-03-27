
// A Behaviour for a dead actor. Does nothing at all.
namespace AI.Behaviours
{
    public class BeDeadBehaviour : AIBehaviour
    {
        public BeDeadBehaviour(Actor actor) : base(actor)
        {
        }

        protected override void OnExecute()
        {

        }

        protected override void OnCancel()
        {

        }
    }
}
