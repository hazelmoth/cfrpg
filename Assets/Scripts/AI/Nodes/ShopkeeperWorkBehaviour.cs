namespace AI.Nodes
{
    public class ShopkeeperWorkBehaviour : Node
    {
        private readonly Actor agent;
        private Node subNode;

        public ShopkeeperWorkBehaviour(Actor agent)
        {
            this.agent = agent;
        }

        protected override void Init()
        {
            subNode = new WorkAtStationBehaviour<ShopStation>(agent);
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
