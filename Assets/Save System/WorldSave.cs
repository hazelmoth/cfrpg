using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldSave
{
	public bool newlyCreated;
	public string worldName;
	public string saveFileId;
	public string playerActorId;
	public Vector2 worldSize;
	public SerializableWorldMap worldMap;
	public List<SavedEntity> entities;
	public List<SavedActor> actors;
	public List<SerializableScenePortal> scenePortals;

    public WorldSave(string worldName, SerializableWorldMap worldMap, Vector2Int worldSize, List<SavedEntity> entities, List<SavedActor> actors, string playerActorId, List<SerializableScenePortal> scenePortals, bool newlyCreated)
	{
        this.worldName = worldName;
		this.worldMap = worldMap;
		this.entities = entities;
		this.worldSize = worldSize;
		this.actors = actors;
		this.playerActorId = playerActorId;
		this.scenePortals = scenePortals;
		this.newlyCreated = newlyCreated;
	}
}
