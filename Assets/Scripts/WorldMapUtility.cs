using UnityEngine;

// Methods for manipulating maps that aren't necessarily loaded into the scene
public static class WorldMapUtility
{
	public static void AddEntityToMap (EntityData entity, Vector2Int point, string scene, RegionMap map) {
		foreach (Vector2Int entitySection in entity.BaseShape) {
			if (!map.mapDict [scene].ContainsKey (point + entitySection)) {
				map.mapDict [scene].Add((point + entitySection), new MapUnit());
			}
			map.mapDict [scene] [point + entitySection].entityId = entity.Id;
			map.mapDict [scene] [point + entitySection].relativePosToEntityOrigin = point;
		}
	}
}
