
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
            throw new System.NotImplementedException();
        }

        protected override void OnCancel()
        {
            throw new System.NotImplementedException();
        }
    }
}
