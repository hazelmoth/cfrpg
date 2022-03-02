using System.Collections.Generic;
using UnityEngine;

public class RegionMap
{
	// Maps scenes to dictionaries
	public IDictionary<string, Dictionary<Vector2Int, MapUnit>> mapDict;
	public IList<SerializableScenePortal> scenePortals;
	// Map Actor IDs to actor locations
	public Dictionary<string, ActorPosition> actors;
	public IList<SavedDroppedItem> droppedItems;

	[System.Serializable]
	public struct ActorPosition
	{
		public ActorPosition(Location location, Direction direction)
		{
			this.location = location;
			this.direction = direction;
		}
		public Location location;
		public Direction direction;
	}

	public RegionMap()
	{
		mapDict = new Dictionary<string, Dictionary<Vector2Int, MapUnit>>();
		scenePortals = new List<SerializableScenePortal>();
		actors = new Dictionary<string, ActorPosition>();
		droppedItems = new List<SavedDroppedItem>();
	}

	/// Sets the map unit at the given location to have the given tile. Creates a new
	/// map unit if one does not exist at the given location. Logs an error if the
	/// location references a scene that is not in the map.
	public void SetTile(TileLocation location, TilemapLayer layer, string id)
	{
		if (!mapDict.ContainsKey(location.scene))
		{
			Debug.LogError(
				$"Tried to set tile at location {location} in scene {location.scene} but that scene is not in the map.");
			return;
		}
		// Create a new map unit if one does not exist at the given location
		if (!mapDict[location.scene].ContainsKey(location.Vector2Int))
			mapDict[location.scene].Add(location.Vector2Int, new MapUnit());

		mapDict[location.scene][location.Vector2Int].SetTile(layer, id);
	}
}
