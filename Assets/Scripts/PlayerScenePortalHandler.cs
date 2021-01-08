using UnityEngine;

// Detects player interaction events and activates a scene portal, if necessary.
public class PlayerScenePortalHandler : MonoBehaviour
{
	private ActorScenePortalTriggerDetector currentDetector;

	private void Start () {
		PlayerInteractionManager.OnPlayerInteract += OnPlayerInteraction;
		PlayerController.OnPlayerIdSet += ResetTriggerDetector;
		if (PlayerController.PlayerActorId != null)
		{
			ResetTriggerDetector();
		}
	}

	private void OnPlayerInteraction (IInteractable interactedObject) {
		ScenePortal portal = interactedObject as ScenePortal;
		if (portal != null && portal.ActivateOnTouch == false) {
			HandlePortalActivation (portal);
		}
	}

	private void HandlePortalActivation (ScenePortal portal)
	{
		ScenePortalActivator.Activate(ActorRegistry.Get(PlayerController.PlayerActorId).actorObject, portal);
	}

	// Unregisters from the event from currentDetector and re-registers for the current player object
	private void ResetTriggerDetector()
	{
		if (currentDetector)
		{
			currentDetector.OnScenePortalTrigger -= HandlePortalActivation;
		}

		currentDetector = ActorRegistry.Get(PlayerController.PlayerActorId).actorObject
			.GetComponent<ActorScenePortalTriggerDetector>();
		currentDetector.OnScenePortalTrigger += HandlePortalActivation;
	}
}
