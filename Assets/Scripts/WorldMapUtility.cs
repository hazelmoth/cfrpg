using UnityEngine;

// Methods for manipulating maps that aren't necessarily loaded into the scene
public static class WorldMapUtility
{
	public static void AddEntityToMap (EntityData entity, Vector2Int point, string scene, WorldMap map) {
		foreach (Vector2Int entitySection in entity.baseShape) {
			if (!map.mapDict [scene].ContainsKey (point + entitySection)) {
				map.mapDict [scene].Add((point + entitySection), new MapUnit());
			}
			map.mapDict [scene] [point + entitySection].entityId = entity.entityId;
			map.mapDict [scene] [point + entitySection].relativePosToEntityOrigin = point;
		}
	}
}
