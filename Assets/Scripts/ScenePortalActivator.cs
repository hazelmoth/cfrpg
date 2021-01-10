using UnityEngine;

// A static class that allows the client to force any actor to traverse any scene portal
public static class ScenePortalActivator
{
	public static void Activate(Actor actor, ScenePortal portal)
    {
		if (string.IsNullOrEmpty(portal.DestinationSceneObjectId))
		{
			Debug.LogError("Tried to activate a scene portal that doesn't link to any scene!");
			return;
		}
	    actor.MoveActorToScene(portal.DestinationSceneObjectId);
	    actor.GetComponent<ActorAnimController>().SetDirection(portal.EntryDirection);
	    Vector2 newTransform = portal.PortalExitRelativeCoords;
	    actor.transform.localPosition = newTransform;
	}
}
