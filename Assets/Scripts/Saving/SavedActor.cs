using Newtonsoft.Json;

// Stores the save information for both an actor's data and its physical location
[System.Serializable]
public class SavedActor
{
	public bool inWorld;
	public string scene;
	public Vector2Serializable location;
	public Direction direction;
	public SerializableActorData data;

	[JsonConstructor]
	public SavedActor() { }

	public SavedActor (ActorData sourceActor)
	{
		if (ActorRegistry.Get(sourceActor.actorId).actorObject != null) {
			inWorld = true;
			Actor actorObject = ActorRegistry.Get(sourceActor.actorId).actorObject;
			scene = actorObject.CurrentScene;
			location = TilemapInterface.WorldPosToScenePos(actorObject.transform.position, actorObject.CurrentScene).ToSerializable();
			direction = actorObject.Direction;
		} else
		{
			inWorld = false;
		}

		data = new SerializableActorData(sourceActor);
	}
}
