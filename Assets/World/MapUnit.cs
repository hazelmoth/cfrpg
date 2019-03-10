using UnityEngine;

// defines everything that's on a particular tile
public class MapUnit
{
	public string entityId;
	public Vector2Int relativePosToEntityOrigin;
	public GroundMaterial groundMaterial; // the material on the ground
}
