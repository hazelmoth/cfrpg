using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldSave
{
    public string worldName;
	public SerializableWorldMap worldMap;
	public List<SavedEntity> entities;
	public List<SavedNpc> npcs;

    public WorldSave(string worldName, SerializableWorldMap worldMap, List<SavedEntity> entities, List<SavedNpc> npcs)
	{
        this.worldName = worldName;
		this.worldMap = worldMap;
		this.entities = entities;
		this.npcs = npcs;
	}
}
