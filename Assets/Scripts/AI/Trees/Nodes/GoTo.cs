using AI.Behaviours;

namespace AI.Trees.Nodes
{
    public class GoTo : Node
    {
        private Actor agent;
        private TileLocation target;
        private float margin;
        private Node navBehaviour;

        private Status currentStatus;

        public GoTo(Actor agent, TileLocation target, float margin)
        {
            this.agent = agent;
            this.target = target;
            this.margin = margin;
        }
    
        protected override void Init()
        {
            currentStatus = Status.Running;
            navBehaviour = new BehaviourNode(new NavigateBehaviour(agent, target, OnNavFinished));
        }

        protected override Status OnUpdate()
        {
            navBehaviour.Update();
            return currentStatus;
        }

        private void OnNavFinished(bool success)
        {
            currentStatus = success ? Status.Success : Status.Failure;
        }
    }
}
