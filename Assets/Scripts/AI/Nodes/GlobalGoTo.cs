namespace AI.Nodes
{
    /// A behaviour that allows actors to navigate through entire regions of the world,
    /// regardless of whether they are loaded. This inevitably relies on assumptions about
    /// region traversal times, and will ignore anything that is actually happening in
    /// unloaded regions.
    public class GlobalGoTo : Node
    {
        private readonly string actorId;
        private readonly string destRegionId;
        private readonly Location destLocation;

        protected override void Init()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnCancel()
        {
            throw new System.NotImplementedException();
        }

        protected override Status OnUpdate()
        {
            throw new System.NotImplementedException();
        }
    }
}
