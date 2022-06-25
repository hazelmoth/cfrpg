using System.Linq;
using ContinentMaps;
using Dialogue;
using UnityEngine;

namespace AI.Trees.Nodes
{
    public class IntroGuy : Node
    {
        private static float MaxPlayerFollowDistance = 7f;
        private Actor agent;
        private Node subNode;

        public IntroGuy(Actor agent)
        {
            this.agent = agent;
        }

        protected override void Init()
        {
            Vector2Int portalPos = RegionMapManager.GetMapUnits(SceneObjectManager.WorldSceneId)
                .Where(u => u.Value.entityId == "region_exit_down")
                .Where(
                    u => RegionMapManager.GetEntityObjectAtPoint(u.Key, SceneObjectManager.WorldSceneId)
                            .GetComponent<RegionPortal>()
                            .ExitDirection
                        == Direction.Down)
                .Select(u => u.Key)
                .PickRandom();

            RegionPortal portal = RegionMapManager
                .GetEntityObjectAtPoint(portalPos, SceneObjectManager.WorldSceneId)
                .GetComponent<RegionPortal>();

            // 1. Wait for the player's arrival
            // 2. Initiate dialogue with the player
            // 3. Wait for the player to finish the dialogue
            // 4. Walk to the region portal, waiting for player to follow
            // 5. Travel to target region
            subNode = new Sequencer(
                () => new WaitUntil(() => PlayerController.GetPlayerActor() != null),
                () => new Execute(() => Object.FindObjectOfType<DialogueManager>().InitiateDialogue(agent)),
                () => new Wait(1),
                () => new WaitUntil(() => !agent.InDialogue),
                () => new Repeater(
                    () => new Conditional(
                        () => CheckDistance(agent, PlayerController.GetPlayerActor(), MaxPlayerFollowDistance),
                        () => new GoTo(
                            agent,
                            new Location(
                                portalPos + new Vector2(0.5f, 1.5f),
                                SceneObjectManager.WorldSceneId),
                            1f),
                        () => new InstantFailer()),
                    finishOnSuccess: true),
                () => new Execute(
                    () => RegionTravel.OfflineTravel(
                        agent.ActorId,
                        ContinentManager.CurrentRegion.info.connections
                            .Where(c => c.direction == Direction.Down)
                            .PickRandom()
                            .destRegionId)));
        }

        protected override void OnCancel()
        {
            subNode.Cancel();
        }

        protected override Status OnUpdate()
        {
            return subNode.Update();
        }

        private bool CheckDistance(Actor a1, Actor a2, float maxDistance)
        {
            if (a1.CurrentScene != a2.CurrentScene) return false;
            return Vector2.Distance(a1.transform.position, a2.transform.position) < maxDistance;
        }
    }
}
