using System;
using System.Collections.Generic;
using ContentLibraries;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

// A parent class to encompass both the player and Actors, for the purpose of things like health, Actor pathfinding,
// and teleporting actors between scenes when they activate portals.
public class Actor : MonoBehaviour, IImpactReceiver, IPickuppable, IDualInteractable, IInteractMessage
{
	private const float KnockbackDist = 0.5f;
	
	[SerializeField] private string actorId;

	public string ActorId => actorId;
	public bool PlayerControlled => actorId == PlayerController.PlayerActorId;
	public string CurrentScene { get; private set; }
	public Direction Direction => GetComponent<ActorSpriteController>().CurrentDirection;
	public bool InDialogue { get; private set; }
	public Stack<Actor> HostileTargets { get; set; }
	public ActorNavigator Navigator => GetComponent<ActorNavigator>();
	
	

	[UsedImplicitly]
	private void Start()
	{
		DialogueManager.OnInitiateDialogue += OnPlayerEnterDialogue;
		DialogueManager.OnExitDialogue += OnPlayerExitDialogue;
	}

	private void Update()
	{
		if (PauseManager.Paused) return;
		
		// Disable colliders if this actor is dead/unconscious
		if (GetData().PhysicalCondition.Sleeping || GetData().PhysicalCondition.IsDead)
		{
			SetColliderMode(true);
		}
		else
		{
			SetColliderMode(false);
		}
		// Remove the top actor from the stack if dead or gone
		if (HostileTargets.Count > 0 &&
		    (HostileTargets.Peek() == null || HostileTargets.Peek().GetData().PhysicalCondition.IsDead))
		{
			HostileTargets.Pop();
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
		HostileTargets = new Stack<Actor>();
	}

	private void OnDestroy()
	{
		if (!ActorRegistry.IdIsRegistered(ActorId)) return;
		GetData().Inventory.OnHatEquipped -= OnApparelEquipped;
		GetData().Inventory.OnPantsEquipped -= OnApparelEquipped;
		GetData().Inventory.OnShirtEquipped -= OnApparelEquipped;
		GetData().PhysicalCondition.OnDeath -= OnDeath;
	}

	public bool CurrentlyPickuppable => GetData().PhysicalCondition.IsDead;
	
	ItemStack IPickuppable.ItemPickup => new ItemStack("actor:" + actorId, 1);
	
	string IInteractMessage.GetInteractMessage () => CurrentlyPickuppable ? "R to butcher" : "";

	void IInteractable.OnInteract() { }
	
	void IDualInteractable.OnSecondaryInteract()
	{
		if (!CurrentlyPickuppable) return;
		
		ActorRace race = ContentLibrary.Instance.Races.Get(GetData().Race);
		for (int i = 0; i < race.butcherDrops.maxQuantity; i++)
		{
			if (Random.value < race.butcherDrops.dropProbability)
			{
				DroppedItemSpawner.SpawnItem(new ItemStack(race.butcherDrops.itemId, 1), Location.Vector2,
					CurrentScene, true);
			}
		}
		GameObject.Destroy(this.gameObject);
	}

	void IImpactReceiver.OnImpact(ImpactInfo impact)
	{
		// Knock back the actor
		GetComponent<ActorMovementController>().KnockBack(impact.force.normalized * KnockbackDist);
		// Take damage
		GetData().PhysicalCondition?.TakeHit(impact.force.magnitude);
		// Get mad at the attacker, if there was one
		if (impact.source != null && impact.source.actorId != actorId && !HostileTargets.Contains(impact.source)) 
			HostileTargets.Push(impact.source);
	}

	public ActorData GetData()
	{
		Debug.Assert(ActorRegistry.IdIsRegistered(ActorId), $"This actor isn't registered: {ActorId}");
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

	// The precise location of this actor, within the loaded region.
	public Location Location
	{
		get
		{
			Debug.Assert(SceneObjectManager.SceneExists(CurrentScene));
			
			Vector2 pos = transform.position;
			Vector2 scenePos = TilemapInterface.WorldPosToScenePos(pos, CurrentScene);
			return new Location(scenePos, CurrentScene);
		}
	}

	// The location of the tile this actor is standing on.
	public TileLocation TileLocation
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
}