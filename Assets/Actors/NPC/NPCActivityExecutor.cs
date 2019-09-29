using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Contains functions that force an NPC to start or stop different tasks.
// These functions should be called by NPCBehaviourAI.
public class NPCActivityExecutor : MonoBehaviour {

	public delegate void ExecutionCallback ();
	public delegate void ExecutionCallbackFailable(bool didSucceed);
	public delegate void ExecutionCallbackDroppedItemsFailable(bool didSucceed, List<DroppedItem> items);

	const float visualSearchRadius = 20f;

	NPCNavigator nav;
	NPC npc;
	ActorPunchExecutor puncher;

	IAiBehaviour currentBehaviour;

	public NPCBehaviourAI.Activity CurrentActivity { get; private set;}

	void Awake () {
		npc = this.GetComponent<NPC> ();
		nav = this.GetComponent<NPCNavigator> ();
		puncher = this.GetComponent<ActorPunchExecutor>();
		if (nav == null)
			Debug.LogError ("This NPC seems to be missing an NPCNavigation component:", this.gameObject);
	}
		

	// Do nothing
	public void Execute_StandStill () {
		if (CurrentActivity == NPCBehaviourAI.Activity.None)
			return;
		StopAllCoroutines ();
		CurrentActivity = NPCBehaviourAI.Activity.None;
		nav.CancelNavigation ();
	}

	public void Execute_EatSomething ()
	{
		currentBehaviour?.Cancel();
		currentBehaviour = new EatSomethingBehaviour(npc, null);
		currentBehaviour.Execute();
	}

	// Look around for fruit
	public void Execute_ScavengeForFood () {
		currentBehaviour?.Cancel();
		currentBehaviour = new ScavengeForFoodBehaviour(npc);
		currentBehaviour.Execute();
	}

	// Find a tree and cut it
	public void Execute_ScavengeForWood ()
	{
		currentBehaviour?.Cancel();
		currentBehaviour = new ScavengeForWoodBehaviour(npc);
		currentBehaviour.Execute();
	}

	public void Execute_StashWood ()
	{
		currentBehaviour?.Cancel();
		currentBehaviour = new StashWoodBehaviour(npc, null);
		currentBehaviour.Execute();
	}

	// Aimlessly move about
	public void Execute_Wander () {
		currentBehaviour?.Cancel();
		currentBehaviour = new WanderBehaviour(npc);
		currentBehaviour.Execute();
	}


	public void ActivateScenePortal (ScenePortal portal) {
		npc.MoveActorToScene (portal.DestinationSceneObjectId);
		npc.GetComponent<NPCNavigator> ().ForceDirection (portal.EntryDirection);
		Vector2 newTransform = portal.PortalExitRelativeCoords;
		// Offset the transform so the player is in the center of the tile
		newTransform.x += Mathf.Sign (newTransform.x) * HumanAnimController.HumanTileOffset.x;
		newTransform.y += Mathf.Sign (newTransform.y) * HumanAnimController.HumanTileOffset.y;
		npc.transform.localPosition = newTransform;
	}
}