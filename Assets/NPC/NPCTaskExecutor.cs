using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Contains functions that force an NPC to start or stop different tasks.
// These functions should be called by NPCScheduleFollower.
public class NPCTaskExecutor : MonoBehaviour {

	NPCNavigator nav;
	NPC npc;
	bool isWaitingForNavigationToFinish = false;

	// Use this for initialization
	void Awake () {
		npc = this.GetComponent<NPC> ();
		nav = this.GetComponent<NPCNavigator> ();
		if (nav == null)
			Debug.LogError ("This NPC seems to be missing an NPCNavigation component:", this.gameObject);

		nav.NavigationCompleted += OnNavigationFinished;
	}

		
	// Aimlessly move about
	public void Wander () {
		StopAllCoroutines ();
		StartCoroutine (WanderCoroutine()); 
	}
	// Do nothing
	public void StandStill () {
		StopAllCoroutines ();
		// TODO a way to force the nav controller to stop
	}

	IEnumerator WanderCoroutine () {
		while (true) {
            // Walk to a random nearby tile
			nav.FollowPath (TileNavigationHelper.FindPath (
				TilemapInterface.WorldPosToScenePos(transform.position, npc.ActorCurrentScene), 
				TileNavigationHelper.FindRandomNearbyPathTile (TilemapInterface.WorldPosToScenePos(transform.position, npc.ActorCurrentScene), 20, npc.ActorCurrentScene), 
				npc.ActorCurrentScene
			), npc.ActorCurrentScene);
			isWaitingForNavigationToFinish = true;
			while (isWaitingForNavigationToFinish) {
				yield return null;
			}
			// Pause for a bit before walking again
			yield return new WaitForSeconds (Random.Range (1f, 5f));
		}
	}
	// Travel from one place to another, including across scenes
	IEnumerator TravelCoroutine (TileLocation destination) {
		
		if (destination.Scene != this.GetComponent<NPC>().ActorCurrentScene) {
			// Find a portal to traverse scenes
			// TODO not have every NPC use the same portal every time
			ScenePortal targetPortal = ScenePortalLibrary.GetPortalsBetweenScenes (this.GetComponent<NPC>().ActorCurrentScene, destination.Scene)[0];
			if (targetPortal == null) {
				Debug.LogWarning ("Cross-scene navigation failed; no suitable scene portal exists!");
				StopCoroutine (TravelCoroutine(destination));
			}
			Vector2 targetLocation = TileNavigationHelper.GetValidAdjacentTiles (npc.ActorCurrentScene, TilemapInterface.WorldPosToScenePos(targetPortal.transform.position, targetPortal.gameObject.scene.name))[0];
			nav.FollowPath (TileNavigationHelper.FindPath (transform.localPosition, targetLocation, npc.ActorCurrentScene), npc.ActorCurrentScene);
			isWaitingForNavigationToFinish = true;
			while (isWaitingForNavigationToFinish) {
				yield return null;
			}
			// Turn towards scene portal
			nav.ForceDirection(TileNavigationHelper.GetDirectionToLocation(transform.position, targetPortal.transform.position));
			// Pause for a sec
			yield return new WaitForSeconds(0.3f);
			// Activate portal
			ActivateScenePortal (targetPortal);
			// Finish navigation
			nav.FollowPath (TileNavigationHelper.FindPath (
				TilemapInterface.WorldPosToScenePos(transform.position, npc.ActorCurrentScene), 
				destination.Position,
				npc.ActorCurrentScene
			), npc.ActorCurrentScene);
			isWaitingForNavigationToFinish = true;
			while (isWaitingForNavigationToFinish) {
				yield return null;
			}

		} else {
			// Destination is on same scene
			nav.FollowPath (TileNavigationHelper.FindPath (TilemapInterface.WorldPosToScenePos(transform.position, npc.ActorCurrentScene), new Vector2 (destination.x, destination.y), npc.ActorCurrentScene), npc.ActorCurrentScene);
			isWaitingForNavigationToFinish = true;
			while (isWaitingForNavigationToFinish) {
				yield return null;
			}
		}
	}

	void ActivateScenePortal (ScenePortal portal) {
		npc.MoveActorToScene (portal.DestinationScene);
		npc.GetComponent<NPCNavigator> ().ForceDirection (portal.ExitDirection);
		Vector2 newTransform = portal.SceneEntryRelativeCoords;
		// Offset the transform so the player is in the center of the tile
		newTransform.x += Mathf.Sign (newTransform.x) * HumanAnimController.HumanTileOffset.x;
		newTransform.y += Mathf.Sign (newTransform.y) * HumanAnimController.HumanTileOffset.y;
		npc.transform.localPosition = newTransform;
	}

	void OnNavigationFinished () {
		isWaitingForNavigationToFinish = false;
	}
}
