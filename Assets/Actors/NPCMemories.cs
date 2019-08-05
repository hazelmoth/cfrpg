using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCLocationMemories
{
	private class EntityMemory
	{
		public string EntityId { get; }
		public Vector2Int Location { get; }

		public EntityMemory(string entityId, Vector2Int location)
		{
			this.EntityId = entityId;
			this.Location = location;
		}
	}

	private List<EntityMemory> memories;

	private EntityMemory GetMemoryForLocation (Vector2Int location)
	{
		foreach (EntityMemory memory in memories)
		{
			if (memory.Location == location)
			{
				return memory;
			}
		}
		return null;
	}

	public void AddMemoryOfEntity(string entityId, Vector2Int location) {
		EntityMemory existingMemory = GetMemoryForLocation(location);
		if (existingMemory == null)
			memories.Remove(existingMemory);

		EntityMemory newMemory = new EntityMemory(entityId, location);
		memories.Add(newMemory);
	}

	public List<Vector2Int> GetLocationsOfEntity (string entityId)
	{
		if (memories == null)
			return new List<Vector2Int>();

		List<Vector2Int> locations = new List<Vector2Int>();
		foreach (EntityMemory memory in memories)
		{
			if (memory.EntityId == entityId)
			{
				locations.Add(memory.Location);
			}
		}
		return locations;
	}

}
