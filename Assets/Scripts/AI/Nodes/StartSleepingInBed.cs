using ActorAnim;
using ActorComponents;
using UnityEngine;

namespace AI.Nodes
{
    /// Has the agent sleep in a given bed. Moves the agent's position into the bed
    /// regardless of initial position. Instantly returns Success, unless the bed is null.
    public class StartSleepingInBed : Node
    {
        private readonly Actor agent;
        private readonly ActorHealth health;
        private readonly IBed targetBed;

        public StartSleepingInBed(Actor agent, IBed bed)
        {
            this.agent = agent;
            targetBed = bed;
            health = agent.GetData().Get<ActorHealth>();
            
            if (health == null)
            {
                Debug.LogError("StartSleepingInBed: Agent has no health component");
            }
        }

        protected override void Init() { }

        protected override void OnCancel() { }

        protected override Status OnUpdate()
        {
            if (targetBed == null) return Status.Failure;

            agent.transform.position = targetBed.SleepPositionWorldCoords;

            // Set the agent sleeping.
            health.Sleep(targetBed);
            agent.GetComponent<ActorSpriteController>().ForceDirection(Direction.Down);

            return Status.Success;
        }
    }
}
