using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Access PlayerInteractionRaycaster to check whether an interactable object is present and
// take keyboard input to activate an interaction. Interacting with an object should trigger
// the appropriate response in UIManager, or whatever actions the item is meant to perform.
public class PlayerInteractionManager : MonoBehaviour {

	public delegate void PlayerInteractionEvent (InteractableObject activatedObject);
	public static event PlayerInteractionEvent OnPlayerInteract;
	PlayerInteractionRaycaster raycaster;

	// Use this for initialization
	void Start () {
		raycaster = GetComponent<PlayerInteractionRaycaster> ();
	}
	
	// Update is called once per frame
	void Update () {
		GameObject detectedObject = raycaster.DetectInteractableObject ();
		if (detectedObject != null) {
			// TODO: Keyboard input should be in a dedicated input manager class
			if  (Input.GetKeyDown(KeyCode.Space)) {
				InteractableObject detectedInteractable = detectedObject.GetComponent<InteractableObject> ();
				if (OnPlayerInteract != null)
					OnPlayerInteract (detectedInteractable);
				detectedInteractable.OnInteract ();
			}
		}
	}
}
