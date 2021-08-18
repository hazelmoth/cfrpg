namespace AI.Trees.Nodes
{
    /// A behaviour for continuously claiming an Occupiable. Returns a status of Failure
    /// if the Occupiable is destroyed, or if it claimed by another Actor.
    public class OccupyOccupiable : Node
    {
        private readonly Actor agent;
        private readonly IOccupiable occupiable;

        public OccupyOccupiable(Actor agent, IOccupiable occupiable)
        {
            this.agent = agent;
            this.occupiable = occupiable;
        }

        protected override void Init() { }

        protected override void OnCancel() { }

        protected override Status OnUpdate()
        {
            if (occupiable == null) return Status.Failure;
            return occupiable.OccupyNextFrame(agent) ? Status.Running : Status.Failure;
        }
    }
}
