using System.Collections.Generic;
using System.Collections.Immutable;
using UnityEngine;

/// A set of utility methods for handling RegionMap.
public static class RegionMapUtil
{
    private const float EntityPlacementRadiusPerRot = 10;
    private const float EntityPlacementDegreesPerAttempt = 20;
    private const string WorldSceneName = SceneObjectManager.WorldSceneId;
    private static readonly int RegionSizeX = SaveInfo.RegionSize.x;
    private static readonly int RegionSizeY = SaveInfo.RegionSize.y;

    /// Attempts to place the given entity on the map somewhere near the given target position over the
    /// given number of attempts, moving farther from that position for each failed attempt. Returns false
    /// if all attempts fail.
    public static bool AttemptPlaceEntity(
        this RegionMap map,
        EntityData entity,
        int attempts,
        Vector2 targetPos,
        PlacementSettings placementSettings)
    {
        return map.AttemptPlaceEntity(entity, attempts, targetPos, placementSettings, out Vector2Int _);
    }

    /// Attempts to place the given entity on the map somewhere near the given target position over the
    /// given number of attempts, moving farther from that position for each failed attempt. Returns false
    /// if all attempts fail.
    public static bool AttemptPlaceEntity(
        this RegionMap map,
        EntityData entity,
        int attempts,
        Vector2 targetPos,
        PlacementSettings placementSettings,
        out Vector2Int placedAt)
    {
        for (int i = 0; i < attempts; i++)
        {
            float rot = i * EntityPlacementDegreesPerAttempt;
            Vector2 pos = GenerationHelper.Spiral(EntityPlacementRadiusPerRot, 0f, false, rot);
            pos += targetPos;
            int tileX = Mathf.FloorToInt(pos.x);
            int tileY = Mathf.FloorToInt(pos.y);
            if (tileX > RegionSizeX || tileX < 0 || tileY > RegionSizeY || RegionSizeY < 0)
            {
                if (attempts == 1)
                    Debug.LogWarning(
                        $"Placement out of bounds: ({tileX}, {tileY}) for entity: {entity.Id}\n"
                        + $"(Region size is {RegionSizeX} by {RegionSizeY})");
                continue;
            }

            bool failure = false;
            // Check that each tile the entity is placed over is buildable
            foreach (Vector2Int basePosition in entity.BaseShape)
            {
                Vector2Int absolute = new Vector2Int(basePosition.x + tileX, basePosition.y + tileY);
                if (map.mapDict[WorldSceneName].ContainsKey(absolute))
                {
                    MapUnit mapUnit = map.mapDict[WorldSceneName][absolute];
                    if (!mapUnit.groundMaterial.isWater
                        && mapUnit.cliffMaterial == null
                        && (mapUnit.groundMaterial == null || !mapUnit.groundMaterial.isImpassable)
                        && (mapUnit.entityId == null || placementSettings.WillPlaceOver(mapUnit.entityId))) continue;
                }
                failure = true;
                break;
            }

            if (failure) continue;

            // It seems all of the tiles are buildable, so let's actually place the entity
            foreach (Vector2Int basePosition in entity.BaseShape)
            {
                Vector2Int absolute = new Vector2Int(basePosition.x + tileX, basePosition.y + tileY);
                MapUnit mapUnit = map.mapDict[WorldSceneName][absolute];
                mapUnit.groundCover = null;
                mapUnit.entityId = entity.Id;
                mapUnit.relativePosToEntityOrigin = basePosition;
            }
            // TODO remove multi-tile entities if we place over part of them
            placedAt = new Vector2Int(tileX, tileY);
            return true;
        }

        placedAt = Vector2Int.zero;
        return false;
    }

    /**
     * Settings defining which entities may be overwritten during entity placement.
     */
    public readonly struct PlacementSettings
    {
        private PlacementSettings(bool listIsWhitelist, ImmutableList<string> entityExceptionList)
        {
            this.listIsWhitelist = listIsWhitelist;
            this.entityExceptionList = entityExceptionList.ToImmutableHashSet();
        }

        /// If true, the exception list contains the only entities that will be placed
        /// over; otherwise, it contains the only entities that won't be placed over.
        private readonly bool listIsWhitelist;
        private readonly ISet<string> entityExceptionList;

        public static PlacementSettings PlaceOverAnything =>
            new PlacementSettings(false, ImmutableList.Create<string>());
        public static PlacementSettings PlaceOverNothing =>
            new PlacementSettings(true, ImmutableList.Create<string>());

        public static PlacementSettings PlaceOverAnythingExcept(ImmutableList<string> entityBlacklist)
        {
            return new PlacementSettings(false, entityBlacklist);
        }

        public static PlacementSettings PlaceOverOnly(ImmutableList<string> entityWhitelist)
        {
            return new PlacementSettings(true, entityWhitelist);
        }

        public bool WillPlaceOver(string entityId)
        {
            return entityExceptionList.Contains(entityId) ? listIsWhitelist : !listIsWhitelist;
        }
    }
}