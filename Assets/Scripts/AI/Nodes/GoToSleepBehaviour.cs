using ActorComponents;
using ContinentMaps;
using SettlementSystem;
using UnityEngine;

namespace AI.Nodes
{
    /// A behaviour node that makes an agent go to their bed and sleep. If the agent has
    /// no bed, will immediately return Failure; otherwise, returns Running.
    public class GoToSleepBehaviour : Node
    {
        /// If an actor is too far from their bed, we wake them up.
        private const float MaxSleepDist = 0.3f;
        /// How close we must be to the bed initiate sleep.
        private const float MaxStartSleepDist = 1.5f;

        private readonly Actor agent;
        private readonly ActorHealth health;
        private SettlementManager settlementManager;
        private IBed targetBed;
        private Vector2 outOfBedWorldPos;
        private Node subNode;
        private Node navNode;

        public GoToSleepBehaviour(Actor agent)
        {
            this.agent = agent;
            health = agent.GetData().Get<ActorHealth>();
            Debug.Assert(health != null, "Actor has no health!");
        }

        protected override void Init()
        {
            settlementManager = GameObject.FindObjectOfType<SettlementManager>();

            if (settlementManager == null)
                Debug.LogError("No SettlementManager found in scene!");

            outOfBedWorldPos = agent.transform.position;

            subNode = new Repeater(
                () => new Conditional(
                    () => health.Sleeping,
                    // Currently asleep: either wake from distance, or do nothing.
                    () => new Conditional(
                        () => Vector2.Distance(agent.transform.position, targetBed.SleepPositionWorldCoords) > MaxSleepDist,
                        () => new Execute(
                            () =>
                            {
                                health.WakeUp();
                                agent.transform.position = outOfBedWorldPos;
                            }),
                        () => new Wait(1)),
                    // Not asleep: go to bed
                    () => new Conditional(
                        () => Vector2.Distance(agent.transform.position, targetBed.SleepPositionWorldCoords)
                            < MaxStartSleepDist,
                        // If we're close, start sleeping
                        () => new Sequencer(
                            () => new Execute(() => outOfBedWorldPos = agent.transform.position),
                            () => new StartSleepingInBed(agent, targetBed)),
                        // Otherwise navigate to the bed
                        () => new NavigateNextToThing(
                            agent,
                            ((MonoBehaviour)targetBed).gameObject,
                            ((MonoBehaviour)targetBed).GetComponent<EntityObject>().Scene))));
        }

        protected override void OnCancel()
        {
            if (navNode is { Stopped: false }) navNode.Cancel();
            subNode.Cancel();

            if (health.Sleeping)
                agent.transform.position = outOfBedWorldPos;

            health.WakeUp();
        }

        protected override Status OnUpdate()
        {
            if (health == null) return Status.Failure;
            
            string region = ContinentManager.CurrentRegionId;
            string scene = settlementManager.GetHomeScene(agent.ActorId, region);
            if (scene == null) return Status.Failure;

            BuildingInfo buildingInfo = settlementManager.GetBuildingInfo(scene, region);
            if (buildingInfo == null) return Status.Failure;

            if (targetBed == null)
            {
                // This may not recognize if the agent switches houses or beds while this
                // node is running. Could become an issue.
                targetBed = SceneObjectManager.GetSceneObjectFromId(scene).GetComponentInChildren<IBed>();
                if (targetBed == null) return Status.Failure;
            }

            // The bed seems to exist. :>
            return subNode.Update();
        }
    }
}
