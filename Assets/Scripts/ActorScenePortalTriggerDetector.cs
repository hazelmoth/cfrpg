using System;
using UnityEngine;

// Detects when this actor collides with a scene portal trigger
public class ActorScenePortalTriggerDetector : MonoBehaviour
{
	private PlayerScenePortalHandler playerPortalHandler;

	private void Start()
	{
		playerPortalHandler = GameObject.FindObjectOfType<PlayerScenePortalHandler>();
		Debug.Assert(playerPortalHandler != null, "Player scene portal handler not found in scene.");
	}


	// Handle portals that are activate by trigger colliders
    private void OnTriggerEnter2D(Collider2D col)
    {
	    ScenePortal portal = col.GetComponent<ScenePortal>();
	    if (portal != null && portal.ActivateOnTouch)
	    {
		    // Only the player actor responds to touch-trigger portals
		    if (!GetComponent<Actor>().PlayerControlled) return;
		    
			playerPortalHandler.HandlePortalActivation(portal);
	    }
    }
}
