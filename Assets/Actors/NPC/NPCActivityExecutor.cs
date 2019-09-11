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
	bool isWaitingForNavigationToFinish = false;

	public NPCBehaviourAI.Activity CurrentActivity { get; private set;}

	void Awake () {
		npc = this.GetComponent<NPC> ();
		nav = this.GetComponent<NPCNavigator> ();
		puncher = this.GetComponent<ActorPunchExecutor>();
		if (nav == null)
			Debug.LogError ("This NPC seems to be missing an NPCNavigation component:", this.gameObject);

		nav.NavigationCompleted += OnNavigationFinished;
	}
		

	// Do nothing
	public void Execute_StandStill () {
		if (CurrentActivity == NPCBehaviourAI.Activity.None)
			return;
		StopAllCoroutines ();
		CurrentActivity = NPCBehaviourAI.Activity.None;
		nav.CancelNavigation ();
	}


	public void Execute_EatSomething () {
		StartCoroutine(EatSomethingCoroutine(null));
	}

	// Look around for fruit
	public void Execute_ScavengeForFood () {
		if (CurrentActivity == NPCBehaviourAI.Activity.ScavengeForFood)
			return;
		CurrentActivity = NPCBehaviourAI.Activity.ScavengeForFood;
		StopAllCoroutines ();
		StartCoroutine (ScavengeForFoodCoroutine ());
	}

	// Find a tree and cut it
	public void Execute_ScavengeForWood ()
	{
		if (CurrentActivity == NPCBehaviourAI.Activity.ScavengeForWood)
			return;
		CurrentActivity = NPCBehaviourAI.Activity.ScavengeForWood;
		StopAllCoroutines();
		StartCoroutine(ScavengeForWoodCoroutine());
	}


	public void Execute_StashWood ()
	{
		if (CurrentActivity == NPCBehaviourAI.Activity.StashWood)
			return;
		CurrentActivity = NPCBehaviourAI.Activity.StashWood;
		StopAllCoroutines();
		StartCoroutine(StashWoodCoroutine(null));
	}

	// Aimlessly move about
	public void Execute_Wander () {
		// Do nothing if we're already wandering
		if (CurrentActivity == NPCBehaviourAI.Activity.Wander)
			return;
		CurrentActivity = NPCBehaviourAI.Activity.Wander;

		StopAllCoroutines ();
		StartCoroutine (WanderCoroutine ());
	}


	// Assumes we're within punching range of the tree
	IEnumerator HarvestTreeCoroutine (BreakableTree tree, ExecutionCallbackFailable callback)
	{
		bool didFinish = false;
		bool didSucceed = false;

		IAiBehaviour behaviour = new HarvestTreeBehaviour(npc, tree, (bool success) => { didFinish = true; didSucceed = success; });
		behaviour.Execute();

		while (!didFinish)
		{
			yield return null;
		}
		callback?.Invoke(didSucceed);
	}
	// Assumes that we're next to the object to be destroyed
	IEnumerator DestroyBreakableObjectCoroutine(BreakableObject breakableObject, ExecutionCallbackDroppedItemsFailable callback)
	{
		bool didFinish = false;
		bool didSucceed = false;
		List<DroppedItem> returnedItems = null;

		IAiBehaviour behaviour = new DestroyBreakableObjectBehaviour(
			npc,
			breakableObject,
			(bool success, List<DroppedItem> items) => { didFinish = true; returnedItems = items; didSucceed = success; }
		);

		behaviour.Execute();

		while (!didFinish)
		{
			yield return null;
		}
		callback?.Invoke(didSucceed, returnedItems);
	}

	IEnumerator EatSomethingCoroutine(ExecutionCallbackFailable callback)
	{
		bool didFinish = false;
		bool didSucceed = false;

		IAiBehaviour behaviour = new EatSomethingBehaviour(npc, (bool success) => { didFinish = true; didSucceed = success; });
		behaviour.Execute();

		while (!didFinish)
		{
			yield return null;
		}
		callback?.Invoke(didSucceed);
	}

	IEnumerator MoveRandomlyCoroutine(int steps, ExecutionCallback callback)
	{
		bool didFinish = false;

		IAiBehaviour behaviour = new MoveRandomlyBehaviour(npc, steps, () => { didFinish = true; });
		behaviour.Execute();

		while (!didFinish)
		{
			yield return null;
		}
		callback?.Invoke();
	}

	IEnumerator NavigateNextToObjectCoroutine(GameObject gameObject, string scene, ExecutionCallbackFailable callback)
	{
		bool didFinish = false;
		bool didSucceed = false;

		IAiBehaviour behaviour = new NavigateNextToObjectBehaviour(npc, gameObject, scene, (bool success) => { didFinish = true; didSucceed = success; });
		behaviour.Execute();

		while (!didFinish)
		{
			yield return null;
		}
		callback?.Invoke(didSucceed);
	}

	IEnumerator ScavengeForFoodCoroutine ()
	{
		IAiBehaviour behaviour = new ScavengeForFoodBehaviour(npc);
		behaviour.Execute();

		while (behaviour.IsRunning)
		{
			yield return null;
		}
	}

	IEnumerator ScavengeForWoodCoroutine()
	{
		IAiBehaviour behaviour = new ScavengeForWoodBehaviour(npc);
		behaviour.Execute();

		while (behaviour.IsRunning)
		{
			yield return null;
		}
	}
	// TODO a generic way to stash things, by specifying types of containers for storing certain items
	IEnumerator StashWoodCoroutine (ExecutionCallbackFailable callback)
	{
		IAiBehaviour behaviour = new StashWoodBehaviour(npc, callback);
		behaviour.Execute();

		while (behaviour.IsRunning)
		{
			yield return null;
		}
	}
	// Travel from one place to another, including across scenes
	IEnumerator TravelCoroutine (TileLocation destination, ExecutionCallbackFailable callback) {
		bool didFinish = false;
		bool didSucceed = false;

		IAiBehaviour navBehaviour = new NavigateBehaviour(npc, destination, (bool success) => { didFinish = true; didSucceed = success; });
		navBehaviour.Execute();

		while (!didFinish)
		{
			yield return null;
		}
		callback?.Invoke(didSucceed);
	}

	IEnumerator WanderCoroutine ()
	{
		IAiBehaviour behaviour = new WanderBehaviour(npc);
		behaviour.Execute();

		while (behaviour.IsRunning)
		{
			yield return null;
		}
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

	void OnNavigationFinished () {
		isWaitingForNavigationToFinish = false;
	}
}
