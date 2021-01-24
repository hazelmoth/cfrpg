using System.Collections.Generic;
using UnityEngine;

// Allow an actor to search for and identify nearby objects
public static class NearbyObjectLocaterSystem
{

	public static List<GameObject> FindEntitiesInRange(Vector2 searchCenter, float searchRadius, string scene, List<string> entityIdsToSearch) 
	{
		List<GameObject> found = new List<GameObject> ();
		// Find all the tiles in a circle around the start point
		Vector2 center = TilemapInterface.WorldPosToScenePos (searchCenter,scene);

		for (int y = 0; y <= searchRadius; y++) {
			for (int x = 0; x <= searchRadius; x++) {
				for (int signy = -1; signy <= 1; signy += 2) {
					for (int signx = -1; signx <= 1; signx += 2) {

						Vector2 relativePos = new Vector2 (x * signx, y * signy);
						if (relativePos.magnitude > searchRadius)
							continue;

						Vector2 pos = center + relativePos;
						pos = TilemapInterface.FloorToTilePos (pos);

						MapUnit mapUnit = RegionMapManager.GetMapObjectAtPoint (Vector2Int.FloorToInt (pos), scene);
						if (mapUnit != null) {
							foreach (string id in entityIdsToSearch) {
								if (id == mapUnit.entityId)
									found.Add (RegionMapManager.GetEntityObjectAtPoint (Vector2Int.FloorToInt (pos), scene));
							}
						}
					}
				}
			}
		}
		return found;
	}
		
	public static List<GameObject> FindEntitiesWithComponent<Component>(Vector2 searchCenter, float searchRadius, string scene) 
	{
		List<GameObject> found = new List<GameObject> ();
		// Find all the tiles in a circle around the start point
		Vector2 center = TilemapInterface.WorldPosToScenePos (searchCenter,scene);

		for (int y = 0; y <= searchRadius; y++) {
			for (int x = 0; x <= searchRadius; x++) {
				for (int signy = -1; signy <= 1; signy += 2) {
					for (int signx = -1; signx <= 1; signx += 2) {

						Vector2 relativePos = new Vector2 (x * signx, y * signy);
						if (relativePos.magnitude > searchRadius)
							continue;

						Vector2 pos = center + relativePos;
						pos = TilemapInterface.FloorToTilePos (pos);

						MapUnit mapUnit = RegionMapManager.GetMapObjectAtPoint (Vector2Int.FloorToInt (pos), scene);
						if (mapUnit != null) {
							GameObject entity = RegionMapManager.GetEntityObjectAtPoint (Vector2Int.FloorToInt (pos), scene);
							if (entity != null && entity.GetComponent<Component>() != null)
								found.Add (entity);
						}
					}
				}
			}
		}
		return found;
	}
		
	public static GameObject FindClosestEntityInRange(Vector2 searchCenter, float searchRadius, string scene, List<string> entityIdsToSearch) 
	{
		Vector2 center = TilemapInterface.WorldPosToScenePos (searchCenter,scene);

		for (int y = 0; y <= searchRadius; y++) {
			for (int x = 0; x <= searchRadius; x++) {
				for (int signy = -1; signy <= 1; signy += 2) {
					for (int signx = -1; signx <= 1; signx += 2) {

						Vector2 relativePos = new Vector2 (x * signx, y * signy);
						if (relativePos.magnitude > searchRadius)
							continue;

						Vector2 pos = center + relativePos;
						pos = TilemapInterface.FloorToTilePos (pos);

						MapUnit mapUnit = RegionMapManager.GetMapObjectAtPoint (Vector2Int.FloorToInt (pos), scene);
						if (mapUnit != null) {
							foreach (string id in entityIdsToSearch) {
								if (id == mapUnit.entityId)
									return (RegionMapManager.GetEntityObjectAtPoint (Vector2Int.FloorToInt (pos), scene));
							}
						}
					}
				}
			}
		}
		// nothing found
		return null;
	}
	public static GameObject FindClosestEntityInRange(Vector2 searchCenter, float searchRadius, string scene, List<string> entityIdsToSearch, out Vector2Int posInScene) 
	{
		Vector2 center = TilemapInterface.WorldPosToScenePos (searchCenter,scene);

		for (int y = 0; y <= searchRadius; y++) {
			for (int x = 0; x <= searchRadius; x++) {
				for (int signy = -1; signy <= 1; signy += 2) {
					for (int signx = -1; signx <= 1; signx += 2) {

						Vector2 relativePos = new Vector2 (x * signx, y * signy);
						if (relativePos.magnitude > searchRadius)
							continue;

						Vector2 pos = center + relativePos;
						pos = TilemapInterface.FloorToTilePos (pos);

						MapUnit mapUnit = RegionMapManager.GetMapObjectAtPoint (Vector2Int.FloorToInt (pos), scene);
						if (mapUnit != null) {
							foreach (string id in entityIdsToSearch) {
								if (id == mapUnit.entityId) {
									posInScene = Vector2Int.FloorToInt (pos);
									return (RegionMapManager.GetEntityObjectAtPoint (posInScene, scene));
								}
							}
						}
					}
				}
			}
		}
		// nothing found
		posInScene = new Vector2Int();
		return null;
	}
		
	public static GameObject FindClosestEntityWithComponent<Component>(Vector2 searchCenter, float searchRadius, string scene) 
	{
		// Find all the tiles in a circle around the start point
		Vector2 center = TilemapInterface.WorldPosToScenePos (searchCenter,scene);

		for (int y = 0; y <= searchRadius; y++) {
			for (int x = 0; x <= searchRadius; x++) {
				for (int signy = -1; signy <= 1; signy += 2) {
					for (int signx = -1; signx <= 1; signx += 2) {

						Vector2 relativePos = new Vector2 (x * signx, y * signy);
						if (relativePos.magnitude > searchRadius)
							continue;

						Vector2 pos = center + relativePos;
						pos = TilemapInterface.FloorToTilePos (pos);

						MapUnit mapUnit = RegionMapManager.GetMapObjectAtPoint (Vector2Int.FloorToInt (pos), scene);
						if (mapUnit != null) {
							GameObject entity = RegionMapManager.GetEntityObjectAtPoint (Vector2Int.FloorToInt (pos), scene);
							if (entity != null && entity.GetComponent<Component> () != null)
								return entity;
						}
					}
				}
			}
		}
		return null;
	}
	public static GameObject FindClosestEntityWithComponent<Component>(Vector2 searchCenter, float searchRadius, string scene, out Vector2Int posInScene) 
	{
		Vector2 center = TilemapInterface.WorldPosToScenePos (searchCenter,scene);
		posInScene = new Vector2Int ();
		float lowestDist = 0f;
		GameObject foundObject = null;

		// Find all the tiles in a circle around the start point
		for (int y = 0; y <= searchRadius; y++) {
			for (int x = 0; x <= searchRadius; x++) {
				for (int signy = -1; signy <= 1; signy += 2) {
					// ignore sign if y is 0
					if (signy == 1 && y == 0)
						continue;
					for (int signx = -1; signx <= 1; signx += 2) {
						
						if (signx == 1 && x == 0)
							continue;

						Vector2 relativePos = new Vector2 (x * signx, y * signy);
						float dist = relativePos.magnitude;

						if (dist > searchRadius)
							continue;
						if (dist >= lowestDist && foundObject != null)
							continue;
						
						Vector2 pos = center + relativePos;

						MapUnit mapUnit = RegionMapManager.GetMapObjectAtPoint (Vector2Int.FloorToInt (pos), scene);
						if (mapUnit != null) {
							GameObject entity = RegionMapManager.GetEntityObjectAtPoint (Vector2Int.FloorToInt (pos), scene);
							if (entity != null && entity.GetComponent<Component> () != null) {
								posInScene = Vector2Int.FloorToInt (pos);
								lowestDist = dist;
								foundObject = entity;
							}
						}
					}
				}
			}
		}
		return foundObject;
	}
}
