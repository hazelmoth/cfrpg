using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Access PlayerInteractionRaycaster to check whether an interactable object or dropped item is
// present and take keyboard input to activate an interaction. Interacting with an object should trigger
// the appropriate response in UIManager, or whatever actions the item is meant to perform.
public class PlayerInteractionManager : MonoBehaviour {

	public delegate void PlayerInteractionEvent (InteractableObject activatedObject);
	public static event PlayerInteractionEvent OnPlayerInteract;
	PlayerInteractionRaycaster raycaster;
	DroppedItemDetector itemDetector;

	void OnDestroy ()
	{
		OnPlayerInteract = null;
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
				DroppedItemPickupManager.AttemptPickup (Player.instance, detectedItem);
			}
		}
		else if (detectedObject != null) {
			// TODO: Keyboard input should be in a dedicated input manager class
			if  (Input.GetKeyDown(KeyCode.E)) {
				InteractableObject detectedInteractable = detectedObject.GetComponent<InteractableObject> ();
				if (OnPlayerInteract != null)
					OnPlayerInteract (detectedInteractable);
				detectedInteractable.OnInteract ();
			}
		}
	}
}
