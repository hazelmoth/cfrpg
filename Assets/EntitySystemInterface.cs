using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySystemInterface : MonoBehaviour
{
	public static EntityData GetEntityAtTile (string scene, int relativeX, int relativeY) {
		// Returns null if there are no entities.
		// Otherwise returns the entity at the tile, including any multi-tile object covering the tile.

		return null;
	} 
}
