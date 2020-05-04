using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Stores the ID of this NPC and accesses the appropriate classes
// to load data based on that ID.
public class NPC : Actor, InteractableObject
{
	NPCCharacter npcCharacter;
	NPCNavigator npcNavigator;
	NPCLocationMemories memories;

	public NPCNavigator Navigator => npcNavigator;
	public NPCLocationMemories Memories => memories;
	public NPCCharacter Character => npcCharacter;

	public string AssignedJob { get; set; }

	private ActorInventory Inventory
	{
		get => GetData().Inventory;
		set => GetData().Inventory = value;
	}

	// Has this NPC been initialized with data, or is it blank?
	bool hasBeenInitialized = false;

	public bool IsInPlayerDialogue { get; private set; }

	// Use this for initialization
    void Start()
    {
		scene = SceneObjectManager.GetSceneIdForObject(this.gameObject);

		npcCharacter = new NPCCharacter();
		npcNavigator = GetComponent<NPCNavigator> ();

		DialogueManager.OnInitiateDialogue += OnPlayerEnterDialogue;
		DialogueManager.OnExitDialogue += OnPlayerExitDialogue;

		// If an NPC somehow hasn't been initialized but does have an ID set
		if (!hasBeenInitialized && ActorId != null) {
			InitializeWithId(ActorId);
		}

		LoadSprites();
	}
	
    void LoadSprites () {
		NPCData spriteData = NPCDataMaster.GetNpcFromId (ActorId);
		if (spriteData != null)
		{
			string hatId = Inventory.GetEquippedHat()?.GetItemId();
			string shirtId = Inventory.GetEquippedShirt()?.GetItemId();
			string pantsId = Inventory.GetEquippedPants()?.GetItemId();
			GetComponent<HumanSpriteLoader>().LoadSprites(spriteData.RaceId, spriteData.HairId, hatId, shirtId, pantsId);
		}
		else
		{
			GetComponent<HumanSpriteLoader>().LoadSprites("human_light", null, null, null, null);
		}
	}

	void OnApparelItemEquipped (Item item)
	{
		LoadSprites();
	}

	// Sets up all the simulation scripts for this NPC; should be called whenever an NPC is created
	void InitializeNPCScripts (NPCData data) 
	{
		if (npcCharacter == null) {
			npcCharacter = new NPCCharacter();
		}
		if (npcNavigator == null) {
			npcNavigator = gameObject.AddComponent<NPCNavigator> ();
		}
		if (memories == null)
		{
			memories = new NPCLocationMemories();
		}
		if (Inventory == null) {
			Inventory = new ActorInventory();
			Inventory.SetInventory(data.Inventory);
		}
		// TODO load physical condition from data
		GetData().PhysicalCondition.Init ();
		npcCharacter.Init (data);
		Inventory.Initialize ();
		GetData().Personality = data.Personality;
		GetData().Race = data.RaceId;

		Inventory.OnHatEquipped += OnApparelItemEquipped;
		Inventory.OnShirtEquipped += OnApparelItemEquipped;
		Inventory.OnPantsEquipped += OnApparelItemEquipped;
	}
	void OnPlayerEnterDialogue (NPC npc, DialogueDataMaster.DialogueNode startNode) {
		if (npc == this) {
			IsInPlayerDialogue = true;
		}
	}
	void OnPlayerExitDialogue () {
		IsInPlayerDialogue = false;
	}

    public void InitializeWithId (string id)
    {
		ActorRegistry.UnregisterActorGameObject(ActorId);

		NPCData data = NPCDataMaster.GetNpcFromId (id);
		if (data == null) {
			Debug.LogWarning ("This NPC doesn't seem to have a real ID!");
			data = new NPCData (id, "Nameless Clone", "human_light", Gender.Male);
		}
        ActorId = id;
        ActorRegistry.RegisterActorGameObject(this);

		InitializeNPCScripts (data);
		
		GetData().ActorName = data.NpcName;

		hasBeenInitialized = true;
		LoadSprites();
	}

	protected override void OnDeath ()
	{
		base.OnDeath();
		HumanSpriteController spriteController = GetComponent<HumanSpriteController>();
		if (spriteController != null)
		{
			spriteController.ForceUnconsciousSprite = true;
		}
		NPCBehaviourExecutor executor = GetComponent<NPCBehaviourExecutor>();
		if (executor != null)
		{
			executor.ForceCancelBehaviours();
		}
	}

	
	public void OnInteract () {	}
}
