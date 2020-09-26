using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script does nothing but check if a player interaction is with an Actor, and if so calls an event.
public class ActorInteractionHandler : MonoBehaviour {
	public delegate void ActorInteractionEvent (Actor Actor);
	public static event ActorInteractionEvent OnInteractWithActor;

	private void OnDestroy ()
	{
		OnInteractWithActor = null;
	}
	// Use this for initialization
	private void Start () {
		PlayerInteractionManager.OnPlayerInteract += OnPlayerInteract;
	}

	private void OnPlayerInteract (IInteractableObject interactable) {
		Actor Actor = interactable as Actor;
		if (Actor != null && !DialogueManager.IsInDialogue) {
			if (OnInteractWithActor != null) {
				OnInteractWithActor (Actor);
			}
		} 
	}
}
