namespace AI.Nodes
{
    /// A behaviour which makes an Actor actively work as a sheriff, in the region she is
    /// currently in. This currently just entails sitting at an available sheriff desk.
    public class SheriffWorkBehaviour : Node
    {
        private readonly Actor agent;
        private Node subNode;

        public SheriffWorkBehaviour(Actor agent)
        {
            this.agent = agent;
        }

        protected override void Init()
        {
            subNode = new WorkAtStationBehaviour<SheriffDesk>(agent);
        }

        protected override void OnCancel()
        {
            subNode.Cancel();
        }

        protected override Status OnUpdate()
        {
            return subNode.Update();
        }
    }
}
