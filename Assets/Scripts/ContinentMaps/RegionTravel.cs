using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ContinentMaps
{
    /// Provides static methods for moving the player to different regions in the world.
    public static class RegionTravel
    {
        private const float FadeTime = 0.5f;
        private const float RegionLoadTimeout = 5f;
        private static bool regionTravelInProgress;
        private static Coroutine travelCoroutine;


        /// Moves the player to the adjacent region in the given direction, if such a
        /// region exists and is navigable. When finished, passes false to the given
        /// callback if the region is off the map or not navigable, or true otherwise.
        public static void TravelToAdjacent(Actor player, Direction direction, Action<bool> callback)
        {
            Vector2Int dest = RegionMapManager.CurrentRegionCoords + direction.ToVector2().ToVector2Int();
            Vector2Int tileDest = player.Location.Vector2.ToVector2Int();
            if (direction == Direction.Left) tileDest.x = SaveInfo.RegionSize.x - 1;
            if (direction == Direction.Right) tileDest.x = 0;
            if (direction == Direction.Up) tileDest.y = 0;
            if (direction == Direction.Down) tileDest.y = SaveInfo.RegionSize.y - 1;

            AttemptTravel(player.ActorId, dest, tileDest, direction, true, callback);
        }

        /// Moves the player directly to the given region. The player's location in the
        /// loaded region will be the same as in the current region. Calls back true if
        /// loading the region was successful.
        public static void TravelTo(Actor player, Vector2Int regionCoords, bool fadeScreen, Action<bool> callback)
        {
            Vector2Int tileDest = player.Location.Vector2.ToVector2Int();
            AttemptTravel(player.ActorId, regionCoords, tileDest, player.Direction, fadeScreen, callback);
        }

        /// Fades the screen to black, saves the current region to ContinentManager, attempts to load the region with
        /// given coords, and moves the player to given tile and facing the given direction in the new region's outside
        /// scene. Calls back false if the region failed to load, or if region traversal was already
        /// in progress; calls back true otherwise.
        private static void AttemptTravel(
            string playerId,
            Vector2Int regionCoords,
            Vector2Int arrivalTile,
            Direction arrivalDir,
            bool fadeScreen,
            Action<bool> callback)
        {
            // To handle cases when the coroutine stops prematurely
            if (GlobalCoroutineObject.Instance == null) regionTravelInProgress = false;

            // Ignore requests if travel is already happening
            if (regionTravelInProgress)
            {
                callback?.Invoke(false);
                return;
            }

            travelCoroutine = GlobalCoroutineObject.Instance.StartCoroutine(
                AttemptTravelCoroutine(
                    playerId, regionCoords, arrivalTile,
                    arrivalDir, fadeScreen, callback));
        }

        private static IEnumerator AttemptTravelCoroutine(
            string playerID,
            Vector2Int regionCoords,
            Vector2Int arrivalTile,
            Direction arrivalDir,
            bool fadeScreen,
            Action<bool> callback)
        {
            regionTravelInProgress = true;

            PauseManager.Pause();
            if (fadeScreen) 
            {
                ScreenFadeAnimator.FadeOut(FadeTime);
                yield return new WaitForSecondsRealtime(FadeTime);
            }

            float regionLoadStart = Time.unscaledTime;
            bool waitingForRegionLoad = true;
            bool regionLoadSucceeded = false;

            RegionMap loadedMap = null;
            ContinentManager.SaveRegion(RegionMapManager.GetRegionMap(true), RegionMapManager.CurrentRegionCoords);
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

                if (ContinentManager.LoadedMap.regionInfo[regionCoords.x, regionCoords.y].disableAutoRegionTravel)
                {
                    // Auto region travel is disabled, so this region probably uses portals.
                    // Let's find those.
                    List<RegionPortal> portals = GameObject.FindObjectsOfType<RegionPortal>().ToList();
                    RegionPortal portal = portals.FirstOrDefault(portal => portal.ExitDirection.Invert() == arrivalDir);
                    portal ??= portals.First();
                    if (portal != null)
                    {
                        arrivalTile = portal.GetComponent<EntityObject>().Location.Vector2Int + portal.ExitDirection.Invert().ToVector2().ToVector2Int();
                    }
                }

                // Load the player in the scene
                ActorSpawner.Spawn(
                    playerID,
                    arrivalTile + new Vector2(0.5f, 0.5f),
                    SceneObjectManager.WorldSceneId,
                    arrivalDir);
            }

            PauseManager.Unpause();
            callback?.Invoke(regionLoadSucceeded);
            regionTravelInProgress = false;
            if (fadeScreen)
            {
                ScreenFadeAnimator.FadeIn(FadeTime);
                yield return new WaitForSecondsRealtime(FadeTime);
            }
        }
    }
}
