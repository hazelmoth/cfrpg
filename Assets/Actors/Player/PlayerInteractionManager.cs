using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;

// Accesses PlayerInteractionRaycaster to check whether an interactable object or dropped item is
// present and take keyboard input to activate an interaction. Interacting with an object triggers
// the appropriate response in UIManager, or whatever actions the item is meant to perform.
public class PlayerInteractionManager : MonoBehaviour {

	public delegate void PlayerInteractionEvent (InteractableObject activatedObject);
	public delegate void PlayerActorInteractionEvent(Actor Actor);
	public static event PlayerInteractionEvent OnPlayerInteract;
	public static event PlayerActorInteractionEvent OnInteractWithSettler;
	private PlayerInteractionRaycaster raycaster;
	private PickupDetector itemDetector;

	private void OnDestroy ()
	{
		OnPlayerInteract = null;
		OnInteractWithSettler = null;
	}

	// Use this for initialization
	private void Start ()
	{
		raycaster = FindObjectOfType<PlayerInteractionRaycaster>();
		SetComponents();
		PlayerController.OnPlayerIdSet += SetComponents;
	}
	
	// Update is called once per frame
	private void Update () 
	{
		if (PauseManager.GameIsPaused)
		{
			return;
		}

		IPickuppable detectedItem = itemDetector.GetCurrentDetectedItem ();
		GameObject detectedObject = raycaster.DetectInteractableObject ();

		// Items take priority over entities
		if (detectedItem != null) {
			if (Input.GetKeyDown(KeyCode.E)) {
				PickupSystem.AttemptPickup (ActorRegistry.Get(PlayerController.PlayerActorId).actorObject, detectedItem);
			}
		}
		else if (detectedObject != null) {
			if  (Input.GetKeyDown(KeyCode.E)) {
				InteractableObject detectedInteractable = detectedObject.GetComponent<InteractableObject> ();
				OnPlayerInteract?.Invoke(detectedInteractable);
				detectedInteractable.OnInteract ();

				// Message the player's inventory that there may be an active container
				ActorRegistry.Get(PlayerController.PlayerActorId).data.Inventory.OnInteractWithContainer(detectedInteractable);
			} 
			else if (Input.GetKeyDown(KeyCode.F))
			{
				Actor detectedActor = detectedObject.GetComponent<Actor>();
				if (detectedActor != null)
				{
					// Only allow task delegation if this Actor is in the player's settlement
					if (detectedActor.GetData().FactionStatus.FactionId != null && detectedActor.GetData().FactionStatus.FactionId == ActorRegistry.Get(PlayerController.PlayerActorId).data.FactionStatus.FactionId)
						OnInteractWithSettler?.Invoke(detectedActor);
				}
			}
		}
	}

	private void SetComponents()
	{
		Actor playerObject = ActorRegistry.Get(PlayerController.PlayerActorId)?.actorObject;
		if (playerObject != null)
		{
			itemDetector = playerObject.GetComponent<PickupDetector>();
		}
		else
		{
			itemDetector = null;
		}
	}
}
