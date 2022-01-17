using UnityEngine;

namespace AI.Trees.Nodes
{
    /// Has the agent sleep in a given bed. Moves the agent's position into the bed
    /// regardless of initial position. Instantly returns Success, unless the bed is null.
    public class StartSleepingInBed : Node
    {
        private readonly Actor agent;
        private readonly IBed targetBed;

        public StartSleepingInBed(Actor agent, IBed bed)
        {
            this.agent = agent;
            targetBed = bed;
        }

        protected override void Init() { }

        protected override void OnCancel() { }

        protected override Status OnUpdate()
        {
            if (targetBed == null) return Status.Failure;

            agent.transform.position = targetBed.SleepPositionWorldCoords;

            // Set the agent sleeping.
            agent.GetData().Health.Sleep(targetBed);
            agent.GetComponent<ActorAnimController>().SetDirection(Direction.Down);

            return Status.Success;
        }
    }
}
