using UnityEngine;

namespace AI.Trees.Nodes
{
    public class ShopkeeperBehaviour : Node
    {
        private readonly Actor agent;
        private NonPlayerWorkstation shopWorkstation;
        private Node subNode;

        public ShopkeeperBehaviour(Actor agent)
        {
            this.agent = agent;
        }

        protected override void Init()
        {
            // TODO figure out why this isn't doing anything :/
            subNode = new Repeater(
                () => new Conditional(
                    () => shopWorkstation != null,
                    () => new Sequencer(
                        () => new GoTo(agent, shopWorkstation.UserTileLocation, margin: 0.1f),
                        () => new Look(agent, shopWorkstation.UserDirection),
                        () => new OccupyOccupiable(agent, shopWorkstation)),
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
                shopWorkstation = Object.FindObjectOfType<NonPlayerWorkstation>();

            return subNode.Update();
        }
    }
}