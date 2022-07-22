namespace AI.Nodes
{
    public class Follow : Node
    {
        // Interval at which path is recalculated
        private const float RefreshTime = 0.5f;
    
        private readonly Actor agent;
        private readonly Actor target;
        private readonly float targetDist;
        private Node subNode;
    
        public Follow(Actor agent, Actor target, float targetDist)
        {
            this.agent = agent;
            this.target = target;
            this.targetDist = targetDist;
        }
    
        protected override void Init()
        {
            subNode = new ImpatientRepeater(
                () => new GoToActor(agent, target, targetDist), 
                RefreshTime
            );
        }

        protected override void OnCancel()
        {
            if (subNode != null && !subNode.Stopped) subNode.Cancel();
        }

        protected override Status OnUpdate()
        {
            subNode.Update();
            return Status.Running;
        }
    }
} 