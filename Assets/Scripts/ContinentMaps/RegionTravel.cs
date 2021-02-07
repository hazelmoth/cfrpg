using System.Collections;
using System.Collections.Generic;
using Popcron.Console;
using UnityEngine;

namespace ContinentMaps
{
    public static class RegionTravel
    {
        public static void MoveToRegion (Actor playerActor, string regionId, Vector2Int arrivalTile)
        {
            ContinentManager.GetRegion(RegionMapManager.CurrentRegionCoords.x,
                RegionMapManager.CurrentRegionCoords.y,
                (success, map) =>
                {
                    OnRegionLoaded(success, map, arrivalTile);
                });
        }

        private static void OnRegionLoaded(bool success, RegionMap map, Vector2Int arrivalTile)
        {
            if (!success) return;
            RegionMapManager.LoadMap(map);
            Actor player = ActorSpawner.Spawn(PlayerController.PlayerActorId, arrivalTile, SceneObjectManager.WorldSceneId,
                Direction.Down);
            PlayerController.SetPlayerActor(player.ActorId);
        }
        
        [Command("GoLeft")]
        public static void GoLeft()
        {
            Actor player = ActorRegistry.Get(PlayerController.PlayerActorId).actorObject;
            Vector2Int dest = RegionMapManager.CurrentRegionCoords + Vector2Int.left;
            MoveToRegion(player, SceneObjectManager.WorldSceneId, Vector2Int.zero);
        }
    }
}