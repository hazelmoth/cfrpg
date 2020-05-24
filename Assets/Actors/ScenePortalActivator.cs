using UnityEngine;

// A static class that allows the client to force any actor to traverse any scene portal
public static class ScenePortalActivator
{
	public static void Activate(Actor actor, ScenePortal portal)
    {
	    actor.MoveActorToScene(portal.DestinationSceneObjectId);
	    actor.GetComponent<ActorAnimController>().SetDirection(portal.EntryDirection);
	    Vector2 newTransform = portal.PortalExitRelativeCoords;
	    actor.transform.localPosition = newTransform;
	}
}
