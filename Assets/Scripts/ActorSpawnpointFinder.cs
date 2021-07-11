using System.Collections.Generic;
using ContentLibraries;
using UnityEngine;

public static class ActorSpawnpointFinder
{
	// How far we're willing to search around our target coordinates before giving up
	private const int MaxSearchRadius = 100;

	// Finds a suitable spawn point in the given region and scene.
	public static Vector2 FindSpawnPoint (RegionMap map, string scene)
	{
		float x = Random.value * SaveInfo.RegionSize.x;
		float y = Random.value * SaveInfo.RegionSize.y;
		return FindSpawnPointNearCoords(map, scene, new Vector2(x, y));
	}

	// Finds a spawn point in the given region and scene, as close to the given coordinates as possible.
	public static Vector2 FindSpawnPointNearCoords (RegionMap map, string scene, Vector2 coords)
	{
		for (int i = 0; i < MaxSearchRadius; i++)
		{
			List<Vector2> vectors = GenerateSquareRing(i);
			vectors.Shuffle();
			foreach (Vector2 vector2 in vectors)
			{
				Vector2 currentVector2 = vector2 + coords;
				MapUnit unit = map.mapDict[scene][coords.ToVector2Int()];
				
				if (unit == null || unit.groundMaterial.isWater || unit.groundMaterial.isImpassable) continue;
				if (unit.cliffMaterial != null && unit.cliffMaterial.isImpassable) continue;
				if (unit.entityId != null &&
				    !ContentLibrary.Instance.Entities.Get(unit.entityId).canBeWalkedThrough) continue;
				
				return currentVector2;
			}
		}
		Debug.LogError("No suitable spawn point found.");
		return Vector2.zero;
	}

	private static List<Vector2> GenerateSquareRing (int radius)
	{
		List<Vector2> points = new List<Vector2>();

		if (radius <= 0)
		{
			points.Add(Vector2.zero);
			return points;
		}

		for (int i = -radius; i <= radius; i++)
		{
			// Horizontal lines
			points.Add(new Vector2(i, radius));
			points.Add(new Vector2(i, -radius));
		}
		for (int i = -radius + 1; i <= radius - 1; i++)
		{
			// Vertical lines
			points.Add(new Vector2(radius, i));
			points.Add(new Vector2(-radius, i));
		}

		return points;
	}
}
