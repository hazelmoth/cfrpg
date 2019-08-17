using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldSave
{
	public SerializableWorldMap worldMap;
	public List<SavedEntity> entities;
	public List<SavedNpc> npcs;

	public WorldSave(SerializableWorldMap worldMap, List<SavedEntity> entities, List<SavedNpc> npcs)
	{
		this.worldMap = worldMap;
		this.entities = entities;
		this.npcs = npcs;
	}
}
