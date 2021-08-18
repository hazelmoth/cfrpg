using UnityEngine;

namespace AI.Trees.Nodes
{
    public class ShopkeeperBehaviour : Node
    {
        private const float ShopPositionAcceptableMargin = 0.1f;
        private readonly Actor agent;
        private NonPlayerWorkstation shopWorkstation;
        private Node subNode;

        public ShopkeeperBehaviour(Actor agent)
        {
            this.agent = agent;
        }

        protected override void Init()
        {
            subNode = new Repeater(
                () => new Conditional(
                    () => shopWorkstation != null,
                    () => new Sequencer(
                        () => new GoTo(
                            agent,
                            shopWorkstation.UserTileLocation.WithOffset(new Vector2(0.5f, 0.5f)),
                            ShopPositionAcceptableMargin),
                        () => new Look(agent, shopWorkstation.UserDirection),
                        () => new Conditional(
                            CheckDistance,
                            () => new OccupyOccupiable(agent, shopWorkstation),
                            () => new InstantFailer())),
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

        private bool CheckDistance()
        {
            if (shopWorkstation == null) return false;

            float distance = Vector2.Distance(agent.Location.Vector2, shopWorkstation.UserTileLocation.Vector2 + new Vector2(0.5f, 0.5f));
            Debug.Log($"checking dist: {distance}");
            return distance <= ShopPositionAcceptableMargin;
        }
    }
}
