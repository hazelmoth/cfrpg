using System.Collections.Generic;
using UnityEngine;

/**
 * A MapUnit stores what materials a particular tile is made up of, and what
 * entity is placed on it, if any.
 */
public class MapUnit
{
    public GroundMaterial cliffMaterial;

    // The ID of the entity on this tile. Null if there is none.
    public string entityId;
    public GroundMaterial groundCover;
    public GroundMaterial groundMaterial;
    // Whether this tile is a lower elevation near water.
    public bool isBeach;
    // Whether this tile is outside the playable area.
    public bool outsideMapBounds;
    // Where this part of the entity is relative to the origin, for multi-tile entities.
    public Vector2Int relativePosToEntityOrigin;
    // The save tags for the entity on this tile.
    public List<SavedComponentState> savedComponents;

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
            + $"{nameof(outsideMapBounds)}: {outsideMapBounds}";
    }
}
