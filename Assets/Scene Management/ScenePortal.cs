using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenePortal : MonoBehaviour, InteractableObject
{
	[SerializeField] string destinationScenePrefabId = null;
	[SerializeField] Direction entryDirection = 0;
	[SerializeField] bool activateOnTouch = false;

	public string DestinationScenePrefabId {get{return destinationScenePrefabId;}}
	public string DestinationSceneObjectId { get; private set; }
	public Vector2 PortalExitRelativeCoords { get; private set; } = new Vector2();
	public Direction EntryDirection {get{return entryDirection;}}
	public bool ActivateOnTouch {get{return activateOnTouch;}}

	void Start () {
		
	}
	public void OnInteract ()
	{
		Debug.Log ("Portal activated to " + DestinationSceneObjectId);
	}
	public void SetExitCoords (Vector2 coords) {
		PortalExitRelativeCoords = coords;
	}
	public void SetExitSceneObjectId (string sceneId) {
		this.DestinationSceneObjectId = sceneId;
	}
}
