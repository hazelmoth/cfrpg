using UnityEngine;

namespace AI.Trees.Nodes
{
    public class ShopkeeperWorkBehaviour : Node
    {
        private readonly Actor agent;
        private NonPlayerWorkstation shopWorkstation;
        private Node subNode;

        public ShopkeeperWorkBehaviour(Actor agent)
        {
            this.agent = agent;
        }

        protected override void Init()
        {
            subNode = new Repeater(
                () => new Conditional(
                    () => shopWorkstation != null,
                    () => new GoToAndWorkAtStation(agent, shopWorkstation),
                    () => new Wander(agent)));
        }

        protected override void OnCancel()
        {
            subNode.Cancel();
        }

        protected override Status OnUpdate()
        {
            // so this isn't the best for performance TODO
            // also, won't work if there is more than one TODO
            if (shopWorkstation == null)
                shopWorkstation = Object.FindObjectOfType<ShopStation>();

            return subNode.Update();
        }
    }
}
