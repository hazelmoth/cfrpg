using UnityEngine;

namespace AI.Trees.Nodes
{
    /// A Behaviour which has an agent stare in the direction of a particular actor.
    /// Always returns a status of Running.
    public class StareAtActor : Node
    {
        private readonly Actor agent;
        private readonly Actor target;
        private Node subNode;

        public StareAtActor(Actor agent, Actor target)
        {
            this.agent = agent;
            this.target = target;
            Debug.Assert(agent != null);
            Debug.Assert(target != null);
        }

        protected override void Init()
        {
            subNode = new Repeater(
                () => new Look(agent, GetDirectionToTarget()));
        }

        protected override void OnCancel()
        {
            subNode.Cancel();
        }

        protected override Status OnUpdate()
        {
            return subNode.Update();
        }

        private Direction GetDirectionToTarget()
        {
            Direction dir = Direction.Down;

            if (agent.CurrentScene == target.CurrentScene)
            {
                dir = (target.Location.Vector2 - agent.Location.Vector2).ToDirection();
            }

            return dir;
        }
    }
}
