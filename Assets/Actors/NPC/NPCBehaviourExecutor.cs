using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Contains functions that force an NPC to start or stop different tasks.
// These functions should be called by ActorBehaviourAi.
public class NPCBehaviourExecutor : MonoBehaviour {

	public delegate void ExecutionCallback ();
	public delegate void ExecutionCallbackFailable(bool didSucceed);
	public delegate void ExecutionCallbackDroppedItemsFailable(bool didSucceed, List<DroppedItem> items);

	const float VisualSearchRadius = 20f;

	NPCNavigator nav;
	NPC npc;
	ActorPunchExecutor puncher;

	IAiBehaviour currentBehaviour;

	public ActorBehaviourAi.Activity CurrentActivity { get; private set;}

	void Awake () {
		npc = this.GetComponent<NPC> ();
		nav = this.GetComponent<NPCNavigator> ();
		puncher = this.GetComponent<ActorPunchExecutor>();
		if (nav == null)
			Debug.LogError ("This NPC seems to be missing an NPCNavigation component:", this.gameObject);
	}

	private void Update()
	{
		if (currentBehaviour != null && npc.GetData().PhysicalCondition.IsDead)
		{
			ForceCancelBehaviours();
		}
	}
	public void ForceCancelBehaviours ()
	{
		if (currentBehaviour != null)
		{
			currentBehaviour.Cancel();
			currentBehaviour = null;
		}
	}

	public void Execute_Accompany()
	{
		if (AlreadyRunning(ActorBehaviourAi.Activity.Accompany) &&
		    ((CompanionBehaviour) currentBehaviour).target.ActorId == npc.GetData().FactionStatus.AccompanyTarget)
		{
			return;
		}

		Debug.Log("Behaviour executing");

		currentBehaviour.Cancel();
		Actor target = ActorRegistry.Get(npc.GetData().FactionStatus.AccompanyTarget).gameObject;
		currentBehaviour = new CompanionBehaviour(npc, target);
		currentBehaviour.Execute();

		CurrentActivity = ActorBehaviourAi.Activity.Accompany;
	}

	// Do nothing
	public void Execute_StandStill ()
	{
		if (CurrentActivity == ActorBehaviourAi.Activity.None && currentBehaviour != null && currentBehaviour.IsRunning)
		{
			return;
		}

		currentBehaviour.Cancel();
		currentBehaviour = null;

		CurrentActivity = ActorBehaviourAi.Activity.None;
	}

	public void Execute_EatSomething()
	{
		if (CurrentActivity == ActorBehaviourAi.Activity.Eat && currentBehaviour != null && currentBehaviour.IsRunning)
		{ 
			return;
		}

		currentBehaviour?.Cancel();
		currentBehaviour = new EatSomethingBehaviour(npc, null);
		currentBehaviour.Execute();

		CurrentActivity = ActorBehaviourAi.Activity.Eat;
	}

	// Look around for fruit
	public void Execute_ScavengeForFood ()
	{
		if (CurrentActivity == ActorBehaviourAi.Activity.ScavengeForFood && currentBehaviour != null && currentBehaviour.IsRunning)
		{
			return;
		}

		currentBehaviour?.Cancel();
		currentBehaviour = new ScavengeForFoodBehaviour(npc);
		currentBehaviour.Execute();

		CurrentActivity = ActorBehaviourAi.Activity.ScavengeForFood;
	}

	// Find a tree and cut it
	public void Execute_ScavengeForWood ()
	{
		if (CurrentActivity == ActorBehaviourAi.Activity.ScavengeForWood && currentBehaviour != null && currentBehaviour.IsRunning)
		{
			return;
		}

		currentBehaviour?.Cancel();
		currentBehaviour = new ScavengeForWoodBehaviour(npc);
		currentBehaviour.Execute();

		CurrentActivity = ActorBehaviourAi.Activity.ScavengeForWood;
	}

	public void Execute_StashWood ()
	{
		if (CurrentActivity == ActorBehaviourAi.Activity.StashWood && currentBehaviour != null && currentBehaviour.IsRunning)
		{
			return;
		}

		currentBehaviour?.Cancel();
		currentBehaviour = new StashWoodBehaviour(npc, null);
		currentBehaviour.Execute();

		CurrentActivity = ActorBehaviourAi.Activity.StashWood;
	}

	// Aimlessly move about
	public void Execute_Wander ()
	{
		if (CurrentActivity == ActorBehaviourAi.Activity.Wander && currentBehaviour != null && currentBehaviour.IsRunning)
		{
			return;
		}

		currentBehaviour?.Cancel();
		currentBehaviour = new WanderBehaviour(npc);
		currentBehaviour.Execute();

		CurrentActivity = ActorBehaviourAi.Activity.Wander;
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

	private bool AlreadyRunning(ActorBehaviourAi.Activity activity)
	{
		return CurrentActivity == activity && currentBehaviour != null &&
		       currentBehaviour.IsRunning;
	}
}