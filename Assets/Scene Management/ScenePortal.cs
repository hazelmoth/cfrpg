using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenePortal : MonoBehaviour, InteractableObject
{
	[SerializeField] string destinationScene;
	// The coordinates of the desired entrance, in relative to the scene parent object
	[SerializeField] Vector2 sceneEntryRelativeCoords;
	[SerializeField] Direction entryDirection;
	[SerializeField] bool activateOnTouch;

	public string DestinationScene {get{return destinationScene;}}
	public Vector2 SceneEntryRelativeCoords {get{return sceneEntryRelativeCoords;}}
	public Direction ExitDirection {get{return entryDirection;}}
	public bool ActivateOnTouch {get{return activateOnTouch;}}

	public void OnInteract ()
	{
		Debug.Log ("Portal activated to " + destinationScene);
	}
}
