using System.Collections.Generic;

namespace AI.Trees.Nodes
{
    /// A behaviour node that makes the actor wander around aimlessly, without leaving
    /// their current subregion.
    public class WanderLocal : Node
    {
        private Node subNode;
        private Actor agent;

        public WanderLocal(Actor agent)
        {
            this.agent = agent;
        }

        protected override void Init()
        {
            subNode = new ImpatientRepeater(
                () => new Sequencer(
                    () => new MoveRandomly(agent, 20),
                    () => new Wait(1.4f)
                ),
                maxRestartTime: 15f
            );
        }

        protected override Status OnUpdate()
        {
            return subNode.Update();
        }

        protected override void OnCancel()
        {
            if (subNode != null && !subNode.Stopped) subNode.Cancel();
        }
    }
}
