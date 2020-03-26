using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// A parent class to encompass both the player and NPCs, for the purpose of things like health, NPC pathfinding,
// and teleporting actors between scenes when they activate portals.
public abstract class Actor : MonoBehaviour, PunchReciever
{
	[SerializeField] string actorId;
	ActorPhysicalCondition physicalCondition;
	protected string scene = SceneObjectManager.WorldSceneId;
	protected NPCBehaviourAI behaviourAi;
	protected ActorInventory inventory;
	protected ActorSettlementData settlementData;
	public string ActorId { get => actorId; protected set => actorId = value; }
	public string CurrentScene => scene;
	public Direction Direction => GetComponent<HumanAnimController>().GetDirection();
	public bool IsDead => physicalCondition.IsDead;
	
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
	
	public NPCBehaviourAI BehaviourAi
	{
		get
		{
			if (behaviourAi == null)
			{
				return GetComponent<NPCBehaviourAI>();
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

	public ActorSettlementData SettlementData
	{
		get => settlementData ?? (settlementData = new ActorSettlementData(null));
		set => settlementData = value;
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

