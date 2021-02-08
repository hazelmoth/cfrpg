using System;
using Popcron.Console;
using UnityEngine;

namespace ContinentMaps
{
    // Provides static methods for moving the player to different regions in the world.
    public static class RegionTravel
    {
        private const float FadeTime = 0.5f;
        
        // Loads the region with the given ID and moves the player to the given tile in its outside scene, if the region
        // exists. Calls the given method after loading finishes with true if successful, or false otherwise.
        private static void AttemptTravel (Actor playerActor, Vector2Int regionCoords, Vector2Int arrivalTile, Direction arrivalDir, Action<bool> callback)
        {
            ScreenFadeAnimator.FadeOut(FadeTime);
            PauseManager.Pause();
            ContinentManager.GetRegion(regionCoords.x, regionCoords.y,
                (success, map) =>
                {
                    if (success)
                    {
                        RegionMapManager.CurrentRegionCoords = regionCoords;
                    }
                    RegionLoaded(success, playerActor.ActorId, map, arrivalTile, arrivalDir);
                    callback?.Invoke(success);
                });

            // Callback helper method to build the region in the scene after it has been retrieved
            static void RegionLoaded(bool success, string playerID, RegionMap map, Vector2Int arrivalTile, Direction arrivalDir)
            {
                if (success)
                {
                    RegionMapManager.LoadMap(map);
                    // Load the player in the scene
                    ActorSpawner.Spawn(playerID, arrivalTile + new Vector2(0.5f, 0.5f), SceneObjectManager.WorldSceneId,
                        arrivalDir);
                }
                PauseManager.Unpause();
                ScreenFadeAnimator.FadeIn(FadeTime);
            }
        }

        // Moves the player to the adjacent region in the given direction, if such a region exists and is navigable.
        // When finished, passes false to the given callback if the region is off the map or not navigable, or true otherwise.
        public static void TravelToAdjacent(Actor playerActor, Direction direction, Action<bool> callback)
        {
            Actor player = ActorRegistry.Get(PlayerController.PlayerActorId).actorObject;
            Vector2Int dest = RegionMapManager.CurrentRegionCoords + direction.ToVector2().ToVector2Int();
            Vector2Int tileDest = player.Location.Vector2.ToVector2Int();
            if (direction == Direction.Left) tileDest.x = SaveInfo.RegionSize.x - 1;
            if (direction == Direction.Right) tileDest.x = 0;
            if (direction == Direction.Up) tileDest.y = 0;
            if (direction == Direction.Down) tileDest.y = SaveInfo.RegionSize.y - 1;
            
            AttemptTravel(player, dest, tileDest, direction, callback);
        }

        [Command("GoRight")]
        public static void GoRight()
        {
            Actor player = ActorRegistry.Get(PlayerController.PlayerActorId).actorObject;
            TravelToAdjacent(player, Direction.Right, null);
        }
    }
}