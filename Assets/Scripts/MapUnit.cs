using System.Collections.Generic;
using UnityEngine;

/**
 * A MapUnit stores what materials a particular tile is made up of, and what
 * entity is placed on it, if any.
 */
public class MapUnit
{
	public MapUnit()
	{
		savedComponents = new List<SavedComponentState>();
	}
	
	// The ID of the entity on this tile. Null if there is none.
	public string entityId;
	// The save tags for the entity on this tile.
	public List<SavedComponentState> savedComponents;
	// Where this part of the entity is relative to the origin, for multi-tile entities.
	public Vector2Int relativePosToEntityOrigin;
	public GroundMaterial groundMaterial;
	public GroundMaterial groundCover;
	public GroundMaterial cliffMaterial;
}