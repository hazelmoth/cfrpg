using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigateBehaviour : IAiBehaviour
{
	// The max number of times we will recalculate a route if navigation fails
	private const int MAX_NAV_RETRIES = 3;

	private NPC npc;
	private NPCNavigator nav;
	private TileLocation destination;
	private ISet<TileLocation> blockedTiles;
	private NPCBehaviourExecutor.ExecutionCallbackFailable callback;
	private Coroutine coroutineObject;

	bool isWaitingForNavigationToFinish = false;
	bool navDidFail = false;
	int failedAttempts = 0;

	public bool IsRunning { get; private set; } = false;

	public NavigateBehaviour(NPC npc, TileLocation destination, NPCBehaviourExecutor.ExecutionCallbackFailable callback)
	{
		this.npc = npc;
		nav = npc.GetComponent<NPCNavigator>();
		this.destination = destination;
		this.callback = callback;
		blockedTiles = new HashSet<TileLocation>();
	}

	public void Execute()
	{
		IsRunning = true;
		coroutineObject = npc.StartCoroutine(TravelCoroutine(destination, callback));
	}
	public void Cancel()
	{
		if (coroutineObject != null)
		{
			npc.StopCoroutine(coroutineObject);
		}
		nav.CancelNavigation();
		isWaitingForNavigationToFinish = false;
		IsRunning = false;
		callback?.Invoke(false);
	}
	
	private void RetryNavigation ()
	{
		if (coroutineObject != null)
		{
			npc.StopCoroutine(coroutineObject);
		}
		nav.CancelNavigation();
		isWaitingForNavigationToFinish = false;
		Execute();
	}
	private void OnNavigationFinished (bool didSucceed, Vector2Int discoveredObstacleWorldPos)
	{
		isWaitingForNavigationToFinish = false;
		if (!didSucceed)
		{
			// We're assuming that the scene the blocked tile is on is the same one this npc is on
			string scene = npc.CurrentScene;
			Vector2Int scenePos = TilemapInterface.WorldPosToScenePos(discoveredObstacleWorldPos, npc.CurrentScene).ToVector2Int();
			blockedTiles.Add(new TileLocation(scenePos.x, scenePos.y, npc.CurrentScene));
		}
		navDidFail = !didSucceed;
	}

	IEnumerator TravelCoroutine(TileLocation destination, NPCBehaviourExecutor.ExecutionCallbackFailable callback)
	{
		ISet<Vector2> blockedTilesInScene = new HashSet<Vector2>();
		foreach (TileLocation tile in blockedTiles)
		{
			if (tile.Equals(destination))
			{
				// Destination is blocked; instant failure.
				Cancel();
				yield break;
			}

			if (tile.Scene == npc.CurrentScene)
			{
				blockedTilesInScene.Add(new Vector2(tile.x, tile.y));
			}
		}

		nav.CancelNavigation();
		if (destination.Scene != npc.CurrentScene)
		{
			// Find a portal to traverse scenes
			// TODO not have every NPC use the same portal every time (take the closest one instead)
			ScenePortal targetPortal = ScenePortalLibrary.GetPortalsBetweenScenes(npc.GetComponent<NPC>().CurrentScene, destination.Scene)[0];
			if (targetPortal == null)
			{
				Debug.LogWarning("Cross-scene navigation failed; no suitable scene portal exists!");
				callback?.Invoke(false);
				yield break;
			}

			Vector2 targetLocation = Pathfinder.GetValidAdjacentTiles(
				npc.CurrentScene,
				TilemapInterface.WorldPosToScenePos(targetPortal.transform.position,
				targetPortal.gameObject.scene.name),
				blockedTilesInScene)[0];

			isWaitingForNavigationToFinish = true;

			IList<Vector2> navPath = Pathfinder.FindPath(
				npc.transform.localPosition,
				targetLocation,
				npc.CurrentScene,
				blockedTilesInScene);

			if (navPath == null)
			{
				// No path found
				callback?.Invoke(false);
				yield break;
			}

			nav.FollowPath(
				navPath,
				npc.CurrentScene,
				OnNavigationFinished);

			while (isWaitingForNavigationToFinish)
			{
				yield return null;
			}

			// Turn towards scene portal
			nav.ForceDirection(Pathfinder.GetDirectionToLocation(npc.transform.position, targetPortal.transform.position));
			// Pause for a sec
			yield return new WaitForSeconds(0.3f);
			// Activate portal
			npc.GetComponent<NPCBehaviourExecutor>().ActivateScenePortal(targetPortal);

			// Since we just entered a new scene, we can assume that 
			// there are no known blocked tiles in this scene to avoid

			// Finish navigation
			isWaitingForNavigationToFinish = true;
			nav.FollowPath(Pathfinder.FindPath(
				TilemapInterface.WorldPosToScenePos(npc.transform.position, npc.CurrentScene),
				destination.Position,
				npc.CurrentScene,
				null
			), npc.CurrentScene, OnNavigationFinished);
		}
		else
		{
			// Destination is on same scene
			IList<Vector2> navPath = Pathfinder.FindPath(
				TilemapInterface.WorldPosToScenePos(
					npc.transform.position,
					npc.CurrentScene),
				new Vector2(
					destination.x,
					destination.y),
				npc.CurrentScene,
				blockedTilesInScene);

			if (navPath == null)
			{
				callback?.Invoke(false);
				yield break;
			}

			isWaitingForNavigationToFinish = true;
			nav.FollowPath(
				navPath,
				npc.CurrentScene,
				OnNavigationFinished);
		}

		while (isWaitingForNavigationToFinish)
		{
			yield return null;
		}
		if (navDidFail)
		{
			failedAttempts++;
			if (failedAttempts < MAX_NAV_RETRIES)
			{
				RetryNavigation();
			}
			else
			{
				callback?.Invoke(false);
			}
		}
		else
		{
			callback?.Invoke(true);
		}
	}
}
