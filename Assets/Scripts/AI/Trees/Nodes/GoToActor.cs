using AI.Behaviours;
using UnityEngine;

namespace AI.Trees.Nodes
{
    public class GoToActor : Node
    {
        private readonly Actor agent;
        private readonly Actor target;
        private readonly float targetDist;

        private Node navNode;
    
        public GoToActor(Actor agent, Actor target, float targetDist)
        {
            this.agent = agent;
            this.target = target;
            this.targetDist = targetDist;
        }

        protected override void Init()
        {
            navNode = new GoTo(agent, target.Location, targetDist, target);
        }

        protected override void OnCancel()
        {
            if (navNode != null && !navNode.Stopped) navNode.Cancel();
        }

        protected override Status OnUpdate()
        {
            if (agent == null || target == null) return Status.Failure;
            
            if (CheckDistance()) return Status.Success;
            return navNode.Update() == Status.Running ? Status.Running : Status.Failure;
        }

        // Returns true if the agent is within the target distance to the target.
        private bool CheckDistance()
        {
            if (agent == null || target == null) return false;
            if (agent.CurrentScene != target.CurrentScene) return false;

            return (Vector2.Distance(agent.transform.position, target.transform.position) < targetDist);
        }
    }
}
