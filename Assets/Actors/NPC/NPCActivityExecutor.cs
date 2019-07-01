using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Contains functions that force an NPC to start or stop different tasks.
// These functions should be called by NPCBehaviourAI.
public class NPCActivityExecutor : MonoBehaviour {

	delegate void ExecutionCallback ();
	delegate void ExecutionCallbackFailable(bool didSucceed);
	delegate void ExecutionCallbackDroppedItems(List<DroppedItem> items);

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


	public void Execute_Eat (Item item) {
		StopAllCoroutines ();
		CurrentActivity = NPCBehaviourAI.Activity.None;
		ActorEatingSystem.AttemptEat (npc, item);
	}


	public void Execute_EatSomething () {
		StartCoroutine(EatSomethingCoroutine());
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
	IEnumerator HarvestTreeCoroutine (BreakableTree tree, ExecutionCallback callback)
	{
		BreakableObject breakable = tree.GetComponent<BreakableObject>();
		if (breakable != null) {
			Coroutine breakCoroutine = StartCoroutine(DestroyBreakableObjectCoroutine(breakable, OnItemsDropped));
			yield return breakCoroutine;
		} else {
			Debug.LogWarning("Tried to harvest a tree that doesn't have a BreakableObject component!");
		}
		Debug.Log("Destruction complete");
		
		void OnItemsDropped (List<DroppedItem> items)
		{
			StartCoroutine(PickUpItems(items));
		}

		//pick up wood
		IEnumerator PickUpItems (List<DroppedItem> items)
		{
			yield return new WaitForSeconds(0.5f);
			for (int i = 0; i < items.Count; i++)
			{
				if (items[i] != null && npc.Inventory.AttemptAddItemToInv(ItemManager.GetItemById(items[i].ItemId)))
				{
					yield return new WaitForSeconds(0.5f);
					GameObject.Destroy(items[i].gameObject);
				}
			}
			callback?.Invoke();
		}
	}

	// Assumes that we're next to the object to be destroyed
	IEnumerator DestroyBreakableObjectCoroutine(BreakableObject breakableObject, ExecutionCallbackDroppedItems callback)
	{
		if (puncher == null && breakableObject != null)
		{
			puncher = GetComponent<ActorPunchExecutor>();
			if (puncher == null)
				puncher = gameObject.AddComponent<ActorPunchExecutor>();
		}
		breakableObject.OnDropItems += ActivateCallback;
		Vector2 punchDir = (transform.position.ToVector2() - breakableObject.transform.position.ToVector2()).ToDirection().Invert().ToVector2();

		while (breakableObject != null)
		{
			puncher.InitiatePunch(punchDir);
			yield return null;
		}

		void ActivateCallback (List<DroppedItem> items) {
			callback(items);
		}
		// TODO callback if object isn't breaking
	}

	IEnumerator EatSomethingCoroutine ()
	{
		foreach (Item item in npc.Inventory.GetAllItems())
		{
			if (item != null && item.IsEdible)
			{
				Debug.Log(npc.NpcId + " is eating a " + item);
				yield return new WaitForSeconds(2f);
				Execute_Eat(item);
				npc.Inventory.RemoveOneInstanceOf(item);
				yield break;
			}
		}
		Debug.Log(npc.NpcId + " tried to eat but has no food!");
	}

	IEnumerator MoveRandomlyCoroutine(ExecutionCallback callback)
	{
		Debug.Log(npc.name + " couldn't find a plant");
		nav.FollowPath(TileNavigationHelper.FindPath(
			TilemapInterface.WorldPosToScenePos(transform.position, npc.ActorCurrentScene),
			TileNavigationHelper.FindRandomNearbyPathTile(TilemapInterface.WorldPosToScenePos(transform.position, npc.ActorCurrentScene), 20, npc.ActorCurrentScene),
			npc.ActorCurrentScene
		), npc.ActorCurrentScene);
		isWaitingForNavigationToFinish = true;
		while (isWaitingForNavigationToFinish)
		{
			yield return null;
		}
		callback?.Invoke();
	}

	IEnumerator HarvestPlantCoroutine(HarvestablePlant plant, ExecutionCallback callback)
	{
		DroppedItem item = null;
		if (plant != null)
			plant.Harvest(out item);

		// Wait a bit before picking up the item
		yield return new WaitForSeconds(0.5f);

		if (item != null && npc.Inventory.AttemptAddItemToInv(ItemManager.GetItemById(item.ItemId)))
		{
			GameObject.Destroy(item.gameObject);
		}
		callback?.Invoke();
	}

	IEnumerator NavigateNextToObjectCoroutine(GameObject gameObject, string scene, ExecutionCallbackFailable callback)
	{
		// TODO: handle entities that cover multiple tiles
		Vector2 locationInScene = TilemapInterface.WorldPosToScenePos(gameObject.transform.position, scene);

		// Determine which side of the object is best to approach;
		// offset is (1,0), (-1, 0), (0, 1) or (0,-1)
		Vector2 offset = (TilemapInterface.WorldPosToScenePos(transform.position, npc.ActorCurrentScene) - locationInScene).ToDirection().ToVector2();
		Vector2 navigationTarget = locationInScene + offset;

		List<Vector2Int> validAdjacentTiles = TileNavigationHelper.GetValidAdjacentTiles(scene, locationInScene);
		// If the ideal target isn't walkable, just find one that works
		if (!validAdjacentTiles.Contains(Vector2Int.FloorToInt(navigationTarget)))
		{
			if (validAdjacentTiles.Count == 0)
			{
				// No valid adjacent tiles exist
				Debug.LogWarning(npc.name + " tried to navigate to an object with no valid adjacent tiles");
				callback?.Invoke(false);
				yield break;
			}
			navigationTarget = validAdjacentTiles[0];
		}

		Coroutine travelCoroutine = StartCoroutine(TravelCoroutine(new TileLocation((int)navigationTarget.x, (int)navigationTarget.y, npc.ActorCurrentScene), callback));
		yield return travelCoroutine;
	}

	IEnumerator ScavengeForFoodCoroutine () {
		Vector2Int discoveredPlantLocation = new Vector2Int ();

		isWaitingForNavigationToFinish = false;

		while (true)
		{
			// Walk to a plant if there's one nearby
			// TODO support for multi-tile plants
			GameObject nearbyPlantObject = NearbyObjectLocaterSystem.FindClosestEntityWithComponent<HarvestablePlant> (transform.position, 20, npc.ActorCurrentScene, out discoveredPlantLocation);
			if (nearbyPlantObject != null) {

				Coroutine navigateCoroutine = StartCoroutine(NavigateNextToObjectCoroutine(nearbyPlantObject, npc.ActorCurrentScene, null));
				yield return navigateCoroutine;

				if (nearbyPlantObject == null)
					continue;

				Direction direction = (nearbyPlantObject.transform.position.ToVector2() - transform.position.ToVector2()).ToDirection();
				npc.Navigator.ForceDirection (direction);

				yield return new WaitForSeconds (Random.Range(0.1f, 0.5f));

				if (nearbyPlantObject != null) {
					Coroutine harvestRoutine = StartCoroutine (HarvestPlantCoroutine (nearbyPlantObject.GetComponent<HarvestablePlant> (), null));
					yield return harvestRoutine;
				} else {
					continue;
				}
				continue;
			}

			// If no plant is nearby, walk to a random nearby tile and look again
			if (!isWaitingForNavigationToFinish) {
				Coroutine randomMoveCoroutine = StartCoroutine(MoveRandomlyCoroutine(null));
				yield return randomMoveCoroutine;
			}
			// Pause for a bit before walking again
			yield return new WaitForSeconds (Random.Range (1f, 3f));
		}
	}

	IEnumerator ScavengeForWoodCoroutine()
	{
		Vector2Int discoveredTreeLocation = new Vector2Int();

		isWaitingForNavigationToFinish = false;

		while (true)
		{
			// Pause for a bit before walking again
			yield return new WaitForSeconds(Random.Range(1f, 3f));

			// Walk to a nearby tree if one exists
			// TODO support for multi-tile trees
			GameObject nearbyTreeObject = NearbyObjectLocaterSystem.FindClosestEntityWithComponent<BreakableTree>(transform.position, 20, npc.ActorCurrentScene, out discoveredTreeLocation);
			if (nearbyTreeObject != null)
			{

				Coroutine navigateCoroutine = StartCoroutine(NavigateNextToObjectCoroutine(nearbyTreeObject, npc.ActorCurrentScene, null));
				yield return navigateCoroutine;

				if (nearbyTreeObject == null)
					continue;

				// Turn to face the tree
				Direction direction = (nearbyTreeObject.transform.position.ToVector2() - transform.position.ToVector2()).ToDirection();
				npc.Navigator.ForceDirection(direction);

				yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));

				if (nearbyTreeObject != null)
				{
					Coroutine harvestRoutine = StartCoroutine(HarvestTreeCoroutine(nearbyTreeObject.GetComponent<BreakableTree>(), null));
					yield return harvestRoutine;
				}
				else
				{
					continue;
				}
				continue;
			}

			// If no tree is nearby, walk to a random nearby tile and look again
			if (!isWaitingForNavigationToFinish)
			{
				Coroutine randomMoveCoroutine = StartCoroutine(MoveRandomlyCoroutine(null));
				yield return randomMoveCoroutine;
			}
		}
	}

	// Travel from one place to another, including across scenes
	IEnumerator TravelCoroutine (TileLocation destination, ExecutionCallbackFailable callback) {
		nav.CancelNavigation();
		if (destination.Scene != this.GetComponent<NPC>().ActorCurrentScene) {
			// Find a portal to traverse scenes
			// TODO not have every NPC use the same portal every time (take the closest one instead)
			ScenePortal targetPortal = ScenePortalLibrary.GetPortalsBetweenScenes (this.GetComponent<NPC>().ActorCurrentScene, destination.Scene)[0];
			if (targetPortal == null) {
				Debug.LogWarning ("Cross-scene navigation failed; no suitable scene portal exists!");
				callback?.Invoke(false);
				yield break;
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
		callback?.Invoke(true);
	}

	IEnumerator WanderCoroutine () {
		while (true) {
			// Walk to a random nearby tile
			nav.FollowPath (TileNavigationHelper.FindPath (
				TilemapInterface.WorldPosToScenePos (transform.position, npc.ActorCurrentScene), 
				TileNavigationHelper.FindRandomNearbyPathTile (TilemapInterface.WorldPosToScenePos (transform.position, npc.ActorCurrentScene), 20, npc.ActorCurrentScene), 
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



	void ActivateScenePortal (ScenePortal portal) {
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
