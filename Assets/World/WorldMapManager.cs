using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Stores the locations of entities for loaded scenes
public class WorldMapManager : MonoBehaviour
{
	// Maps scenes to dictionaries
	// Dictionaries map locations to entities
	static Dictionary<string, Dictionary<Vector2Int, MapUnit>> mapDict;
	// Maps locations to the actual entity objects on them
	static Dictionary<string, Dictionary<Vector2Int, GameObject>> worldObjectDict;

	public static void LoadMap (WorldMap map) {
		mapDict = map.mapDict;
		InitializeObjectDict ();
		LoadMapsIntoScenes ();
	}
	public static MapUnit GetMapObjectAtPoint (Vector2Int point, string scene) {
		if (!mapDict[scene].ContainsKey(point)) {
			return null;
		}
		return mapDict [scene] [point];
	}
	public static bool AttemptPlaceEntityAtPoint (EntityData entity, Vector2Int point, string scene) {
		if (!worldObjectDict.ContainsKey(scene)) {
			worldObjectDict.Add (scene, new Dictionary<Vector2Int, GameObject> ());
		}
		// Go through all the tiles the entity would cover and make sure they're okay to be covered
		foreach (Vector2Int entitySection in entity.baseShape) {
			if (!worldObjectDict[scene].ContainsKey(point + entitySection)) {
				worldObjectDict [scene].Add (point + entitySection, null);
			}
			MapUnit mapObject = mapDict [scene] [point + entitySection];
			if ((mapObject.entityId != null && 
                EntityLibrary.GetEntityFromID(mapObject.entityId).canBeBuiltOver == false) || 
                mapObject.groundMaterial.isWater == true) 
            {
				return false;
			}
		}
		PlaceEntityAtPoint (entity, point, scene);
		return true;
	}
	public static void RemoveEntityAtPoint (Vector2Int point, string scene) {
		MapUnit mapUnit = GetMapObjectAtPoint (point, scene);
		if (mapUnit.entityId == null)
			return;
		Vector2Int objectRootPos = point - mapUnit.relativePosToEntityOrigin;
		MapUnit rootMapUnit = GetMapObjectAtPoint (objectRootPos, scene);
		foreach (Vector2Int entitySection in EntityLibrary.GetEntityFromID(rootMapUnit.entityId).baseShape) {
			if (mapDict[scene].ContainsKey(objectRootPos + entitySection)) {
				mapDict [scene] [objectRootPos + entitySection].entityId = null;
			}
			if (worldObjectDict [scene].ContainsKey (objectRootPos + entitySection)) {
				worldObjectDict[scene][objectRootPos + entitySection] = null;
			}
		}
	}
	public static void LoadMapsIntoScenes () {
		TilemapInterface.ClearWorldTilemap ();
		foreach (string scene in mapDict.Keys) {
			foreach (Vector2Int point in mapDict[scene].Keys) {
				TilemapInterface.ChangeTile (point.x, point.y, mapDict [scene] [point].groundMaterial.tileAsset, scene);
				if (mapDict[scene][point].entityId != null && mapDict[scene][point].relativePosToEntityOrigin == new Vector2Int (0,0)) {
					PlaceEntityAtPoint (EntityLibrary.GetEntityFromID (mapDict [scene] [point].entityId), point, scene);
				}
			}
		}
		TilemapLibrary.BuildLibrary ();
	}

	// Check that placement is legal before using this
	static void PlaceEntityAtPoint (EntityData entity, Vector2Int point, string scene) {
		// Make the actual object
		GameObject entityObject = GameObject.Instantiate (entity.entityPrefab, SceneManager.GetSceneByName(scene).GetRootGameObjects()[0].transform);

		if (entity.pivotAtCenterOfTile)
			entityObject.transform.localPosition = TilemapInterface.GetCenterPositionOfTile (new Vector2 (point.x, point.y));
		else
			entityObject.transform.localPosition = new Vector2 (point.x, point.y);
		
		// Add the entity data to the maps
		foreach (Vector2Int entitySection in entity.baseShape) {
			if (!worldObjectDict[scene].ContainsKey(point + entitySection)) {
				worldObjectDict[scene].Add(point + entitySection, null);
			}
			worldObjectDict [scene] [point + entitySection] = entityObject;
			mapDict [scene] [point + entitySection].entityId = entity.entityId;
			mapDict [scene] [point + entitySection].relativePosToEntityOrigin = point;
		}
	}
	static void InitializeObjectDict () {
		worldObjectDict = new Dictionary<string, Dictionary<Vector2Int, GameObject>> ();
		foreach (string scene in mapDict.Keys) {
			worldObjectDict.Add (scene, new Dictionary<Vector2Int, GameObject> ());
		}
	}
}
