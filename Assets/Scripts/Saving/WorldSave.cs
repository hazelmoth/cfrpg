using System.Collections.Generic;
using ContinentMaps;
using UnityEngine;

[System.Serializable]
public class WorldSave
{
	public bool newlyCreated;
	public string worldName;
	public string saveFileId;
	public ulong time;
	public string playerActorId;
	public History.EventLog eventLog;
	public Vector2IntSerializable regionSize;
	public SerializableWorldMap worldMap;
	public Vector2IntSerializable currentRegionCoords;
	public List<SavedEntity> entities;
	public List<SavedActor> actors;

	public WorldSave(
		string worldName,
		ulong time,
		string playerActorId,
		History.EventLog eventLog,
		Vector2IntSerializable regionSize,
		SerializableWorldMap worldMap,
		Vector2IntSerializable currentRegionCoords,
		List<SavedEntity> entities,
		List<SavedActor> actors,
		bool newlyCreated)
	{
        this.worldName = worldName;
		this.time = time;
		this.playerActorId = playerActorId;
		this.eventLog = eventLog;
		this.regionSize = regionSize;
		this.worldMap = worldMap;
		this.currentRegionCoords = currentRegionCoords;
		this.entities = entities;
		this.actors = actors;
		this.newlyCreated = newlyCreated;
	}
}
