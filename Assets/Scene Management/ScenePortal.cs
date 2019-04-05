using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenePortal : MonoBehaviour, InteractableObject
{
	[SerializeField] string destinationScenePrefabId = null;
	[SerializeField] Direction entryDirection = 0;
	[SerializeField] bool activateOnTouch = false;

	Vector2 portalExitRelativeCoords = new Vector2();

	// The ID of the live scene object this portal leads to
	string destinationSceneObjectId;

	public string DestinationScenePrefabId {get{return destinationScenePrefabId;}}
	public string DestinationSceneObjectId {get{return destinationSceneObjectId;}}
	public Vector2 PortalExitRelativeCoords {get{return portalExitRelativeCoords;}}
	public Direction EntryDirection {get{return entryDirection;}}
	public bool ActivateOnTouch {get{return activateOnTouch;}}

	void Start () {
		
	}
	public void OnInteract ()
	{
		Debug.Log ("Portal activated to " + destinationSceneObjectId);
	}
	public void SetExitCoords (Vector2 coords) {
		portalExitRelativeCoords = coords;
	}
	public void SetExitSceneObjectId (string sceneId) {
		this.destinationSceneObjectId = sceneId;
	}
}
