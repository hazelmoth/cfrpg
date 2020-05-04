using UnityEngine;

// A parent class to encompass both the player and NPCs, for the purpose of things like health, NPC pathfinding,
// and teleporting actors between scenes when they activate portals.
public abstract class Actor : MonoBehaviour, IPunchReceiver
{
	[SerializeField] string actorId;
	protected string scene;
	public string ActorId { get => actorId; protected set => actorId = value; }
	public string CurrentScene => scene;
	public Direction Direction => GetComponent<HumanSpriteController>().CurrentDirection();

	public ActorData GetData()
	{
		return ActorRegistry.Get(ActorId)?.data;
	}

	public GameObject SpritesObject
	{
		get
		{
			Transform found = transform.Find("Sprites");
			return found ? found.gameObject : null;
		}
	}

	public TileLocation Location
	{
		get
		{
			Vector2 pos = transform.position;
			Vector2 scenePos = TilemapInterface.WorldPosToScenePos(pos, scene);
			scenePos = TilemapInterface.GetCenterPositionOfTile(scenePos);
			return new TileLocation(scenePos.ToVector2Int(), scene);
		}
	}

	public void OnPunch(float strength, Vector2 direction)
	{
		GetData().PhysicalCondition?.TakeHit(strength);
	}

	public void MoveActorToScene (string scene) {

		this.scene = scene;
		GameObject sceneRoot = SceneObjectManager.GetSceneObjectFromId (scene);
		if (sceneRoot != null) {
			this.gameObject.transform.SetParent (sceneRoot.transform);
		}
	}

	protected virtual void OnDeath ()
	{
		if (GetData().PhysicalCondition.IsDead == false)
		{
			Debug.LogError("This actor has died but isn't marked as dead!");
		}
		Debug.Log(name + " has been killed.");
	}
}