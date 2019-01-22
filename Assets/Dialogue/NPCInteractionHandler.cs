using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script does nothing but check if a player interaction is with an npc, and if so calls an event.
public class NPCInteractionHandler : MonoBehaviour {
	public delegate void NpcInteractionEvent (NPC npc);
	public static event NpcInteractionEvent OnInteractWithNpc;

	// Use this for initialization
	void Start () {
		PlayerInteractionManager.OnPlayerInteract += OnPlayerInteract;
	}

	void OnPlayerInteract (InteractableObject interactable) {
		NPC npc = interactable as NPC;
		if (npc != null && !DialogueManager.IsInDialogue) {
			if (OnInteractWithNpc != null) {
				OnInteractWithNpc (npc);
			}
		} 
	}
}
