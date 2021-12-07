using UnityEngine;

namespace AI.Trees.Nodes
{
    public class BankerWorkBehaviour : Node
    {
        private readonly Actor agent;
        private Node subNode;

        public BankerWorkBehaviour(Actor agent)
        {
            this.agent = agent;
        }

        protected override void Init()
        {
            subNode = new WorkAtStationBehaviour<BankDesk>(agent);
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
