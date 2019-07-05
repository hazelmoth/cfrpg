using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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
		if (!mapDict.ContainsKey(scene))
		{
			return null;
		}
		if (!mapDict[scene].ContainsKey(point)) {
			return null;
		}
		return mapDict [scene] [point];
	}
	public static GroundMaterial GetGroundMaterialtAtPoint(Vector2Int point, string scene)
	{
		MapUnit mapUnit;
		if (!mapDict.ContainsKey(scene))
		{
			return null;
		}
		if (!mapDict[scene].ContainsKey(point))
		{
			return null;
		}
		mapUnit = mapDict[scene][point];
		return mapUnit.groundMaterial;
	}
	public static GameObject GetEntityObjectAtPoint (Vector2Int point, string scene) {
		if (!worldObjectDict [scene].ContainsKey (point))
			return null;
		return worldObjectDict [scene] [point];
	}
	public static string GetEntityIdForObject(GameObject entity, string scene) {
		Vector2 localPos = TilemapInterface.WorldPosToScenePos(entity.transform.position, scene);
		MapUnit mapObject = GetMapObjectAtPoint(new Vector2Int((int)localPos.x, (int)localPos.y), scene);
		if (mapObject == null)
			return null;
		return mapObject.entityId;
	}
	public static bool AttemptPlaceEntityAtPoint (EntityData entity, Vector2Int point, string scene) {
		// If the specified scene doesn't have a map yet, make one
		if (!worldObjectDict.ContainsKey(scene)) {
			Debug.LogWarning ("Attempted to place an entity in a scene that isn't registered in the world map");
			worldObjectDict.Add (scene, new Dictionary<Vector2Int, GameObject> ());
		}
		// Go through all the tiles the entity would cover and make sure they're okay to be covered
		foreach (Vector2Int entitySection in entity.baseShape) {
			// Return false if there is no map unit defined at this point
			if (!mapDict.ContainsKey(scene) || !mapDict[scene].ContainsKey(point + entitySection)) {
				return false;
			}
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
				GameObject.Destroy (worldObjectDict [scene] [objectRootPos + entitySection]);
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
		TilemapInterface.RefreshWorldTiles();
		TilemapLibrary.BuildLibrary ();
	}
	public static void BuildMapForScene (string scene, GameObject sceneRootObject)
	{
		if (mapDict == null)
		{
			Debug.LogError("Can't build map for scene; world map hasn't been initialized!");
			return;
		}
		Dictionary<Vector2Int, MapUnit> map = new Dictionary<Vector2Int, MapUnit>();
		Dictionary<Vector2Int, GameObject> objectMap = new Dictionary<Vector2Int, GameObject>();
		Tilemap tilemap = sceneRootObject.GetComponentInChildren<Tilemap>();
		if (tilemap == null)
		{
			mapDict.Add(scene, map);
			Debug.LogWarning("tried to build maps for a scene containing no tilemap");
			return;
		}
		foreach (Vector3 pos in tilemap.cellBounds.allPositionsWithin)
		{
			// Note that this assumes the names of tile prefabs are the same as the tile IDs!
			GroundMaterial material = GroundMaterialLibrary.GetGroundMaterialById(tilemap.GetTile(pos.ToVector3Int())?.name);
			if (material != null)
			{
				MapUnit unit = new MapUnit();
				unit.groundMaterial = material;
				map.Add(pos.ToVector2Int(), unit);
			}
		}
		mapDict.Add(scene, map);
		worldObjectDict.Add(scene, objectMap);
	}

	// Check that placement is legal before using this
	static void PlaceEntityAtPoint (EntityData entity, Vector2Int point, string scene) {
		// Make the actual object
		GameObject entityObject = GameObject.Instantiate (entity.entityPrefab, SceneObjectManager.GetSceneObjectFromId(scene).transform);

		if (entity.pivotAtCenterOfTile)
			entityObject.transform.localPosition = TilemapInterface.GetCenterPositionOfTile (new Vector2 (point.x, point.y));
		else
			entityObject.transform.localPosition = new Vector2 (point.x, point.y);
		
		// Add the entity data to the maps
		foreach (Vector2Int entitySection in entity.baseShape) {
			// Get rid of anything already there
			RemoveEntityAtPoint (point + entitySection, scene);
			if (!worldObjectDict[scene].ContainsKey(point + entitySection)) {
				worldObjectDict[scene].Add(point + entitySection, null);
			}
			worldObjectDict [scene] [point + entitySection] = entityObject;
			mapDict [scene] [point + entitySection].entityId = entity.entityId;
			mapDict [scene] [point + entitySection].relativePosToEntityOrigin = entitySection;
		}
	}
	static void InitializeObjectDict () {
		worldObjectDict = new Dictionary<string, Dictionary<Vector2Int, GameObject>> ();
		foreach (string scene in mapDict.Keys) {
			worldObjectDict.Add (scene, new Dictionary<Vector2Int, GameObject> ());
		}
	}
}
