using System;
using UnityEngine;

// Detects player interaction events and activates a scene portal, if necessary.
public class PlayerScenePortalHandler : MonoBehaviour
{
	private void Start () {
		PlayerInteractionManager.OnPlayerInteract += OnPlayerInteraction;
	}

	private void OnPlayerInteraction (IInteractable interactedObject) {
		ScenePortal portal = interactedObject as ScenePortal;
		if (portal != null && portal.ActivateOnTouch == false) {
			HandlePortalActivation (portal);
		}
	}

	public void HandlePortalActivation (ScenePortal portal)
	{
		ScenePortalActivator.Activate(ActorRegistry.Get(PlayerController.PlayerActorId).actorObject, portal);
	}
}
