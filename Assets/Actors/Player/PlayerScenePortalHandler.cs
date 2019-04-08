using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScenePortalHandler : MonoBehaviour
{
	void Start () {
		PlayerInteractionManager.OnPlayerInteract += OnPlayerInteraction;
	}
	void OnPlayerInteraction (InteractableObject interactedObject) {
		ScenePortal portal = interactedObject as ScenePortal;
		if (portal != null && portal.ActivateOnTouch == false) {
			HandlePortalActivation (portal);
		}
	}
	// Handle portals that are activate by trigger colliders
	private void OnTriggerEnter2D(Collider2D collider)
	{
		ScenePortal portal = collider.GetComponent<ScenePortal> ();
		if (portal != null && portal.ActivateOnTouch == true)
		{
			HandlePortalActivation (portal);
		}
	}

	public void HandlePortalActivation (ScenePortal portal) {
		Player.instance.MoveActorToScene (portal.DestinationSceneObjectId);
		Player.instance.GetComponent<HumanAnimController> ().SetDirection (portal.EntryDirection);
		Vector2 newTransform = portal.PortalExitRelativeCoords;
		Player.instance.transform.localPosition = newTransform;
	}


}
