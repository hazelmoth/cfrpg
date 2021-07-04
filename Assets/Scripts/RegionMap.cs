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
}
