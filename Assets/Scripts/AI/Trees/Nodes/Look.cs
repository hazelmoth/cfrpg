namespace AI.Trees.Nodes
{
    /// A single-frame behaviour that makes an Actor look in some direction.
    /// Always returns Success.
    public class Look : Node
    {
        private Actor agent;
        private Direction direction;

        public Look(Actor agent, Direction direction)
        {
            this.agent = agent;
            this.direction = direction;
        }
        
        protected override void Init() { }

        protected override void OnCancel() { }

        protected override Status OnUpdate()
        {
            agent.GetComponent<ActorAnimController>().SetDirection(direction);
            return Status.Success;
        }
    }
}