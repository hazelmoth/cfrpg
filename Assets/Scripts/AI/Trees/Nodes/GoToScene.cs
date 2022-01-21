namespace AI.Trees.Nodes
{
    public class GoToScene : Node
    {
        private Actor agent;
        private string targetScene;
        private Node subNode;

        public GoToScene(Actor agent, string targetScene)
        {
            this.agent = agent;
            this.targetScene = targetScene;
        }

        protected override void Init()
        {
            Location targetLocation = new Location(0, 0, targetScene);
            subNode = new GoTo(agent, targetLocation, 0.1f);
        }

        protected override void OnCancel()
        {
            if (subNode is {Stopped: false}) subNode.Cancel();
        }

        protected override Status OnUpdate()
        {
            return subNode.Update();
        }
    }
}
