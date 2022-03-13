using System.Collections.Generic;
using ContentLibraries;
using UnityEngine;

/**
 * A MapUnit stores what materials a particular tile is made up of, and what
 * entity is placed on it, if any.
 */
public class MapUnit
{
    /// How many ticks from watering until this tile is no longer moist.
    private static readonly ulong TicksUntilDry = (ulong)(TimeKeeper.TicksPerInGameDay * 0.6);

    public GroundMaterial groundMaterial;
    public GroundMaterial groundCover;
    public GroundMaterial cliffMaterial;
    // Whether this tile is a lower elevation near water.
    public bool isBeach;
    // Whether this tile is outside the playable area.
    public bool outsideMapBounds;
    // The tick for the time this tile was last moisturized.
    public ulong lastMoisturizedTick;
    // The ID of the entity on this tile. Null if there is none.
    public string entityId;
    // Where this part of the entity is relative to the origin, for multi-tile entities.
    public Vector2Int relativePosToEntityOrigin;
    // The save tags for the entity on this tile.
    public List<SavedComponentState> savedComponents;

    /// Whether this tile is currently moist.
    public bool IsMoist => lastMoisturizedTick + TicksUntilDry > TimeKeeper.CurrentTick;

    public MapUnit()
    {
        savedComponents = new List<SavedComponentState>();
    }

    public override string ToString()
    {
        return $"{nameof(entityId)}: {entityId}, "
            + $"{nameof(savedComponents)}: {savedComponents}, "
            + $"{nameof(relativePosToEntityOrigin)}: {relativePosToEntityOrigin}, "
            + $"{nameof(groundMaterial)}: {groundMaterial}, "
            + $"{nameof(groundCover)}: {groundCover}, "
            + $"{nameof(cliffMaterial)}: {cliffMaterial}, "
            + $"{nameof(isBeach)}: {isBeach}, "
            + $"{nameof(outsideMapBounds)}: {outsideMapBounds}, "
            + $"{nameof(lastMoisturizedTick)}: {lastMoisturizedTick}";
    }

    /// Sets this map unit's tile on the specified layer.
    public void SetTile(TilemapLayer layer, string groundMatId)
    {
        switch (layer)
        {
            case TilemapLayer.Ground:
                groundMaterial = ContentLibrary.Instance.GroundMaterials.Get(groundMatId);
                break;
            case TilemapLayer.GroundCover:
                groundCover = ContentLibrary.Instance.GroundMaterials.Get(groundMatId);
                break;
            case TilemapLayer.Cliff:
                cliffMaterial = ContentLibrary.Instance.GroundMaterials.Get(groundMatId);
                break;
            default:
                Debug.LogError($"Unknown tilemap layer {layer}");
                break;
        }
    }

    /// Whether actors can walk on this tile.
    public bool IsWalkable()
    {
        return !outsideMapBounds
            && groundMaterial is { isImpassable: false }
            && groundCover is not { isImpassable: true }
            && cliffMaterial is not { isImpassable: true }
            && (entityId == null || ContentLibrary.Instance.Entities.Get(entityId).CanBeWalkedThrough);
    }
}
