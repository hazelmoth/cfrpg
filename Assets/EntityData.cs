using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Describes any object that can be placed on a tile (buildings, plants, boxes, etc.)
[System.Serializable]
public class EntityData
{
	public string entityId; 

	public GameObject entityPrefab;

	public List<Vector2Int> baseShape;

	// Defines the shape of the entity's base; true = a tile that the entity covers.
	// Can be any size.
	// For example:
	//	 true false
	//	 true true
	// would define a 2x2 L-shaped entity.
}
