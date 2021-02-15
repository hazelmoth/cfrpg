using System;
using System.Collections;
using Popcron.Console;
using UnityEngine;

namespace ContinentMaps
{
    // Provides static methods for moving the player to different regions in the world.
    public static class RegionTravel
    {
        private static bool regionTravelInProgress;
        private static Coroutine travelCoroutine;
        
        private const float FadeTime = 0.5f;
        private const float RegionLoadTimeout = 5f;
        
        // Fades the screen to black, saves the current region to ContinentManager, attempts to load the region with
        // given coords, and moves the player to given tile and facing the given direction in the new region's outside 
        // scene. Calls back false if the region failed to load, or if region traversal was already
        // in progress; calls back true otherwise.
        private static void AttemptTravel(
            string playerId,
            Vector2Int regionCoords,
            Vector2Int arrivalTile,
            Direction arrivalDir,
            Action<bool> callback)
        {
            // To handle cases when the coroutine stops prematurely
            if (GlobalCoroutineObject.Instance == null)
            {
                regionTravelInProgress = false;
            }
            
            // Ignore requests if travel is already happening
            if (regionTravelInProgress)
            {
                callback?.Invoke(false);
                return;
            }
            travelCoroutine = GlobalCoroutineObject.Instance.StartCoroutine(AttemptTravelCoroutine(
                playerId, regionCoords, arrivalTile, 
                arrivalDir, callback));
        }

        private static IEnumerator AttemptTravelCoroutine(
            string playerID,
            Vector2Int regionCoords,
            Vector2Int arrivalTile,
            Direction arrivalDir,
            Action<bool> callback)
        {
            regionTravelInProgress = true;
            
            PauseManager.Pause();
            ScreenFadeAnimator.FadeOut(FadeTime);
            yield return new WaitForSecondsRealtime(FadeTime);

            float regionLoadStart = Time.unscaledTime;
            bool waitingForRegionLoad = true;
            bool regionLoadSucceeded = false;
            
            RegionMap loadedMap = null;
            ContinentManager.SaveRegion(RegionMapManager.GetRegionMap(), RegionMapManager.CurrentRegionCoords);
            ContinentManager.GetRegion(regionCoords.x, regionCoords.y,
                (success, map) =>
                {
                    waitingForRegionLoad = false;
                    regionLoadSucceeded = success;
                    loadedMap = map;
                });
            
            // Wait for region loading to finish
            while (waitingForRegionLoad)
            {
                // Check for time out
                if (Time.unscaledTime - regionLoadStart > RegionLoadTimeout)
                {
                    waitingForRegionLoad = false;
                    regionLoadSucceeded = false;
                    break;
                }
                yield return null;
            }

            if (regionLoadSucceeded)
            {
                RegionMapManager.CurrentRegionCoords = regionCoords;
                RegionMapManager.LoadMap(loadedMap);
                ScenePortalLibrary.BuildLibrary();
                // Load the player in the scene
                ActorSpawner.Spawn(playerID, arrivalTile + new Vector2(0.5f, 0.5f), SceneObjectManager.WorldSceneId,
                    arrivalDir);
            }
            
            PauseManager.Unpause();
            ScreenFadeAnimator.FadeIn(FadeTime);
            callback?.Invoke(regionLoadSucceeded);
            yield return new WaitForSecondsRealtime(FadeTime);
            regionTravelInProgress = false;
        }

        // Moves the player to the adjacent region in the given direction, if such a region exists and is navigable.
        // When finished, passes false to the given callback if the region is off the map or not navigable, or true otherwise.
        public static void TravelToAdjacent(Actor player, Direction direction, Action<bool> callback)
        {
            Vector2Int dest = RegionMapManager.CurrentRegionCoords + direction.ToVector2().ToVector2Int();
            Vector2Int tileDest = player.Location.Vector2.ToVector2Int();
            if (direction == Direction.Left) tileDest.x = SaveInfo.RegionSize.x - 1;
            if (direction == Direction.Right) tileDest.x = 0;
            if (direction == Direction.Up) tileDest.y = 0;
            if (direction == Direction.Down) tileDest.y = SaveInfo.RegionSize.y - 1;
            
            AttemptTravel(player.ActorId, dest, tileDest, direction, callback);
        }

        [Command("GoRight")]
        public static void GoRight()
        {
            Actor player = ActorRegistry.Get(PlayerController.PlayerActorId).actorObject;
            TravelToAdjacent(player, Direction.Right, null);
        }
    }
}