using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Stores the save information for both an actor's data and its physical location
[System.Serializable]
public class SavedActor
{
	public string scene;
	public Vector2 location;
	public Direction direction;
	public SerializableActorData data;

	public SavedActor (Actor sourceActor)
	{
		scene = sourceActor.CurrentScene;
		location = TilemapInterface.WorldPosToScenePos(sourceActor.transform.position, sourceActor.CurrentScene);
		direction = sourceActor.Direction;
		data = new SerializableActorData(sourceActor.GetData());
	}
}
