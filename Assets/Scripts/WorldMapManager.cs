using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Stores all the data for tile properties and entity positions in loaded scene objects
public class WorldMapManager : MonoBehaviour
{
	// Maps scenes to dictionaries
	// Dictionaries map locations to entities
	private static Dictionary<string, Dictionary<Vector2Int, MapUnit>> mapDict;
	// Maps locations to the actual entity objects on them
	private static Dictionary<string, Dictionary<Vector2Int, GameObject>> worldObjectDict;

	public static void LoadMap (RegionMap map) {
		if (map == null)
		{
			Debug.LogError("attempted to load a nonexistent world map!");
			return;
		}
		mapDict = new Dictionary<string, Dictionary<Vector2Int, MapUnit>>();
		worldObjectDict = new Dictionary<string, Dictionary<Vector2Int, GameObject>>();
		// Make sure we have scene objects for all the scenes in the new map
		SceneObjectManager.DestroyAllScenes();
		foreach (string scene in map.mapDict.Keys)
		{
			if (!SceneObjectManager.SceneExists(scene))
			{
				SceneObjectManager.CreateNewScene(scene);
			}
		}
		TilemapInterface.ClearWorldTilemap();
		foreach (string scene in map.mapDict.Keys)
		{
			foreach (Vector2Int point in map.mapDict[scene].Keys)
			{
				TilemapInterface.ChangeTile(point.x, point.y, map.mapDict[scene][point].groundMaterial.tileAsset, scene, TilemapLayer.Ground);
				if (map.mapDict[scene][point].groundCover != null)
					TilemapInterface.ChangeTile(point.x, point.y, map.mapDict[scene][point].groundCover.tileAsset, scene, TilemapLayer.GroundCover);

				// If the saved map has an entity id for this tile, place that entity in the scene
				if (map.mapDict[scene][point].entityId != null && map.mapDict[scene][point].relativePosToEntityOrigin == Vector2Int.zero)
				{
					EntityData entity = ContentLibrary.Instance.Entities.Get(map.mapDict[scene][point].entityId);
					if (entity == null)
					{
						Debug.LogWarning("Couldn't find entity for id \"" + map.mapDict[scene][point].entityId + "\"");
					} else { 
						PlaceEntityAtPoint(entity, point, scene, entity.baseShape);
					}
				}
			}
		}
		mapDict = map.mapDict;
		TilemapInterface.RefreshWorldTiles();
		TilemapLibrary.BuildLibrary();
	}

	public static RegionMap GetWorldMap ()
	{
		return new RegionMap(mapDict);
	}

	public static Dictionary<string, Dictionary<Vector2Int, GameObject>> GetObjectMaps ()
	{
		return worldObjectDict;
	}

	// Returns the MapUnit at the given scene coordinates in the given scene. Returns null if none exists.
	public static MapUnit GetMapObjectAtPoint (Vector2Int point, string scene) 
	{
		if (!mapDict.ContainsKey(scene))
		{
			return null;
		}
		if (!mapDict[scene].ContainsKey(point)) {
			return null;
		}
		return mapDict [scene] [point];
	}

	// Takes a tile position in scene coordinates.
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

	// Takes a tile position in scene coordinates.
	public static GroundMaterial GetGroundCoverAtPoint(Vector2Int point, string scene)
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
		return mapUnit.groundCover;
	}

	// Takes a tile position in scene coordinates.
	public static void ChangeGroundMaterial(Vector2Int tile, string scene, TilemapLayer layer, GroundMaterial newMaterial)
	{
		MapUnit mapUnit;
		if (!mapDict.ContainsKey(scene))
		{
			Debug.LogError("Given scene \"" + scene + "\" not found.");
		}
		if (!mapDict[scene].ContainsKey(tile))
		{
			mapDict[scene][tile] = new MapUnit();
		}
		mapUnit = mapDict[scene][tile];
		if (layer == TilemapLayer.Ground)
		{
			mapUnit.groundMaterial = newMaterial;
		} 
		else
		{
			mapUnit.groundCover = newMaterial;
		}
		TileBase newTile = newMaterial == null ? null : newMaterial.tileAsset;
		TilemapInterface.ChangeTile(tile.x, tile.y, newTile, scene, layer);
	}

	// Takes a tile position in scene coordinates.
	public static GameObject GetEntityObjectAtPoint (Vector2Int point, string scene) {
		if (!worldObjectDict [scene].ContainsKey (point))
			return null;
		return worldObjectDict [scene] [point];
	}

	// Takes a tile position in scene coordinates.
	public static string GetEntityIdAtPoint(Vector2Int point, string scene)
	{
		if (!mapDict[scene].ContainsKey(point))
			return null;
		return mapDict[scene][point].entityId;
	}

	// Takes a tile position in scene coordinates.
	public static string GetEntityIdForObject(GameObject entity, string scene) {
		Vector2 localPos = TilemapInterface.WorldPosToScenePos(entity.transform.position, scene);
		MapUnit mapObject = GetMapObjectAtPoint(new Vector2Int((int)localPos.x, (int)localPos.y), scene);
		if (mapObject == null)
			return null;
		return mapObject.entityId;
	}

	// Takes a tile position in scene coordinates.
	public static bool AttemptPlaceEntityAtPoint(EntityData entity, Vector2Int point, string scene)
	{
		return AttemptPlaceEntityAtPoint(entity, point, scene, null, out _);
	}

	// Attempts to place the given entity at the given tile in the given scene. Mandates that all the tiles in
	// forcedBaseShape be clear, and sets their entity tag, if it is not null; otherwise, uses the base shape 
	// of the given entity. Outputs the entity game object that was placed. Returns true only if placement was successful.
	public static bool AttemptPlaceEntityAtPoint (EntityData entity, Vector2Int point, string scene, List<Vector2Int> forcedBaseShape, out EntityObject placed)
	{
		placed = null;
		if (entity == null)
		{
			Debug.LogException(new NullReferenceException("Tried to place null entity!"));
		}
		if (forcedBaseShape == null)
		{
			forcedBaseShape = entity.baseShape;
		}
		// If the specified scene doesn't have an object map yet, make one
		if (!worldObjectDict.ContainsKey(scene)) {
			Debug.LogWarning ("Attempted to place an entity in a scene that isn't registered in the world map");
			worldObjectDict.Add (scene, new Dictionary<Vector2Int, GameObject> ());
		}
		// Go through all the tiles the entity would cover and make sure they're okay to be covered
		foreach (Vector2Int entitySection in forcedBaseShape) {
			// Return false if there is no map unit defined at this point
			if (!mapDict.ContainsKey(scene) || !mapDict[scene].ContainsKey(point + entitySection)) {
				return false;
			}
			if (!worldObjectDict[scene].ContainsKey(point + entitySection)) {
				worldObjectDict [scene].Add (point + entitySection, null);
			}
			MapUnit mapObject = mapDict [scene] [point + entitySection];
			if ((mapObject.entityId != null && 
                !ContentLibrary.Instance.Entities.Get(mapObject.entityId).canBeBuiltOver) || 
                mapObject.groundMaterial.isWater == true) 
            {
				return false;
			}
		}
		placed = PlaceEntityAtPoint (entity, point, scene, forcedBaseShape);
		return true;
	}

	// Takes a tile position in scene coordinates.
	public static void RemoveEntityAtPoint (Vector2Int point, string scene)
	{
		MapUnit mapUnit = GetMapObjectAtPoint (point, scene);

		if (mapUnit == null || mapUnit.entityId == null)
			return;

		Vector2Int objectRootPos = point - mapUnit.relativePosToEntityOrigin;
		MapUnit rootMapUnit = GetMapObjectAtPoint (objectRootPos, scene);
		if (rootMapUnit.entityId == null)
		{
			Debug.LogWarning("Found entity \"" + mapUnit.entityId + "\" at given point " + point + " but no entity at its root " + objectRootPos + "!");
		}
		foreach (Vector2Int entitySection in ContentLibrary.Instance.Entities.Get(rootMapUnit.entityId).baseShape) {
			if (mapDict[scene].ContainsKey(objectRootPos + entitySection)) {
				mapDict [scene] [objectRootPos + entitySection].entityId = null;
			}
			if (worldObjectDict [scene].ContainsKey (objectRootPos + entitySection)) {
				GameObject.Destroy (worldObjectDict [scene] [objectRootPos + entitySection]);
				worldObjectDict[scene][objectRootPos + entitySection] = null;
			}
		}
	}

	// Sets the entity at the given tile to null, regardless of what GameObject is actually there. Use sparingly.
	public static void ForceClearEntityDataAtPoint (Vector2Int point, string scene)
	{
		MapUnit mapUnit = GetMapObjectAtPoint(point, scene);

		if (mapUnit == null) return;

		mapUnit.entityId = null;
	}

	public static Vector2Int FindWalkableEdgeTile (Direction mapSide)
	{
		int max = (mapSide == Direction.Left || mapSide == Direction.Right ? GameDataMaster.WorldSize.y : GameDataMaster.WorldSize.x);
		int value = UnityEngine.Random.Range(0, max);
		Vector2Int pos = new Vector2Int();

		if (mapSide == Direction.Left || mapSide == Direction.Right)
		{
			pos.x = (int)Mathf.Clamp01(mapSide.ToVector2().x) * (GameDataMaster.WorldSize.x - 1);
			pos.y = value;
		} 
		else
		{
			pos.x = value;
			pos.y = (int)Mathf.Clamp01(mapSide.ToVector2().y) * (GameDataMaster.WorldSize.y - 1);
		}
		if (TileIsWalkable(SceneObjectManager.WorldSceneId, pos))
		{
			return pos;
		}
		else
		{
			// yeah this might loop forever
			return FindWalkableEdgeTile(mapSide);
		}
	}

	// Constructs a map for a scene object that has been created from a prefab.
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
		Vector2Int tilemapOffset = tilemap.transform.position.ToVector2Int();

		// TODO allow prefabs to have Ground Cover tilemaps as well

		if (tilemap == null)
		{
			if (mapDict.ContainsKey(scene))
				mapDict[scene] = map;
			else
				mapDict.Add(scene, map);
			Debug.LogWarning("tried to build maps for a scene containing no tilemap");
			return;
		}

		// Find the ground material of every tile in the scene.
		foreach (Vector3 pos in tilemap.cellBounds.allPositionsWithin)
		{
			// Note that this assumes the names of tile prefabs are the same as the tile IDs!
			GroundMaterial material = ContentLibrary.Instance.GroundMaterials.Get(tilemap.GetTile(pos.ToVector3Int())?.name);
			if (material != null)
			{
				MapUnit unit = new MapUnit();
				unit.groundMaterial = material;
				map.Add(pos.ToVector2Int(), unit);
			}
		}

		if (mapDict.ContainsKey(scene))
			mapDict[scene] = map;
		else
			mapDict.Add(scene, map);
		if (worldObjectDict.ContainsKey(scene))
			worldObjectDict[scene] = objectMap;
		else
			worldObjectDict.Add(scene, objectMap);


		// Find all existing entities in the scene, and destroy and then properly spawn them.
		foreach (Transform transform in sceneRootObject.transform)
		{
			if (transform.TryGetComponent(out InteriorPrefabEntity prefabEntity))
			{
				string id = prefabEntity.entityID;
				Vector2Int pos = prefabEntity.transform.position.ToVector2Int() - tilemapOffset;

				if (map.ContainsKey(pos))
				{
					map[pos].entityId = id;
				}

				// Now destroy the game object and respawn it, to make sure everything is in order
				EntityData entity = ContentLibrary.Instance.Entities.Get(id);
				if (entity == null)
				{
					Debug.LogError("Prefab entity has invalid ID.", prefabEntity);
					continue;
				}
				GameObject.Destroy(prefabEntity.gameObject);
				PlaceEntityAtPoint(entity, pos, scene, entity.baseShape);
			}
		}
	}

	// Check that placement is legal before using this
	private static EntityObject PlaceEntityAtPoint (EntityData entity, Vector2Int point, string scene, List<Vector2Int> baseShape) {
		if (entity == null)
		{
			throw new ArgumentNullException(nameof(entity));
		}

		// Make the actual object
		GameObject entityObject = GameObject.Instantiate (entity.entityPrefab, SceneObjectManager.GetSceneObjectFromId(scene).transform);

		if (entity.pivotAtCenterOfTile)
			entityObject.transform.localPosition = TilemapInterface.GetCenterPositionOfTile (new Vector2 (point.x, point.y));
		else
			entityObject.transform.localPosition = new Vector2 (point.x, point.y);

		// Give the entity an EntityTag component so we can know what it is from the object
		EntityObject entityTag = entityObject.GetComponent<EntityObject>();
		if (entityTag == null)
		{
			entityTag = entityObject.AddComponent<EntityObject>();
		}
		entityTag.EntityId = entity.entityId;

		// Add the entity data to the maps
		foreach (Vector2Int entitySection in baseShape) {
			// Get rid of anything already there
			RemoveEntityAtPoint(point + entitySection, scene);

			// Add keys for these points if none exist
			if (!mapDict[scene].ContainsKey(point + entitySection))
			{
				mapDict[scene].Add(point + entitySection, new MapUnit());
			}
			if (!worldObjectDict[scene].ContainsKey(point + entitySection)) {
				worldObjectDict[scene].Add(point + entitySection, null);
			}
			
			worldObjectDict[scene] [point + entitySection] = entityObject;
			mapDict [scene] [point + entitySection].entityId = entity.entityId;
			mapDict [scene] [point + entitySection].relativePosToEntityOrigin = entitySection;
		}
		return entityTag;
	}

	// Takes a tile position in scene coordinates.
	private static bool TileIsWalkable (string scene, Vector2Int pos)
	{
		MapUnit tile = GetMapObjectAtPoint(pos, scene);
		if (tile == null)
		{
			Debug.LogError("No tile found at given location (" + pos.x + ", " + pos.y + ")");
		}

		if (tile.groundMaterial != null && !tile.groundMaterial.isWater)
		{
			if (tile.entityId == null || ContentLibrary.Instance.Entities.Get(tile.entityId).canBeWalkedThrough)
			{
				return true;
			}
		}
		return false;
	}
}
