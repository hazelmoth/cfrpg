using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;

// Access PlayerInteractionRaycaster to check whether an interactable object or dropped item is
// present and take keyboard input to activate an interaction. Interacting with an object should trigger
// the appropriate response in UIManager, or whatever actions the item is meant to perform.
public class PlayerInteractionManager : MonoBehaviour {

	public delegate void PlayerInteractionEvent (InteractableObject activatedObject);
	public delegate void PlayerActorInteractionEvent(Actor Actor);
	public static event PlayerInteractionEvent OnPlayerInteract;
	public static event PlayerActorInteractionEvent OnInteractWithSettler;
	private PlayerInteractionRaycaster raycaster;
	private DroppedItemDetector itemDetector;

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
	private void Update () {
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
		Actor playerObject = ActorRegistry.Get(PlayerController.PlayerActorId)?.gameObject;
		if (playerObject != null)
		{
			itemDetector = playerObject.GetComponent<DroppedItemDetector>();
		}
		else
		{
			itemDetector = null;
		}
	}
}
