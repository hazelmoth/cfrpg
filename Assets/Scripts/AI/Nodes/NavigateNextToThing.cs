using System.Collections.Generic;
using System.Linq;
using ContentLibraries;
using MyBox;
using UnityEngine;

namespace AI.Nodes
{
    /// A node that has an agent navigate to a tile adjacent to some thing.
    /// Returns success when the agent is adjacent to the target. Immediately returns
    /// failure if no adjacent tiles are accessible.
    public class NavigateNextToThing : Node
    {
        private Actor agent;
        private GameObject target;
        private string targetScene;
        private TileLocation targetLocation;
        private Node subNode;

        public NavigateNextToThing(Actor agent, GameObject target, string targetScene)
        {
            this.agent = agent;
            this.target = target;
            this.targetScene = targetScene;
        }

        protected override void Init()
        {
            if (TryFindAdjacentTile(target, targetScene, out targetLocation))
                subNode = new GoTo(agent, targetLocation, 0.1f);
        }

        protected override void OnCancel()
        {
            subNode?.Cancel();
        }

        protected override Status OnUpdate()
        {
            return subNode?.Update() ?? Status.Failure;
        }

        /// Searches for valid tiles adjacent to the target. Returns the closest one to
        /// the agent.
        private bool TryFindAdjacentTile(GameObject gameObject, string scene, out TileLocation navDest)
        {
            Vector2Int rootPos = TilemapInterface.WorldPosToScenePos(gameObject.transform.position, scene).ToVector2Int();

            List<Vector2Int> objectTiles = new();
            if (gameObject.TryGetComponent(out EntityObject entity))
            {
                objectTiles.AddRange(
                    from Vector2Int pos in ContentLibrary.Instance.Entities.Get(entity.EntityId).BaseShape
                    select rootPos + pos);
            }
            else objectTiles.Add(rootPos);

            List<Vector2Int> validAdjacentTiles =
                (from Vector2Int pos in objectTiles
                    from tile in Pathfinder.GetValidAdjacentTiles(scene, pos.ToVector2(), null)
                    where !objectTiles.Contains(tile)
                    select tile).ToList();

            if (validAdjacentTiles.Count == 0)
            {
                // No valid adjacent tiles exist
                Debug.LogWarning(agent.name + " tried to navigate to an object with no valid adjacent tiles", gameObject);
                navDest = null;
                return false;
            }

            // Find the closest valid adjacent tile using straight line distance
            Vector2Int agentPos =
                TilemapInterface.WorldPosToScenePos(agent.transform.position, scene).ToVector2Int();
            Vector2Int closest =
                validAdjacentTiles.OrderBy(tile => (tile - agentPos).magnitude).First();

            navDest = new TileLocation(closest, scene);
            return true;
        }
    }
}
