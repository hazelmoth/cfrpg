using System;
using UnityEngine;

// Detects when this actor collides with a scene portal trigger
public class ActorScenePortalTriggerDetector : MonoBehaviour
{
	public event Action<ScenePortal> OnScenePortalTrigger;


    // Handle portals that are activate by trigger colliders
    private void OnTriggerEnter2D(Collider2D collider)
    {
	    ScenePortal portal = collider.GetComponent<ScenePortal>();
	    if (portal != null && portal.ActivateOnTouch == true)
	    {
		    OnScenePortalTrigger?.Invoke(portal);
	    }
    }
}
