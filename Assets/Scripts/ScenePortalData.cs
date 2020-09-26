using UnityEngine;

[System.Serializable]
public class SerializableScenePortal
{
	public Vector2Serializable sceneRelativeLocation;
	// The pseudoscene the portal itself is located in
	public string portalScene;
	public string destinationScenePrefabId;
	public string destinationSceneObjectId;
	public Vector2Serializable portalExitRelativeCoords;
	public Direction entryDirection;
	public bool activateOnTouch;
	public bool ownedByEntity;

	public SerializableScenePortal(Vector2 sceneRelativeLocation, string portalScene, string destinationScenePrefabId, string destinationSceneObjectId, Vector2 portalExitRelativeCoords, Direction entryDirection, bool activateOnTouch, bool ownedByEntity)
	{
		this.sceneRelativeLocation = sceneRelativeLocation.ToSerializable();
		this.portalScene = portalScene;
		this.destinationScenePrefabId = destinationScenePrefabId;
		this.destinationSceneObjectId = destinationSceneObjectId;
		this.portalExitRelativeCoords = portalExitRelativeCoords.ToSerializable();
		this.entryDirection = entryDirection;
		this.activateOnTouch = activateOnTouch;
		this.ownedByEntity = ownedByEntity;
	}
}
