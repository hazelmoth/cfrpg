namespace AI.Trees.Nodes
{
    /// A navigation node which continuously recalculates the path.
    public class GoTo : Node
    {
        /// Duration of time between each path recalculation
        private const float RecalculateFrequency = 1f;

        private readonly Actor agent;
        private readonly Location target;
        private readonly float margin;
        private readonly Actor ignoreCollisionWithActor;
        private Node current;

        public GoTo(Actor agent, Location target, float margin, Actor ignoreCollisionWithActor = null)
        {
            this.agent = agent;
            this.target = target;
            this.margin = margin;
            this.ignoreCollisionWithActor = ignoreCollisionWithActor;
        }

        protected override void Init()
        {
            current = new ImpatientRepeater(
                () => new SimpleGoTo(agent, target, margin, ignoreCollisionWithActor),
                maxRestartTime: RecalculateFrequency,
                finishOnSuccess: true);
        }

        protected override void OnCancel()
        {
            current.Cancel();
        }

        protected override Status OnUpdate()
        {
            return current.Update();
        }
    }
}
