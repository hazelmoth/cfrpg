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

	private void Update()
	{
		if (currentBehaviour != null && npc.IsDead)
		{
			ForceCancelBehaviours();
		}
	}
	public void ForceCancelBehaviours ()
	{
		if (currentBehaviour != null)
		{
			Debug.Log("Cancelling behaviour.");
			currentBehaviour.Cancel();
			currentBehaviour = null;
		}
	}

	// Do nothing
	public void Execute_StandStill ()
	{
		if (CurrentActivity == NPCBehaviourAI.Activity.None && currentBehaviour != null && currentBehaviour.IsRunning)
		{
			return;
		}

		currentBehaviour.Cancel();
		currentBehaviour = null;

		CurrentActivity = NPCBehaviourAI.Activity.None;
	}
	public void Execute_EatSomething()
	{
		if (CurrentActivity == NPCBehaviourAI.Activity.Eat && currentBehaviour != null && currentBehaviour.IsRunning)
		{ 
			return;
		}
		Debug.Log(CurrentActivity);
		Debug.Log(currentBehaviour);
		//Debug.Log(currentBehaviour.IsRunning);

		currentBehaviour?.Cancel();
		currentBehaviour = new EatSomethingBehaviour(npc, null);
		currentBehaviour.Execute();

		CurrentActivity = NPCBehaviourAI.Activity.Eat;
	}

	// Look around for fruit
	public void Execute_ScavengeForFood ()
	{
		if (CurrentActivity == NPCBehaviourAI.Activity.ScavengeForFood && currentBehaviour != null && currentBehaviour.IsRunning)
		{
			return;
		}

		currentBehaviour?.Cancel();
		currentBehaviour = new ScavengeForFoodBehaviour(npc);
		currentBehaviour.Execute();

		CurrentActivity = NPCBehaviourAI.Activity.ScavengeForFood;
	}

	// Find a tree and cut it
	public void Execute_ScavengeForWood ()
	{
		if (CurrentActivity == NPCBehaviourAI.Activity.ScavengeForWood && currentBehaviour != null && currentBehaviour.IsRunning)
		{
			return;
		}

		currentBehaviour?.Cancel();
		currentBehaviour = new ScavengeForWoodBehaviour(npc);
		currentBehaviour.Execute();

		CurrentActivity = NPCBehaviourAI.Activity.ScavengeForWood;
	}

	public void Execute_StashWood ()
	{
		if (CurrentActivity == NPCBehaviourAI.Activity.StashWood && currentBehaviour != null && currentBehaviour.IsRunning)
		{
			return;
		}

		currentBehaviour?.Cancel();
		currentBehaviour = new StashWoodBehaviour(npc, null);
		currentBehaviour.Execute();

		CurrentActivity = NPCBehaviourAI.Activity.StashWood;
	}

	// Aimlessly move about
	public void Execute_Wander ()
	{
		if (CurrentActivity == NPCBehaviourAI.Activity.Wander && currentBehaviour != null && currentBehaviour.IsRunning)
		{
			return;
		}

		currentBehaviour?.Cancel();
		currentBehaviour = new WanderBehaviour(npc);
		currentBehaviour.Execute();

		CurrentActivity = NPCBehaviourAI.Activity.Wander;
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