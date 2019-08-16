using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldSave
{
	public SerializableWorldMap worldMap;
	public List<SavedEntity> entities;

	public WorldSave(SerializableWorldMap worldMap, List<SavedEntity> entities)
	{
		this.worldMap = worldMap;
		this.entities = entities;
	}
}
