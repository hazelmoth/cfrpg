using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Stores the save information for both an actor's data and its physical location
[System.Serializable]
public class SavedActor
{
	public bool inWorld;
	public string scene;
	public Vector2 location;
	public Direction direction;
	public SerializableActorData data;

	public SavedActor (ActorData sourceActor)
	{
		if (ActorRegistry.Get(sourceActor.actorId).actorObject != null) {
			inWorld = true;
			Actor actorObject = ActorRegistry.Get(sourceActor.actorId).actorObject;
			scene = actorObject.CurrentScene;
			location = TilemapInterface.WorldPosToScenePos(actorObject.transform.position, actorObject.CurrentScene);
			direction = actorObject.Direction;
		} else
		{
			inWorld = false;
		}

		data = new SerializableActorData(sourceActor);
	}
}
