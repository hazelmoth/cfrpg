using UnityEngine;

public class ScenePortal : MonoBehaviour, IInteractable
{
	[SerializeField] private string destinationScenePrefabId = null;
	[SerializeField] private Direction entryDirection = 0;
	[SerializeField] private bool activateOnTouch = false;
	// Is this scene portal a child of an entity?
	// We need to know this because scene portals owned by entities are saved and loaded through SaveableComponents rather than on their own
	[SerializeField] private bool ownedByEntity = false;

	public string DestinationScenePrefabId => destinationScenePrefabId;
	public string DestinationSceneObjectId { get; private set; }
	public string PortalScene { get; private set; }

	// The scene-relative coordinates where this portal spits people out (regardless of other portals in that scene)
	public Vector2 PortalExitSceneCoords { get; private set; } = new Vector2();
	public Direction EntryDirection {get{return entryDirection;}}
	public bool ActivateOnTouch {get{return activateOnTouch;}}

	private void Start()
	{
		PortalScene = SceneObjectManager.GetSceneIdForObject(this.gameObject);
	}
	public SerializableScenePortal GetData()
	{
		PortalScene = SceneObjectManager.GetSceneIdForObject(this.gameObject);
		return new SerializableScenePortal(
			TilemapInterface.WorldPosToScenePos(
				this.transform.position,
				SceneObjectManager.GetSceneIdForObject(gameObject)),
			PortalScene,
			destinationScenePrefabId,
			DestinationSceneObjectId,
			PortalExitSceneCoords,
			EntryDirection,
			ActivateOnTouch,
			ownedByEntity);
	}
	public void SetData(SerializableScenePortal data)
	{
		destinationScenePrefabId = data.destinationScenePrefabId;
		PortalScene = data.portalScene;
		DestinationSceneObjectId = data.destinationSceneObjectId;
		PortalExitSceneCoords = data.portalExitRelativeCoords.ToVector2();
		entryDirection = data.entryDirection;
		activateOnTouch = data.activateOnTouch;
		ownedByEntity = data.ownedByEntity;
	}
	public void SetExitSceneObjectId (string sceneObjectId)
	{
		DestinationSceneObjectId = sceneObjectId;
	}
	public void OnInteract ()
	{
		Debug.Log ("Portal activated to " + DestinationSceneObjectId);
	}
	public void SetExitCoords (Vector2 coords) {
		PortalExitSceneCoords = coords;
	}
}
