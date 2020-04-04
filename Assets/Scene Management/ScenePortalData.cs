using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableScenePortal
{
	public Vector2 sceneRelativeLocation;
	// The pseudoscene the portal itself is located in
	public string portalScene;
	public string destinationScenePrefabId;
	public string destinationSceneObjectId;
	public Vector2 portalExitRelativeCoords;
	public Direction entryDirection;
	public bool activateOnTouch;
	public bool ownedByEntity;

	public SerializableScenePortal(Vector2 sceneRelativeLocation, string portalScene, string destinationScenePrefabId, string destinationSceneObjectId, Vector2 portalExitRelativeCoords, Direction entryDirection, bool activateOnTouch, bool ownedByEntity)
	{
		this.sceneRelativeLocation = sceneRelativeLocation;
		this.portalScene = portalScene;
		this.destinationScenePrefabId = destinationScenePrefabId;
		this.destinationSceneObjectId = destinationSceneObjectId;
		this.portalExitRelativeCoords = portalExitRelativeCoords;
		this.entryDirection = entryDirection;
		this.activateOnTouch = activateOnTouch;
		this.ownedByEntity = ownedByEntity;
	}
}
