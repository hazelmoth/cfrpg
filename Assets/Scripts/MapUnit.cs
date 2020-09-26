using UnityEngine;

// defines everything that's on a particular tile
public class MapUnit
{
	public string entityId;
	public Vector2Int relativePosToEntityOrigin;
	public GroundMaterial groundMaterial; // the material of the ground
	public GroundMaterial groundCover; // the material on the groundcover layer
}