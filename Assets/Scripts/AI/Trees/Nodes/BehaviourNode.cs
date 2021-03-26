using AI.Behaviours;

namespace AI.Trees.Nodes
{
    // A node which executes an IAIBehaviour
    public class BehaviourNode : Node
    {
        private readonly IAiBehaviour behaviour;

        public BehaviourNode(IAiBehaviour behaviour)
        {
            this.behaviour = behaviour;
        }

        protected override void Init()
        {
            behaviour.Execute();
        }

        protected override Status OnUpdate()
        {
            // Assume for the time being that all behaviours finish successfully
            if (behaviour.IsRunning) return Status.Running;
            else return Status.Success;
        }
    }
}
