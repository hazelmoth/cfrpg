using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ContentLibraries;
using ContinentMaps;
using UnityEngine;
using UnityEngine.Tilemaps;

// Stores the currently loaded Region Map, and provides methods for accessing it
// and for loading region maps.
public class RegionMapManager : MonoBehaviour
{
	private static RegionMap currentRegion; // The currently loaded region.

	// Stores references to the actual entity gameObjects in scenes
	private static Dictionary<string, Dictionary<Vector2Int, GameObject>> entityObjectMap;

	public static Action regionLoaded;

	/**
	 * Deletes all existing scene objects, including tilemaps, entities, and actors from the scene, and spawns new ones
	 * as described in the specified map. Copies the given map rather than using it directly.
	 */
	public static void LoadMap (RegionMap map) {
		if (map == null)
		{
			Debug.LogError("Map argument to LoadMap was null!");
			return;
		}

		// Destroy all scenes, and set the current map to a blank one. (We'll rebuild it based on the given map.)
		SceneObjectManager.DestroyAllScenes();
		currentRegion = new RegionMap();
		entityObjectMap = new Dictionary<string, Dictionary<Vector2Int, GameObject>>();

		// Iterate through every scene in the new map
		foreach (string scene in map.mapDict.Keys)
		{
			SceneObjectManager.CreateBlankScene(scene); // Create the scene object.
			entityObjectMap.Add(scene, new Dictionary<Vector2Int, GameObject>()); // Create scene entry in entity object dictionary.
			currentRegion.mapDict.Add(scene,new Dictionary<Vector2Int, MapUnit>());

			// Go through every tile in the scene.
			foreach (Vector2Int point in map.mapDict[scene].Keys)
			{
				// Create a MapUnit at this point, and copy the ground data
				currentRegion.mapDict[scene][point] = new MapUnit();
				currentRegion.mapDict[scene][point].groundMaterial = map.mapDict[scene][point].groundMaterial;
				currentRegion.mapDict[scene][point].groundCover = map.mapDict[scene][point].groundCover;
				currentRegion.mapDict[scene][point].cliffMaterial = map.mapDict[scene][point].cliffMaterial;

				// Place the actual ground tiles
				if (map.mapDict[scene][point].groundMaterial != null)
					TilemapInterface.ChangeTile(point.x, point.y, map.mapDict[scene][point].groundMaterial.tileAsset, scene, TilemapLayer.Ground);
				if (map.mapDict[scene][point].groundCover != null)
					TilemapInterface.ChangeTile(point.x, point.y, map.mapDict[scene][point].groundCover.tileAsset, scene, TilemapLayer.GroundCover);
				if (map.mapDict[scene][point].cliffMaterial != null)
					TilemapInterface.ChangeTile(point.x, point.y, map.mapDict[scene][point].cliffMaterial.tileAsset, scene, TilemapLayer.Cliff);
			}

			// Go through the tiles again and place entities.
			foreach (Vector2Int point in map.mapDict[scene].Keys)
			{
				// If the saved map has an entity id originating on this tile, place that entity in the scene.
				if (map.mapDict[scene][point].entityId != null && map.mapDict[scene][point].relativePosToEntityOrigin == Vector2Int.zero)
				{
					EntityData entity = ContentLibrary.Instance.Entities.Get(map.mapDict[scene][point].entityId);
					if (entity == null)
					{
						Debug.LogWarning("Couldn't find entity for id \"" + map.mapDict[scene][point].entityId + "\"");
					} else {
						EntityObject placedEntity = PlaceEntityAtPoint(entity, point, scene, entity.BaseShape);

						// Set component data, if there is any.
						if (map.mapDict[scene][point].savedComponents != null)
							placedEntity.SetState(map.mapDict[scene][point].savedComponents);
					}
				}
			}
		}
		TilemapInterface.RefreshWorldTiles();
		TilemapLibrary.BuildLibrary();

		// Now spawn any scene portals in the map
		foreach (SerializableScenePortal portalInfo in map.scenePortals)
		{
			SaveLoader.SpawnScenePortal(portalInfo);
		}
		ScenePortalLibrary.BuildLibrary();

		// Spawn in any saved actors
		foreach (string actorId in map.actors.Keys)
		{
			ActorSpawner.Spawn(
				actorId,
				map.actors[actorId].location.Vector2,
				map.actors[actorId].location.scene,
				map.actors[actorId].direction);
		}
		// Place any dropped items
		foreach (SavedDroppedItem item in map.droppedItems)
		{
			DroppedItemSpawner.SpawnItem(item.item, item.location.ToVector2(), item.scene);
		}
		regionLoaded?.Invoke();
	}

	public static RegionMap ExportRegionMap (bool ignorePlayer = false)
	{
		// Update with entity save data
		foreach (string scene in currentRegion.mapDict.Keys)
		{
			foreach (Vector2Int point in currentRegion.mapDict[scene].Keys)
			{
				if (!currentRegion.mapDict[scene].ContainsKey(point)) continue;
				if (!entityObjectMap[scene].ContainsKey(point)) continue;

				currentRegion.mapDict[scene][point].savedComponents =
					entityObjectMap[scene][point] != null ?
						entityObjectMap[scene][point].GetComponent<EntityObject>().GetSaveData() :
						null;
			}
		}

		// Update with actor positions
		currentRegion.actors = new Dictionary<string, RegionMap.ActorPosition>();
		foreach (string id in ActorRegistry.GetAllIds())
		{
			if (ignorePlayer && PlayerController.PlayerActorId == id) continue;

			ActorRegistry.ActorInfo info = ActorRegistry.Get(id);
			if (info.actorObject == null) continue; // Only save spawned actors

			RegionMap.ActorPosition position =
				new RegionMap.ActorPosition(info.actorObject.Location, info.actorObject.Direction);

			currentRegion.actors.Add(id, position);
		}

		// Copy scene portal information from scene portal library.
		// TODO I'd prefer that definitive portal data lives in RegionMap;
		//      there's no need for a separate ScenePortalLibrary class
		currentRegion.scenePortals = ScenePortalLibrary.GetAllPortalDatas()
			// Portals owned by entities already have their data stored with the entities.
			.Where(portalData => !portalData.ownedByEntity)
			.ToList();

		// This is messy for the same reasons as above.
		currentRegion.droppedItems = FindObjectOfType<DroppedItemRegistry>().GetItems().Select(item =>
			new SavedDroppedItem(item.transform.localPosition.ToVector2().ToSerializable(),
				SceneObjectManager.GetSceneIdForObject(item.gameObject), item.Item)).ToList();

		return currentRegion;
	}

	public static Dictionary<string, Dictionary<Vector2Int, GameObject>> GetObjectMaps ()
	{
		return entityObjectMap;
	}

	// Returns direct references to all map units in the given scene.
	public static IDictionary<Vector2Int, MapUnit> GetMapUnits (string scene)
    {
	    Debug.Assert(entityObjectMap.ContainsKey(scene), $"Scene {scene} doesn't exist in map");
	    return currentRegion.mapDict[scene];
    }

	/// Returns a reference to the MapUnit at the given scene coordinates in the given
	/// scene. Returns null if none exists.
	public static MapUnit GetMapUnitAtPoint (Vector2Int point, string scene)
	{
		if (!currentRegion.mapDict.ContainsKey(scene))
		{
			return null;
		}
		if (!currentRegion.mapDict[scene].ContainsKey(point)) {
			return null;
		}
		return currentRegion.mapDict [scene] [point];
	}

	/// Takes a tile position in scene coordinates.
	public static GroundMaterial GetGroundMaterialAtPoint(Vector2Int point, string scene)
	{
		MapUnit mapUnit;
		if (!currentRegion.mapDict.ContainsKey(scene))
		{
			return null;
		}
		if (!currentRegion.mapDict[scene].ContainsKey(point))
		{
			return null;
		}
		mapUnit = currentRegion.mapDict[scene][point];
		return mapUnit.groundMaterial;
	}

	/// Takes a tile position in scene coordinates.
	public static GroundMaterial GetGroundCoverAtPoint(Vector2Int point, string scene)
	{
		MapUnit mapUnit;
		if (!currentRegion.mapDict.ContainsKey(scene))
		{
			return null;
		}
		if (!currentRegion.mapDict[scene].ContainsKey(point))
		{
			return null;
		}
		mapUnit = currentRegion.mapDict[scene][point];
		return mapUnit.groundCover;
	}

	/// Takes a tile position in scene coordinates.
	public static void ChangeGroundMaterial(Vector2Int tile, string scene, TilemapLayer layer, GroundMaterial newMaterial)
	{
		MapUnit mapUnit;
		if (!currentRegion.mapDict.ContainsKey(scene))
		{
			Debug.LogError("Given scene \"" + scene + "\" not found.");
		}
		if (!currentRegion.mapDict[scene].ContainsKey(tile))
		{
			currentRegion.mapDict[scene][tile] = new MapUnit();
		}
		mapUnit = currentRegion.mapDict[scene][tile];
		if (layer == TilemapLayer.Ground)
		{
			mapUnit.groundMaterial = newMaterial;
		}
		else
		{
			mapUnit.groundCover = newMaterial;
		}
		TileBase newTile = newMaterial?.tileAsset;
		TilemapInterface.ChangeTile(tile.x, tile.y, newTile, scene, layer);
	}

	/// Takes a tile position in scene coordinates.
	public static GameObject GetEntityObjectAtPoint (Vector2Int point, string scene) {
		if (!entityObjectMap [scene].ContainsKey (point))
			return null;
		return entityObjectMap [scene] [point];
	}

	/// Takes a tile position in scene coordinates.
	public static string GetEntityIdAtPoint(Vector2Int point, string scene)
	{
		if (!currentRegion.mapDict[scene].ContainsKey(point)) return null;
		return currentRegion.mapDict[scene][point].entityId;
	}

	/// Takes a tile position in scene coordinates.
	public static bool AttemptPlaceEntityAtPoint(EntityData entity, Vector2Int point, string scene)
	{
		return AttemptPlaceEntityAtPoint(entity, point, scene, null, out _);
	}

	/// Attempts to place the given entity at the given tile in the given scene. Mandates that all the tiles in
	/// point + forcedBaseShape be clear, and sets their entity tag, if it is not null; otherwise, uses the base shape
	/// of the given entity. Outputs the entity game object that was placed. Returns true only if placement was successful.
	public static bool AttemptPlaceEntityAtPoint (EntityData entity, Vector2Int point, string scene, IEnumerable<Vector2Int> forcedBaseShape, out EntityObject placed)
	{
		placed = null;
		if (entity == null)
		{
			Debug.LogException(new NullReferenceException("Tried to place null entity!"));
			return false;
		}
		forcedBaseShape ??= entity.BaseShape;

		// If the specified scene doesn't have an object map yet, make one
		if (!entityObjectMap.ContainsKey(scene)) {
			Debug.LogWarning ("Attempted to place an entity in a scene that isn't registered in the world map");
			entityObjectMap.Add (scene, new Dictionary<Vector2Int, GameObject> ());
		}
		// Go through all the tiles the entity would cover and make sure they're okay to be covered
		foreach (Vector2Int entitySection in forcedBaseShape) {
			// Return false if there is no map unit defined at this point
			if (!currentRegion.mapDict.ContainsKey(scene) || !currentRegion.mapDict[scene].ContainsKey(point + entitySection)) {
				return false;
			}
			if (!entityObjectMap[scene].ContainsKey(point + entitySection)) {
				entityObjectMap [scene].Add (point + entitySection, null);
			}
			MapUnit mapObject = currentRegion.mapDict [scene] [point + entitySection];
			if ((mapObject.entityId != null &&
                !ContentLibrary.Instance.Entities.Get(mapObject.entityId).CanBeBuiltOver) ||
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
		Debug.Assert(entityObjectMap.ContainsKey(scene), "Given scene isn't in the entity object dictionary!");
		MapUnit mapUnit = GetMapUnitAtPoint (point, scene);

		if (mapUnit == null || mapUnit.entityId == null) return;

		Vector2Int objectRootPos = point - mapUnit.relativePosToEntityOrigin;
		MapUnit rootMapUnit = GetMapUnitAtPoint (objectRootPos, scene);
		if (rootMapUnit.entityId == null)
		{
			Debug.LogWarning("Found entity \"" + mapUnit.entityId + "\" at given point " + point + " but no entity at its root " + objectRootPos + "!");
		}
		foreach (Vector2Int entitySection in ContentLibrary.Instance.Entities.Get(rootMapUnit.entityId).BaseShape) {
			if (currentRegion.mapDict[scene].ContainsKey(objectRootPos + entitySection)) {
				currentRegion.mapDict [scene] [objectRootPos + entitySection].entityId = null;
			}
			if (entityObjectMap [scene].ContainsKey (objectRootPos + entitySection)) {
				GameObject.Destroy (entityObjectMap [scene] [objectRootPos + entitySection]);
				entityObjectMap[scene][objectRootPos + entitySection] = null;
			}
		}
	}

	// Sets the entity at the given tile to null, regardless of what GameObject is actually there. Use sparingly.
	public static void ForceClearEntityDataAtPoint (Vector2Int point, string scene)
	{
		MapUnit mapUnit = GetMapUnitAtPoint(point, scene);

		if (mapUnit == null) return;

		mapUnit.entityId = null;
	}

	/// Finds a tile somewhere at the edge of the map which is walkable.
	/// Assumes the map is rectangular; generally only works for generated regions.
	[Obsolete("Use FindRegionEntranceTile instead")]
	public static Vector2Int FindWalkableEdgeTile (Direction mapSide)
	{
		int max = (mapSide == Direction.Left || mapSide == Direction.Right ? SaveInfo.RegionSize.y : SaveInfo.RegionSize.x);
		int value = UnityEngine.Random.Range(0, max);
		Vector2Int pos = new Vector2Int();

		if (mapSide == Direction.Left || mapSide == Direction.Right)
		{
			pos.x = (int)Mathf.Clamp01(mapSide.ToVector2().x) * (SaveInfo.RegionSize.x - 1);
			pos.y = value;
		}
		else
		{
			pos.x = value;
			pos.y = (int)Mathf.Clamp01(mapSide.ToVector2().y) * (SaveInfo.RegionSize.y - 1);
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

	/// Finds a walkable tile next to an entrance to the current region.
	/// Mandates that the portal have the given tag, if any.
	/// Logs an error and tries its best if a valid entrance is not found.
	public static Vector2Int FindRegionEntranceTile(out Direction entranceDir, string requiredPortalTag = null)
	{
		// Find all portals in the target scene with a matching tag.
		List<RegionPortal> portals = FindObjectsOfType<RegionPortal>()
			.Where(p => !p.ExitOnly)
			.Where(p => requiredPortalTag == null || p.PortalTag == requiredPortalTag)
			.ToList();

		// Choose a random portal from the ones that match.
		RegionPortal portal = portals.Any() ? portals.PickRandom() : null;

		Vector2Int arrivalTile;
		if (portal != null)
		{
			arrivalTile = portal.GetComponent<EntityObject>().Location.Vector2Int
				+ portal.ExitDirection.Invert().ToVector2().ToVector2Int();
			entranceDir = portal.ExitDirection.Invert();
		}
		else
		{
			// No portal found; choose a random spawn point.
			arrivalTile =
				ActorSpawnpointFinder.FindSpawnPoint(ExportRegionMap(), ContinentManager.CurrentRegionId)
					.ToVector2Int();
			entranceDir = Direction.Down;
		}

		return arrivalTile;
	}

	/// Constructs a RegionMap from a region prefab.
	public static RegionMap BuildRegionFromPrefab(GameObject scenePrefab)
	{
		Dictionary<Vector2Int, MapUnit> map = new Dictionary<Vector2Int, MapUnit>();

		Tilemap groundTilemap = scenePrefab.GetComponentsInChildren<Tilemap>()
			.First(tilemapObj => tilemapObj.CompareTag("GroundTilemap"));
		Tilemap groundCoverTilemap = scenePrefab.GetComponentsInChildren<Tilemap>()
			.First(tilemapObj => tilemapObj.CompareTag("GroundCoverTilemap"));
		Tilemap cliffTilemap = scenePrefab.GetComponentsInChildren<Tilemap>()
			.First(tilemapObj => tilemapObj.CompareTag("CliffsTilemap"));

		Vector2Int tilemapOffset = groundTilemap.transform.position.ToVector2Int();

		ImmutableList.Create(groundTilemap, groundCoverTilemap, cliffTilemap)
			.ForEach(
				tilemap =>
				{
					// Find the ground material of every tile in the scene.
					foreach (Vector3 pos in tilemap.cellBounds.allPositionsWithin)
					{
						MapUnit mapUnit = map.ContainsKey(TilemapInterface.FloorToTilePos(pos))
							? map[TilemapInterface.FloorToTilePos(pos)]
							: new MapUnit();

						// Note: this assumes the names of tile prefabs are the same as the ground material IDs!
						string tileName = tilemap.GetTile(pos.ToVector3Int())?.name;
						if (tileName == null) continue;
						if (!ContentLibrary.Instance.GroundMaterials.Contains(tileName))
						{
							Debug.LogError($"Failed to find ground material for tile: {tileName}");
							continue;
						}

						GroundMaterial material = ContentLibrary.Instance.GroundMaterials.Get(tileName);
						string tag = tilemap.tag;
						switch (tag)
						{
							case "GroundTilemap":
								mapUnit.groundMaterial = material;
								break;
							case "GroundCoverTilemap":
								mapUnit.groundCover = material;
								break;
							case "CliffsTilemap":
								mapUnit.cliffMaterial = material;
								break;
							default:
								throw new Exception($"Invalid tilemap tag: {tag}");
						};
						// Every tile must have a ground material
						mapUnit.groundMaterial ??= ContentLibrary.Instance.GroundMaterials.Get("sand");
						map[TilemapInterface.FloorToTilePos(pos)] = mapUnit;
					}
				});

		// Find every entity in the prefab and add data about it to the map
		foreach (Transform transform in scenePrefab.GetComponentsInChildren<Transform>())
		{
			if (!transform.TryGetComponent(out EntityObject prefabEntity)) continue;

			string id = prefabEntity.EntityId;
			if (!ContentLibrary.Instance.Entities.Contains(id))
			{
				Debug.LogError($"Prefab entity has unrecognized ID: {id}", prefabEntity);
				continue;
			}

			EntityData entityData = ContentLibrary.Instance.Entities.Get(id);
			Vector2Int rootPos = prefabEntity.transform.position.ToVector2Int() - tilemapOffset;
			List<SavedComponentState> saveData = prefabEntity.GetSaveData();

			entityData.BaseShape.ForEach(
				offset =>
				{
					if (!map.ContainsKey(rootPos + offset))
					{
						Debug.LogWarning($"Entity object {id} in scene prefab covers an empty tile.");
						map[rootPos + offset] = new MapUnit();
					}
					map[rootPos + offset].entityId = id;
					map[rootPos + offset].savedComponents = saveData;
					map[rootPos + offset].relativePosToEntityOrigin = offset;
				});
		}

		return new RegionMap
		{
			mapDict = new Dictionary<string, Dictionary<Vector2Int, MapUnit>> {{SceneObjectManager.WorldSceneId, map}}
		};
	}

	/// Constructs a map for a scene object that has been created from a prefab, and adds
	/// it to the current region map.
	public static void BuildMapForScene(string scene, GameObject sceneRootObject)
	{
		Debug.Log($"Building map for {scene}, from {sceneRootObject}");
		if (currentRegion == null || currentRegion.mapDict == null)
		{
			Debug.LogError("Can't build map for scene; world map hasn't been initialized!");
			return;
		}
		Dictionary<Vector2Int, MapUnit> map = new();
		Dictionary<Vector2Int, GameObject> objectMap = new();

		Tilemap groundTilemap = sceneRootObject.GetComponentsInChildren<Tilemap>()
			.FirstOrDefault(tilemapObj => tilemapObj.CompareTag("GroundTilemap"));
		Tilemap groundCoverTilemap = sceneRootObject.GetComponentsInChildren<Tilemap>()
			.FirstOrDefault(tilemapObj => tilemapObj.CompareTag("GroundCoverTilemap"));
		Tilemap cliffTilemap = sceneRootObject.GetComponentsInChildren<Tilemap>()
			.FirstOrDefault(tilemapObj => tilemapObj.CompareTag("CliffsTilemap"));

		Vector2Int tilemapOffset = (groundTilemap?.transform.position.ToVector2Int()).GetValueOrDefault();

		if (groundTilemap == null)
		{
			if (currentRegion.mapDict.ContainsKey(scene))
				currentRegion.mapDict[scene] = map;
			else
				currentRegion.mapDict.Add(scene, map);
			Debug.LogWarning("tried to build maps for a scene containing no tilemap");
			return;
		}

		// Find the ground material of every tile in the scene.
		ImmutableList.Create(groundTilemap, groundCoverTilemap, cliffTilemap)
			.ForEach(tilemap =>
			{
				if (tilemap == null) return;
				foreach (Vector3 pos in tilemap.cellBounds.allPositionsWithin)
				{
					MapUnit mapUnit = map.ContainsKey(TilemapInterface.FloorToTilePos(pos))
						? map[TilemapInterface.FloorToTilePos(pos)]
						: new MapUnit();

					// Note: this assumes the names of tile prefabs are the same as the ground material IDs!
					string tileName = tilemap.GetTile(pos.ToVector3Int())?.name;
					if (tileName == null) continue;
					if (!ContentLibrary.Instance.GroundMaterials.Contains(tileName))
					{
						Debug.LogError($"Failed to find ground material for tile: {tileName}");
						continue;
					}

					GroundMaterial material = ContentLibrary.Instance.GroundMaterials.Get(tileName);
					string tag = tilemap.tag;
					switch (tag)
					{
						case "GroundTilemap":
							mapUnit.groundMaterial = material;
							break;
						case "GroundCoverTilemap":
							mapUnit.groundCover = material;
							break;
						case "CliffsTilemap":
							mapUnit.cliffMaterial = material;
							break;
						default:
							throw new Exception($"Invalid tilemap tag: {tag}");
					};
					// Every tile must have a ground material
					mapUnit.groundMaterial ??= ContentLibrary.Instance.GroundMaterials.Get("sand");
					map[TilemapInterface.FloorToTilePos(pos)] = mapUnit;
				}
			});

		// Add this new scene to the region map.
		if (currentRegion.mapDict.ContainsKey(scene)) { currentRegion.mapDict[scene] = map; }
		else { currentRegion.mapDict.Add(scene, map); }

		if (entityObjectMap.ContainsKey(scene)) { entityObjectMap[scene] = objectMap; }
		else { entityObjectMap.Add(scene, objectMap); }

		// Find all existing entities in the scene we just created, and destroy and then properly spawn them.
		ImmutableList<EntityObject> entitiesInScene = sceneRootObject.GetComponentsInChildren<EntityObject>().ToImmutableList();
		foreach (EntityObject prefabEntity in entitiesInScene)
		{
			string id = prefabEntity.EntityId;
			Vector2Int pos = prefabEntity.transform.position.ToVector2Int() - tilemapOffset;

			if (map.ContainsKey(pos))
			{
				map[pos].entityId = id;
			}

			EntityData entity = ContentLibrary.Instance.Entities.Get(id);
			if (entity == null)
			{
				Debug.LogError($"Prefab entity has unrecognized ID \"{id}\".", prefabEntity);
				continue;
			}
			// Now destroy the game object and respawn it, to make sure everything is in order
			List<SavedComponentState> saveData = prefabEntity.GetSaveData();
			Destroy(prefabEntity.gameObject);
			EntityObject newEntity = PlaceEntityAtPoint(entity, pos, scene, entity.BaseShape);
			newEntity.SetState(saveData);

			if (newEntity.GetComponent<RegionPortal>() != null)
			{
				Debug.Log($"Found region portal.\nSave data: {saveData.Count}");
			}
		}
	}

	// Check that placement is legal before using this. Places the given entity at the given point in the given scene
	// using the given base shape, removing any entities currently there, and updates the map data appropriately.
	private static EntityObject PlaceEntityAtPoint (EntityData entity, Vector2Int point, string scene, IEnumerable<Vector2Int> baseShape) {
		if (entity == null)
		{
			throw new ArgumentNullException(nameof(entity));
		}

		// Make the actual object
		GameObject entityObject = GameObject.Instantiate (entity.EntityPrefab, SceneObjectManager.GetSceneObjectFromId(scene).transform);

		if (entity.PivotAtCenterOfTile)
			entityObject.transform.localPosition = TilemapInterface.GetCenterPositionOfTile (new Vector2 (point.x, point.y));
		else
			entityObject.transform.localPosition = new Vector2 (point.x, point.y);

		// Give the entity an EntityTag component so we can know what it is from the object
		EntityObject entityTag = entityObject.GetComponent<EntityObject>();
		if (entityTag == null)
		{
			entityTag = entityObject.AddComponent<EntityObject>();
		}
		entityTag.EntityId = entity.Id;

		// Add the entity data to the maps
		foreach (Vector2Int entitySection in baseShape) {
			// Get rid of anything already there
			RemoveEntityAtPoint(point + entitySection, scene);

			// Add keys for these points if none exist
			if (!currentRegion.mapDict[scene].ContainsKey(point + entitySection))
			{
				currentRegion.mapDict[scene].Add(point + entitySection, new MapUnit());
			}
			if (!entityObjectMap[scene].ContainsKey(point + entitySection)) {
				entityObjectMap[scene].Add(point + entitySection, null);
			}

			entityObjectMap[scene] [point + entitySection] = entityObject;
			currentRegion.mapDict [scene] [point + entitySection].entityId = entity.Id;
			currentRegion.mapDict [scene] [point + entitySection].relativePosToEntityOrigin = entitySection;
		}
		return entityTag;
	}

	// Takes a tile position in scene coordinates.
	private static bool TileIsWalkable (string scene, Vector2Int pos)
	{
		MapUnit tile = GetMapUnitAtPoint(pos, scene);
		if (tile == null)
		{
			Debug.LogError("No tile found at given location (" + pos.x + ", " + pos.y + ")");
			return false;
		}

		if (tile.groundMaterial != null && !tile.groundMaterial.isWater)
		{
			if (tile.entityId == null || ContentLibrary.Instance.Entities.Get(tile.entityId).CanBeWalkedThrough)
			{
				return true;
			}
		}
		return false;
	}

	private void OnDestroy()
	{
		regionLoaded = null;
	}
}
