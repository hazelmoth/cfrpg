using UnityEngine;

namespace AI.Trees.Nodes
{
    /// A behaviour which makes an Actor actively work as a sheriff, in the region she is
    /// currently in. This currently just entails sitting at an available sheriff desk.
    public class SheriffWorkBehaviour : Node
    {
        private Actor agent;
        private Node subNode;
        private NonPlayerWorkstation sheriffDesk;

        public SheriffWorkBehaviour(Actor agent)
        {
            this.agent = agent;
        }

        protected override void Init()
        {
            subNode = new Conditional(
                () => sheriffDesk != null,
                () => new GoToAndWorkAtStation(agent, sheriffDesk),
                () => new Wander(agent));
        }

        protected override void OnCancel()
        {
            subNode.Cancel();
        }

        protected override Status OnUpdate()
        {
            sheriffDesk ??= Object.FindObjectOfType<SheriffDesk>();
            return subNode.Update();
        }
    }
}
