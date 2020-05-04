﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;

// Access PlayerInteractionRaycaster to check whether an interactable object or dropped item is
// present and take keyboard input to activate an interaction. Interacting with an object should trigger
// the appropriate response in UIManager, or whatever actions the item is meant to perform.
public class PlayerInteractionManager : MonoBehaviour {

	public delegate void PlayerInteractionEvent (InteractableObject activatedObject);
	public delegate void PlayerNpcInteractionEvent(NPC npc);
	public static event PlayerInteractionEvent OnPlayerInteract;
	public static event PlayerNpcInteractionEvent OnInteractWithSettler;
	PlayerInteractionRaycaster raycaster;
	DroppedItemDetector itemDetector;

	void OnDestroy ()
	{
		OnPlayerInteract = null;
		OnInteractWithSettler = null;
	}

	// Use this for initialization
	void Start () {
		raycaster = GetComponent<PlayerInteractionRaycaster> ();
		itemDetector = GetComponent<DroppedItemDetector> ();
	}
	
	// Update is called once per frame
	void Update () {
		DroppedItem detectedItem = itemDetector.GetCurrentDetectedItem ();
		GameObject detectedObject = raycaster.DetectInteractableObject ();

		// Items take priority over entities
		if (detectedItem != null) {
			if (Input.GetKeyDown(KeyCode.E)) {
				DroppedItemPickupManager.AttemptPickup (ActorRegistry.Get(PlayerController.PlayerActorId).gameObject, detectedItem);
			}
		}
		else if (detectedObject != null) {
			if  (Input.GetKeyDown(KeyCode.E)) {
				InteractableObject detectedInteractable = detectedObject.GetComponent<InteractableObject> ();
				OnPlayerInteract?.Invoke(detectedInteractable);
				detectedInteractable.OnInteract ();
			} 
			else if (Input.GetKeyDown(KeyCode.F))
			{
				NPC detectedNpc = detectedObject.GetComponent<NPC>();
				if (detectedNpc != null)
				{
					// Only allow task delegation if this NPC is in the player's settlement
					if (detectedNpc.GetData().FactionStatus.FactionId != null && detectedNpc.GetData().FactionStatus.FactionId == ActorRegistry.Get(PlayerController.PlayerActorId).data.FactionStatus.FactionId)
						OnInteractWithSettler?.Invoke(detectedNpc);
				}
			}
		}
	}
}
