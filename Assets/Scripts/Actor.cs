using System;
using System.Collections.Generic;
using ActorAnim;
using ActorComponents;
using Dialogue;
using Items;
using JetBrains.Annotations;
using UnityEngine;

// A parent class to encompass both the player and Actors, for the purpose of things like health, Actor pathfinding,
// and teleporting actors between scenes when they activate portals.
public class Actor : MonoBehaviour, IImpactReceiver, IPickuppable, IInteractable, ISecondaryInteractable, IInteractMessage
{
	private const float KnockbackDist = 0.5f;
	
	[SerializeField] private string actorId;

	private ActorMovementController movementController;
	private ActorHealth health;

	public string ActorId => actorId;
	public bool PlayerControlled => actorId == PlayerController.PlayerActorId;
	public string CurrentScene { get; private set; }
	public Direction Direction => GetComponent<ActorSpriteController>().CurrentDirection;
	public bool InDialogue { get; private set; }
	public Occupiable CurrentWorkstation { get; set; }
	public bool UsingWorkstation => CurrentWorkstation != null;
	public Stack<Actor> HostileTargets { get; set; }
	public ActorNavigator Navigator => GetComponent<ActorNavigator>();

	/// The speed and direction that this actor is currently walking.
	public Vector2 WalkVector => movementController.WalkVector;

	private void Start()
	{
		movementController = GetComponent<ActorMovementController>();
		health = GetData().Get<ActorHealth>();

		DialogueManager.OnInitiateDialogue += OnPlayerEnterDialogue;
		DialogueManager.OnExitDialogue += OnPlayerExitDialogue;
	}

	private void Update()
	{
		if (PauseManager.Paused) return;

		if (health is { Dead:false }) health.Regen(TimeKeeper.DeltaTicks);
		
		// Disable colliders if this actor is dead/unconscious
		SetColliderToTrigger(health != null && (health.Sleeping || health.Dead));

		// Remove the top actor from the stack if dead or gone
		if (HostileTargets.Count > 0 &&
		    (HostileTargets.Peek() == null
			    || HostileTargets.Peek().GetData().Get<ActorHealth>() is { Dead: true }))
		{
			HostileTargets.Pop();
		}
	}

	public void Initialize(string id)
	{
		actorId = id;
		health = GetData().Get<ActorHealth>();
		HostileTargets = new Stack<Actor>();

		if (health != null)
		{
			health.OnDeath += OnDeath;
			SetColliderToTrigger(health.Dead || health.Sleeping);
		}
	}

	private void OnDestroy()
	{
		if (!ActorRegistry.IdIsRegistered(ActorId)) return;
		if (health != null) health.OnDeath -= OnDeath;
	}

	public bool CurrentlyPickuppable => health is { Dead: true };

	public ItemStack ItemPickup
	{
		get
		{
			string id = ItemIdParser.SetModifier("corpse", "actor_id", ActorId);
			id = ItemIdParser.SetModifier(id, "race", GetData().RaceId);
			return new ItemStack(id, 1);
		}
	}

	void IPickuppable.OnPickup() { Destroy(gameObject); }

	void IImpactReceiver.OnImpact(ImpactInfo impact)
	{
		// Knock back the actor
		GetComponent<ActorMovementController>().KnockBack(impact.force.normalized * KnockbackDist);
		// Take damage
		health?.TakeHit(impact.force.magnitude);
		// Get mad at the attacker, if there was one
		if (impact.source != null && impact.source.actorId != actorId && !HostileTargets.Contains(impact.source)) 
			HostileTargets.Push(impact.source);
	}

    /// Returns the data associated with this Actor
	public ActorData GetData()
	{
		if (!ActorRegistry.IdIsRegistered(ActorId))
		{
			Debug.LogError($"This actor isn't registered: {ActorId}");
			return new ActorData("missing", "missing");
		}

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

	/// The precise location of this actor, within the loaded region.
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

	/// The location of the tile this actor is standing on.
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

	/// Parents this actor to the given scene. Doesn't move the actor in world space.
	public void MoveActorToScene (string scene) {
		if (string.IsNullOrEmpty(scene))
		{
			Debug.LogError("Tried to move actor to scene using an empty scene ID!");
			return;
		}
		this.CurrentScene = scene;
		GameObject sceneRoot = SceneObjectManager.GetSceneObjectFromId (scene);
		if (sceneRoot == null)
			Debug.LogError($"Failed to get root object of scene {scene}.");
		else {
			this.gameObject.transform.SetParent (sceneRoot.transform);
		}
	}

	private void OnDeath ()
	{
		if (health is not { Dead: true })
		{
			Debug.LogError("This actor has died but isn't marked as dead!");
		}
		Debug.Log(GetData().ActorName + " has been killed.");
		
		// If this is the player actor, trigger the death sequence.
		if (PlayerControlled) PlayerDeathSequence.HandleDeath(ActorId);
	}

	private void SetColliderToTrigger(bool isTrigger)
	{
		Collider2D collider = GetComponent<Collider2D>();
		if (collider != null)
		{
			collider.isTrigger = isTrigger;
		}
	}

	private void OnPlayerEnterDialogue(Actor other)
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

	public void OnInteract()
	{
		// Doesn't do anything, but the interface is necessary for dialogue to recognize a
		// dialogue target.
	}

    public void OnSecondaryInteract(Actor interactor)
    {
		// Secondary interaction is opening this actor's inv as a container, if dead
		if (health is not { Dead: true }) return;

		ActorInventory inv = GetData().Get<ActorInventory>();
		if (inv == null || inv.IsEmpty()) return;

		ActorInventory interactorInv = interactor.GetData().Get<ActorInventory>();
		interactorInv?.OpenContainer(inv);
    }

    public string GetInteractMessage()
    {
	    if (health is not { Dead: true }) return null;

	    ActorInventory inv = GetData().Get<ActorInventory>();
		if (inv is not {Empty: false}) return null;

		return "R to loot";
    }
}
