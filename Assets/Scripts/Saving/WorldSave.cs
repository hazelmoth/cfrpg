using System.Collections.Generic;

[System.Serializable]
public class WorldSave
{
	public bool newlyCreated;
	public string worldName;
	public string saveFileId;
	public ulong time;
	public string playerActorId;
	public History.EventLog eventLog;
	public Vector2IntSerializable worldSize;
	public SerializableWorldMap worldMap;
	public List<SavedEntity> entities;
	public List<SavedActor> actors;
	public List<SerializableScenePortal> scenePortals;

	public WorldSave(string worldName, ulong time, SerializableWorldMap worldMap, Vector2IntSerializable worldSize, History.EventLog eventLog, List<SavedEntity> entities, List<SavedActor> actors, string playerActorId, List<SerializableScenePortal> scenePortals, bool newlyCreated)
	{
        this.worldName = worldName;
		this.time = time;
		this.worldMap = worldMap;
		this.entities = entities;
		this.worldSize = worldSize;
		this.eventLog = eventLog;
		this.actors = actors;
		this.playerActorId = playerActorId;
		this.scenePortals = scenePortals;
		this.newlyCreated = newlyCreated;
	}
}
