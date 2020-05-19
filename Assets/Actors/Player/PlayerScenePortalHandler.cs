﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScenePortalHandler : MonoBehaviour
{
	private void Start () {
		PlayerInteractionManager.OnPlayerInteract += OnPlayerInteraction;
	}

	private void OnPlayerInteraction (InteractableObject interactedObject) {
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
		ActorRegistry.Get(PlayerController.PlayerActorId).actorObject.MoveActorToScene (portal.DestinationSceneObjectId);
		ActorRegistry.Get(PlayerController.PlayerActorId).actorObject.GetComponent<ActorAnimController> ().SetDirection (portal.EntryDirection);
		Vector2 newTransform = portal.PortalExitRelativeCoords;
		ActorRegistry.Get(PlayerController.PlayerActorId).actorObject.transform.localPosition = newTransform;
	}
}
