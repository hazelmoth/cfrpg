using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Stores the ID of this NPC and accesses the appropriate classes
// to load data based on that ID.
public class NPC : Actor, InteractableObject {

	[SerializeField] string npcId = null;

	public string NpcId {get{return npcId;}}


	// Use this for initialization
    void Start()
    {
        LoadSprites();
        actorCurrentScene = this.gameObject.scene.name;
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

    public void SetId (string id)
    {
        npcId = id;
        LoadSprites();
    }

    public void OnInteract () {
		
	}
}
