using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MyBox;
using UnityEngine;

namespace ContinentMaps
{
    /// Provides static methods for moving the player to different regions in the world.
    public static class RegionTravel
    {
        private const float FadeTime = 0.5f;
        private const float RegionLoadTimeout = 5f;
        private static bool regionTravelInProgress;


        /// Moves the player to the adjacent region in the given direction, if such a
        /// region exists and is navigable. When finished, passes false to the given
        /// callback if the region is off the map or not navigable, or true otherwise.
        public static void TravelToAdjacent(Actor player, Direction direction, string entryPortalTag, Action<bool> callback)
        {
            RegionInfo currentRegionInfo = ContinentManager.CurrentRegion.info;
            RegionInfo.RegionConnection? connection = currentRegionInfo.connections?
                .Where(
                    conn => conn.direction == direction
                        && (conn.portalTag == entryPortalTag
                            || (conn.portalTag.IsNullOrEmpty() && entryPortalTag.IsNullOrEmpty())))
                .FirstOrDefault();
            if (!connection.HasValue)
            {
                Debug.LogWarning($"Failed to find connecting region. Direction: {direction}. Tag: {entryPortalTag}");
                return;
            }
            if (connection.Value.destRegionId.IsNullOrEmpty())
            {
                Debug.LogError("Region connection has a null or empty destination ID!");
                return;
            }

            string dest = connection.Value.destRegionId;
            Debug.Log($"Travelling to {dest}. Direction: {direction}. Tag: {entryPortalTag}. Dest Tag: {connection.Value.destPortalTag}");

            AttemptTravel(player.ActorId, dest, connection.Value.destPortalTag, direction, true, callback);
        }

        /// Moves the player directly to the given region. The player will enter the new
        /// region through a random portal. Calls back true if loading the region was
        /// successful.
        public static void TravelTo(Actor player, string regionId, bool fadeScreen, Action<bool> callback)
        {
            AttemptTravel(player.ActorId, regionId, null, player.Direction, fadeScreen, callback);
        }

        /// Fades the screen to black, saves the current region to ContinentManager, attempts to load the region with
        /// given id, and moves the player to given tile and facing the given direction in the new region's outside
        /// scene. Calls back false if the region failed to load, or if region traversal was already
        /// in progress; calls back true otherwise.
        private static void AttemptTravel(
            string playerId,
            string regionId,
            string targetPortalTag,
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

            GlobalCoroutineObject.Instance.StartCoroutine(
                AttemptTravelCoroutine(
                    playerId,   regionId,   targetPortalTag,
                    arrivalDir, fadeScreen, callback));
        }

        /// (1) Loads the scene with the given ID.
        /// (2) Finds a portal with the given tag if not null, or any portal in the right
        ///   direction otherwise, and spawns the actor with the given ID on front of that
        ///   portal.
        /// (3) Invokes the callback with true if the load was successful.
        private static IEnumerator AttemptTravelCoroutine(
            string playerID,
            string regionId,
            string destPortalTag,
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
            ContinentManager.SaveRegion(RegionMapManager.GetRegionMap(true), ContinentManager.CurrentRegionId);
            ContinentManager.GetRegion(regionId,
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
                ContinentManager.CurrentRegionId = regionId;
                RegionMapManager.LoadMap(loadedMap);
                ScenePortalLibrary.BuildLibrary();

                // Find all portals in the target scene with a matching tag.
                List<RegionPortal> portals = GameObject.FindObjectsOfType<RegionPortal>()
                    .Where(p => destPortalTag == null || p.PortalTag == destPortalTag)
                    .ToList();

                // If possible, choose portals facing the specified direction.
                List<RegionPortal> preferredPortals =
                    portals.Where(p => p.ExitDirection.Invert() == arrivalDir).ToList();

                if (preferredPortals.Any()) portals = preferredPortals;

                // Choose a random portal from the ones that match.
                RegionPortal portal = portals.Any() ? portals.PickRandom() : null;

                Vector2Int arrivalTile;
                if (portal != null)
                {
                    arrivalTile = portal.GetComponent<EntityObject>().Location.Vector2Int
                        + portal.ExitDirection.Invert().ToVector2().ToVector2Int();
                }
                else
                {
                    Debug.LogError("Failed to find a suitable region portal for region entry.");
                    arrivalTile =
                        ActorSpawnpointFinder.FindSpawnPoint(RegionMapManager.GetRegionMap(), regionId)
                        .ToVector2Int();
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
