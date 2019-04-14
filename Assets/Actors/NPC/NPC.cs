using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Stores the ID of this NPC and accesses the appropriate classes
// to load data based on that ID.
public class NPC : Actor, InteractableObject {

	[SerializeField] string npcId = null;
	public string NpcId {get{return npcId;}}

	NPCCharacter npcCharacter;

	// Has this NPC been initialized with data, or is it blank?
	bool hasBeenInitialized = false;

	public bool IsInPlayerDialogue { get; private set; }

	// Use this for initialization
    void Start()
    {
		actorCurrentScene = SceneObjectManager.GetSceneIdForObject(this.gameObject);

		physicalCondition = GetComponent<ActorPhysicalCondition> ();
		npcCharacter = GetComponent<NPCCharacter> ();

		DialogueManager.OnInitiateDialogue += OnPlayerEnterDialogue;
		DialogueManager.OnExitDialogue += OnPlayerExitDialogue;

		LoadSprites();
		// If an NPC somehow hasn't been initialized but does have an ID set
		if (!hasBeenInitialized && npcId != null) {
			InitializeWithId (npcId);
		}
    }
    void LoadSprites () {
		NPCData spriteData = NPCDataMaster.GetNpcFromId (npcId);
        if (spriteData != null)
        {
            GetComponent<HumanSpriteLoader>().LoadSprites(spriteData.BodySprite, spriteData.HairId, spriteData.HatId, spriteData.ShirtId, spriteData.PantsId);
        }
        else
            GetComponent<HumanSpriteLoader>().LoadSprites("human_base");
	}

	// Sets up all the simulation scripts for this NPC
	void InitializeNPCScripts (NPCData data) 
	{
		if (physicalCondition == null) {
			physicalCondition = gameObject.AddComponent<ActorPhysicalCondition> ();
		}
		if (npcCharacter == null) {
			npcCharacter = gameObject.AddComponent<NPCCharacter> ();
		}
		physicalCondition.Init ();
		npcCharacter.Init (data);
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
		hasBeenInitialized = true;

		NPCData data = NPCDataMaster.GetNpcFromId (id);
		if (data == null) {
			Debug.LogWarning ("This NPC doesn't seem to have a real ID!");
			data = new NPCData (id, "Nameless Clone", "human_base", Gender.Male);
		}
        npcId = id;
        LoadSprites();
		InitializeNPCScripts (data);
    }

    public void OnInteract () {
		
	}
}
