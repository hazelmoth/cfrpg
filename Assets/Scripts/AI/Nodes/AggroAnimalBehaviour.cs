using System.Linq;
using UnityEngine;

namespace AI.Nodes
{
    /**
     * A behaviour tree for a creature that attacks on sight.
     */
    public class AggroAnimalBehaviour : Node
    {
        private const float detectionDistance = 8;
        
        private Actor actor;
        private Actor currentTarget;
        private Node currentChild;

        public AggroAnimalBehaviour(Actor agent)
        {
            this.actor = agent;
        }
        
        protected override void Init()
        {
            currentChild = new Conditional(
                () => (currentTarget != null && Vector2.Distance(actor.Location.Vector2, currentTarget.Location.Vector2) 
                          < detectionDistance) 
                      || GetNearbyTarget() != null,
                () => new MeleeFight(
                    actor, 
                    currentTarget != null
                    && Vector2.Distance(actor.Location.Vector2, currentTarget.Location.Vector2) < detectionDistance
                        ? currentTarget
                        : GetNearbyTarget()),
                () => new Wander(actor));
        }

        protected override void OnCancel()
        {
            if (currentChild != null && !currentChild.Stopped) currentChild.Cancel();
        }

        protected override Status OnUpdate()
        {
            // Child node should never be null. If it is, report a failure.
            return currentChild?.Update() ?? Status.Failure;
        }

        /**
         * Returns some actor within the detection distance from the agent, if
         * there are any; otherwise returns null.
         */
        private Actor GetNearbyTarget()
        {
            foreach (Actor otherActor in ActorRegistry.GetAllIds().Select(id => ActorRegistry.Get(id).actorObject))
            {
                if (otherActor == null) continue;
                if (otherActor.ActorId == actor.ActorId) continue;
                if (otherActor.CurrentScene != actor.CurrentScene) continue;
                if (otherActor.GetData().RaceId == actor.GetData().RaceId) continue;
                if (Vector2.Distance(otherActor.Location.Vector2, actor.Location.Vector2) < detectionDistance)
                    return otherActor;
            }
            return null;
        }
    }
}