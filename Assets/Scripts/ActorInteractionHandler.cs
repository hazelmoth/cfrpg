using Dialogue;
using UnityEngine;

/// This script does nothing but check if a player interaction is with an Actor, and if so
/// calls an event. Is it's existence justified? I sincerely doubt it.
public class ActorInteractionHandler : MonoBehaviour {
	public delegate void ActorInteractionEvent (Actor actor);
	public static event ActorInteractionEvent OnInteractWithActor;

	private void OnDestroy ()
	{
		OnInteractWithActor = null;
	}
	// Use this for initialization
	private void Start () {
		PlayerInteractionManager.OnPlayerInteract += OnPlayerInteract;
	}

	private static void OnPlayerInteract (IInteractable interactable) {
		Actor actor = interactable as Actor;
		if (actor == null) return;
		OnInteractWithActor?.Invoke (actor);
	}
}
