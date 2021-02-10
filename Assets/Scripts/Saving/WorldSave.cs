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
	public SerializableContinentMap continentMap;
	public Vector2IntSerializable currentRegionCoords;
	public List<SavedEntity> entities;
	public List<SavedActor> actors;
	public List<SavedDroppedItem> items;
	public List<SerializableScenePortal> scenePortals;

	public WorldSave(
		string worldName,
		ulong time,
		string playerActorId,
		History.EventLog eventLog,
		Vector2IntSerializable regionSize,
		SerializableContinentMap continentMap,
		Vector2IntSerializable currentRegionCoords,
		List<SavedEntity> entities,
		List<SavedActor> actors,
		List<SavedDroppedItem> items,
		List<SerializableScenePortal> scenePortals,
		bool newlyCreated)
	{
        this.worldName = worldName;
		this.time = time;
		this.playerActorId = playerActorId;
		this.eventLog = eventLog;
		this.regionSize = regionSize;
		this.continentMap = continentMap;
		this.currentRegionCoords = currentRegionCoords;
		this.entities = entities;
		this.actors = actors;
		this.items = items;
		this.scenePortals = scenePortals;
		this.newlyCreated = newlyCreated;
	}
}
