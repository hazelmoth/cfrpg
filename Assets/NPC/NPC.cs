using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Stores the ID of this NPC and accesses the appropriate classes
// to load data based on that ID.
public class NPC : Actor, InteractableObject {

	[SerializeField] string npcId;

	public string NpcId {get{return npcId;}}


	// Use this for initialization
	void Start () {
		NPCData spriteData = NPCDataMaster.GetNpcFromId (npcId);
		if (spriteData != null)
			GetComponent<NPCSpriteLoader> ().LoadSprites (spriteData.BodySprite, spriteData.HatId, spriteData.ShirtId, spriteData.PantsId);
		else
			GetComponent<NPCSpriteLoader> ().LoadSprites ("human_base");
		
		actorCurrentScene = this.gameObject.scene.name;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnInteract () {
		
	}
}
