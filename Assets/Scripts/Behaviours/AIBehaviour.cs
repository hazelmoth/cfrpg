
// An abstract Behaviour to handle basic things like IsRunning
namespace Behaviours
{
    public abstract class AIBehaviour : IAiBehaviour
    {
        private Actor actor;

        public AIBehaviour(Actor actor)
        {
            this.actor = actor;
        }

        public void Execute()
        {
            IsRunning = true;
        }

        public void Cancel()
        {
            IsRunning = false;
        }

        public bool IsRunning { get; private set; }
    }
}
