using System;
using JetBrains.Annotations;
using UnityEngine;

// A parent class to encompass both the player and Actors, for the purpose of things like health, Actor pathfinding,
// and teleporting actors between scenes when they activate portals.
public class Actor : MonoBehaviour, IImpactReceiver, IPickuppable, IInteractable
{
	private const float KnockbackDist = 0.5f;
	
	[SerializeField] private string actorId;

	public string ActorId => actorId;
	public bool PlayerControlled => actorId == PlayerController.PlayerActorId;
	public string CurrentScene { get; private set; }
	public Direction Direction => GetComponent<ActorSpriteController>().CurrentDirection;
	public bool InDialogue { get; private set; }
	public ActorNavigator Navigator => GetComponent<ActorNavigator>();

	[UsedImplicitly]
	private void Start()
	{
		DialogueManager.OnInitiateDialogue += OnPlayerEnterDialogue;
		DialogueManager.OnExitDialogue += OnPlayerExitDialogue;
	}

	private void Update()
	{
		if (GetData().PhysicalCondition.Sleeping || GetData().PhysicalCondition.IsDead)
		{
			SetColliderMode(true);
		}
		else
		{
			SetColliderMode(false);
		}
	}

	public void Initialize(string id)
	{
		actorId = id;
		LoadSprites();
		GetData().Inventory.OnHatEquipped += OnApparelEquipped;
		GetData().Inventory.OnPantsEquipped += OnApparelEquipped;
		GetData().Inventory.OnShirtEquipped += OnApparelEquipped;
		GetData().PhysicalCondition.OnDeath += OnDeath;
		SetColliderMode(GetData().PhysicalCondition.IsDead);
	}

	private void OnDestroy()
	{
		GetData().Inventory.OnHatEquipped -= OnApparelEquipped;
		GetData().Inventory.OnPantsEquipped -= OnApparelEquipped;
		GetData().Inventory.OnShirtEquipped -= OnApparelEquipped;
		GetData().PhysicalCondition.OnDeath -= OnDeath;
	}

	bool IPickuppable.CurrentlyPickuppable => GetData().PhysicalCondition.IsDead;

	ItemStack IPickuppable.ItemPickup => new ItemStack("actor:" + actorId, 1);

	void IImpactReceiver.OnImpact(Vector2 impact)
	{
		// Knock back the actor
		GetComponent<ActorMovementController>().KnockBack(impact.normalized * KnockbackDist);
		// Take damage
		GetData().PhysicalCondition?.TakeHit(impact.magnitude);
	}

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
			Vector2 scenePos = TilemapInterface.WorldPosToScenePos(pos, CurrentScene);
			scenePos = TilemapInterface.GetCenterPositionOfTile(scenePos);
			return new TileLocation(scenePos.ToVector2Int(), CurrentScene);
		}
	}

	public void MoveActorToScene (string scene) {
		if (string.IsNullOrEmpty(scene))
		{
			Debug.LogError("Tried to move actor to scene using an empty scene ID!");
			return;
		}
		this.CurrentScene = scene;
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
		Debug.Log(GetData().ActorName + " has been killed.");
	}

	private void LoadSprites()
	{
		ActorData data = GetData();

		if (data == null)
		{
			Debug.LogWarning("Loading sprites for a non-registered actor!");
			GetComponent<HumanSpriteLoader>().LoadSprites("human_light", null, null, null, null);
		}
		else
		{
			string hatId = data.Inventory.GetEquippedHat()?.id;
			string shirtId = data.Inventory.GetEquippedShirt()?.id;
			string pantsId = data.Inventory.GetEquippedPants()?.id;
			GetComponent<HumanSpriteLoader>().LoadSprites(data.Race, data.Hair, hatId, shirtId, pantsId);
		}
	}

	private void SetColliderMode(bool isTrigger)
	{
		Collider2D collider = GetComponent<Collider2D>();
		if (collider != null)
		{
			collider.isTrigger = isTrigger;
		}
	}

	private void OnApparelEquipped(ItemStack apparel)
	{
		LoadSprites();
	}

	private void OnPlayerEnterDialogue(Actor other, DialogueDataMaster.DialogueNode startNode)
	{
		if (other == this)
		{
			InDialogue = true;
		}
	}

	private void OnPlayerExitDialogue()
	{
		InDialogue = false;
	}

	void IInteractable.OnInteract()
	{
		
	}
}