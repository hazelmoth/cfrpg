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
		Player.instance.MoveActorToScene (portal.DestinationScene);
		Player.instance.GetComponent<PlayerAnimController> ().SetDirection (portal.ExitDirection);
		Vector2 newTransform = portal.SceneEntryRelativeCoords;
		// Offset the transform so the player is in the center of the tile
		newTransform.x += Mathf.Sign (newTransform.x) * HumanAnimController.HumanTileOffset.x;
		newTransform.y += Mathf.Sign (newTransform.y) * HumanAnimController.HumanTileOffset.y;
		Player.instance.transform.localPosition = newTransform;
	}


}
