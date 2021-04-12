using Newtonsoft.Json;

// Stores the save information for both an actor's data and its physical location
[System.Serializable]
public class SavedActor
{
	public SerializableActorData data;

	[JsonConstructor]
	public SavedActor() { }

	public SavedActor (ActorData sourceActor)
	{
		data = new SerializableActorData(sourceActor);
	}
}
