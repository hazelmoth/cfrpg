using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// A parent class to encompass both the player and NPCs, for the purpose of things like health, NPC pathfinding,
// and teleporting actors between scenes when they activate portals.
public abstract class Actor : MonoBehaviour, IPunchReceiver
{
	[SerializeField] string actorId;
	ActorPhysicalCondition physicalCondition;
	protected string actorName;
	protected string scene = SceneObjectManager.WorldSceneId;
	protected string personality;
	protected ActorBehaviourAi behaviourAi;
	protected ActorInventory inventory;
	protected FactionStatus factionStatus;
	public string ActorId { get => actorId; protected set => actorId = value; }
	public string ActorName { get => actorName; set => actorName = value; }
	public string CurrentScene => scene;
	public Direction Direction => GetComponent<HumanSpriteController>().CurrentDirection();
	public bool IsDead => physicalCondition.IsDead;
	public string Personality { get => personality; set => personality = value; }
	public string Race { get; set; }

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
	
	public ActorBehaviourAi BehaviourAi
	{
		get
		{
			if (behaviourAi == null)
			{
				return GetComponent<ActorBehaviourAi>();
			}
			else
			{
				return behaviourAi;
			}
		}
	}

	public ActorPhysicalCondition PhysicalCondition
	{
		get
		{
			if (physicalCondition == null)
			{
				physicalCondition = new ActorPhysicalCondition();
				physicalCondition.OnDeath += OnDeath;
			}

			return physicalCondition;
		}
		set
		{
			physicalCondition = value;
		}
	}
	
	public FactionStatus FactionStatus
	{
		get => factionStatus = factionStatus ?? new FactionStatus(null);
		set => factionStatus = value;
	}

	public virtual ActorInventory Inventory
	{
		get
		{
			if (inventory == null)
			{
				inventory = new ActorInventory();
				return inventory;
			}
			else
			{
				return inventory;
			}
		}
	}

	public void OnPunch(float strength, Vector2 direction)
	{
		if (PhysicalCondition == null)
		{
			return;
		}
		PhysicalCondition.TakeHit(strength);
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
		if (IsDead == false)
		{
			Debug.LogError("This actor has died but isn't marked as dead!");
		}
		Debug.Log(name + " has been killed.");
	}
	
}

