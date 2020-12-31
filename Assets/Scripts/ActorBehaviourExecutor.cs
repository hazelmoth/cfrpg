using System.Collections.Generic;
using UnityEngine;

// Contains functions that force an Actor to start or stop different tasks.
// These functions should be called by ActorBehaviourAi.
public class ActorBehaviourExecutor : MonoBehaviour {

	public delegate void ExecutionCallback ();
	public delegate void ExecutionCallbackFailable(bool didSucceed);
	public delegate void ExecutionCallbackDroppedItemsFailable(bool didSucceed, List<DroppedItem> items);

	private const float VisualSearchRadius = 20f;

	private ActorNavigator nav;
	private Actor Actor;
	private ActorPunchExecutor puncher;

	private IAiBehaviour currentBehaviour;

	public ActorBehaviourAi.Activity CurrentActivity { get; private set;}

	private void Awake () {
		Actor = this.GetComponent<Actor> ();
		nav = this.GetComponent<ActorNavigator> ();
		puncher = this.GetComponent<ActorPunchExecutor>();
		if (nav == null)
			Debug.LogError ("This Actor seems to be missing an ActorNavigation component:", this.gameObject);
	}

	private void Update()
	{
		if (currentBehaviour != null && Actor.GetData().PhysicalCondition.IsDead)
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
		    ((FollowBehaviour) currentBehaviour).target.ActorId == Actor.GetData().FactionStatus.AccompanyTarget)
		{
			return;
		}

		currentBehaviour?.Cancel();
		Actor target = ActorRegistry.Get(Actor.GetData().FactionStatus.AccompanyTarget).actorObject;
		currentBehaviour = new FollowBehaviour(Actor, target);
		currentBehaviour.Execute();

		CurrentActivity = ActorBehaviourAi.Activity.Accompany;
	}

	public void Execute_ChillAtHome()
	{
		if (AlreadyRunning(ActorBehaviourAi.Activity.ChillAtHome))
		{
			return;
		}
		currentBehaviour?.Cancel();
		currentBehaviour = new ChillAtHomeBehaviour(Actor);
		currentBehaviour.Execute();

		CurrentActivity = ActorBehaviourAi.Activity.ChillAtHome;
	}

	// Do nothing
	public void Execute_StandStill ()
	{
		if (CurrentActivity == ActorBehaviourAi.Activity.None && currentBehaviour != null && currentBehaviour.IsRunning)
		{
			return;
		}

		currentBehaviour?.Cancel();
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
		currentBehaviour = new EatSomethingBehaviour(Actor, null);
		currentBehaviour.Execute();

		CurrentActivity = ActorBehaviourAi.Activity.Eat;
	}

	public void Execute_Settler()
	{
		if (CurrentActivity == ActorBehaviourAi.Activity.Settler && currentBehaviour != null && currentBehaviour.IsRunning)
		{
			return;
		}

		currentBehaviour?.Cancel();
		currentBehaviour = new SettlerBehaviour(Actor);
		currentBehaviour.Execute();

		CurrentActivity = ActorBehaviourAi.Activity.Settler;
	}

	// Look around for fruit
	public void Execute_ScavengeForFood ()
	{
		if (CurrentActivity == ActorBehaviourAi.Activity.ScavengeForFood && currentBehaviour != null && currentBehaviour.IsRunning)
		{
			return;
		}

		currentBehaviour?.Cancel();
		currentBehaviour = new ScavengeForFoodBehaviour(Actor);
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
		currentBehaviour = new ScavengeForWoodBehaviour(Actor);
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
		currentBehaviour = new StashWoodBehaviour(Actor, null);
		currentBehaviour.Execute();

		CurrentActivity = ActorBehaviourAi.Activity.StashWood;
	}

	public void Execute_Trade()
	{
		if (CurrentActivity == ActorBehaviourAi.Activity.Trade && currentBehaviour != null && currentBehaviour.IsRunning)
		{
			return;
		}

		currentBehaviour?.Cancel();
		currentBehaviour = new TraderBehaviour(Actor);
		currentBehaviour.Execute();

		CurrentActivity = ActorBehaviourAi.Activity.Trade;
	}

	// Aimlessly move about
	public void Execute_Wander ()
	{
		if (CurrentActivity == ActorBehaviourAi.Activity.Wander && currentBehaviour != null && currentBehaviour.IsRunning)
		{
			return;
		}

		currentBehaviour?.Cancel();
		currentBehaviour = new WanderBehaviour(Actor);
		currentBehaviour.Execute();

		CurrentActivity = ActorBehaviourAi.Activity.Wander;
	}


	public void ActivateScenePortal (ScenePortal portal) {
		Actor.MoveActorToScene (portal.DestinationSceneObjectId);
		Actor.GetComponent<ActorNavigator> ().ForceDirection (portal.EntryDirection);
		Vector2 newTransform = portal.PortalExitRelativeCoords;
		// Offset the transform so the player is in the center of the tile
		newTransform.x += Mathf.Sign (newTransform.x) * ActorAnimController.HumanTileOffset.x;
		newTransform.y += Mathf.Sign (newTransform.y) * ActorAnimController.HumanTileOffset.y;
		Actor.transform.localPosition = newTransform;
	}

	private bool AlreadyRunning(ActorBehaviourAi.Activity activity)
	{
		return CurrentActivity == activity && currentBehaviour != null &&
		       currentBehaviour.IsRunning;
	}
}