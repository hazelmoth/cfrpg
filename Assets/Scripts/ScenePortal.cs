using UnityEngine;

public class ScenePortal : MonoBehaviour, IInteractable
{
	[SerializeField] private string destinationScenePrefabId = null;
	[SerializeField] private Direction entryDirection = 0;
	[SerializeField] private bool activateOnTouch = false;
	// Is this scene portal a child of an entity?
	// We need to know this because scene portals owned by entities are saved and loaded
	// through SaveableComponents rather than on their own.
	[SerializeField] private bool ownedByEntity = false;

	/// The prefab for the scene that this portal will lead to.
	public string DestinationScenePrefabId => destinationScenePrefabId;

	/// The ID of the actual scene this portal leads to.
	/// May be null if the scene is not created yet.
	public string DestinationSceneObjectId { get; private set; }

	/// The scene in which this portal is located.
	public string PortalScene { get; private set; }

	/// The scene-relative coordinates where this portal spits people out (regardless of
	/// the locations of any portals in that scene).
	public Vector2 PortalExitSceneCoords { get; private set; } = new Vector2();
	public Direction EntryDirection => entryDirection;
	public bool ActivateOnTouch => activateOnTouch;

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
		Debug.Log("Exit pos: " + PortalExitSceneCoords);
	}
	public void SetExitCoords (Vector2 coords) {
		PortalExitSceneCoords = coords;
	}
}
