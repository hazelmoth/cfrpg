using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigateBehaviour : IAiBehaviour
{
	// The max number of times we will recalculate a route if navigation fails
	private const int MAX_NAV_RETRIES = 3;

	private Actor actor;
	private ActorNavigator nav;
	private TileLocation destination;
	private ISet<TileLocation> blockedTiles;
	private ActorBehaviourExecutor.ExecutionCallbackFailable callback;
	private Coroutine coroutineObject;

	private bool isWaitingForNavigationToFinish = false;
	private bool navDidFail = false;
	private int failedAttempts = 0;

	public bool IsRunning { get; private set; } = false;

	public NavigateBehaviour(Actor Actor, TileLocation destination, ActorBehaviourExecutor.ExecutionCallbackFailable callback)
	{
		this.actor = Actor;
		nav = Actor.GetComponent<ActorNavigator>();
		this.destination = destination;
		this.callback = callback;
		blockedTiles = new HashSet<TileLocation>();
	}

	public void Execute()
	{
		if (coroutineObject != null)
		{
			actor.StopCoroutine(coroutineObject);
		}
		IsRunning = true;
		coroutineObject = actor.StartCoroutine(TravelCoroutine(destination, callback));
	}

	public void Cancel()
	{
		if (!IsRunning) return;

		if (coroutineObject != null)
		{
			actor.StopCoroutine(coroutineObject);
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
			actor.StopCoroutine(coroutineObject);
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
			// We're assuming that the scene the blocked tile is on is the same one this Actor is on
			string scene = actor.CurrentScene;
			Vector2Int scenePos = TilemapInterface.WorldPosToScenePos(discoveredObstacleWorldPos, actor.CurrentScene).ToVector2Int();
			blockedTiles.Add(new TileLocation(scenePos.x, scenePos.y, actor.CurrentScene));
		}
		navDidFail = !didSucceed;
	}

	private IEnumerator TravelCoroutine(TileLocation destination, ActorBehaviourExecutor.ExecutionCallbackFailable callback)
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

			if (tile.Scene == actor.CurrentScene)
			{
				blockedTilesInScene.Add(new Vector2(tile.x, tile.y));
			}
		}

		nav.CancelNavigation();
		if (destination.Scene != actor.CurrentScene)
		{
			// Find a portal to traverse scenes
			// TODO not have every Actor use the same portal every time (take the closest one instead)
			List<ScenePortal> availablePortals = ScenePortalLibrary.GetPortalsBetweenScenes(actor.GetComponent<Actor>().CurrentScene, destination.Scene);

			if (availablePortals.Count == 0)
			{
				Debug.LogWarning("Cross-scene navigation failed; no suitable scene portal exists!");
				callback?.Invoke(false);
				yield break;
			}

			ScenePortal targetPortal = availablePortals[0];

			List<Vector2Int> possibleLocations = Pathfinder.GetValidAdjacentTiles(
				actor.CurrentScene,
				TilemapInterface.WorldPosToScenePos(targetPortal.transform.position,
				targetPortal.PortalScene),
				blockedTilesInScene);

			if (possibleLocations.Count == 0)
			{
				// Scene portal is blocked.
				// No path found
				Debug.LogWarning("Scene portal is blocked.", targetPortal);
				Cancel();
				yield break;
			}

			Vector2 targetLocation = possibleLocations[0];

			IList<Vector2> navPath = Pathfinder.FindPath(
				actor.transform.localPosition,
				targetLocation,
				actor.CurrentScene,
				blockedTilesInScene);

			if (navPath == null)
			{
				// No path found
				Cancel();
				yield break;
			}
			isWaitingForNavigationToFinish = true;

			nav.FollowPath(
				navPath,
				actor.CurrentScene,
				OnNavigationFinished);

			while (isWaitingForNavigationToFinish)
			{
				yield return null;
			}

			// Turn towards scene portal
			nav.ForceDirection(Pathfinder.GetDirectionToLocation(actor.transform.position, targetPortal.transform.position));
			// Pause for a sec
			yield return new WaitForSeconds(0.3f);
			// Activate portal
			ScenePortalActivator.Activate(actor, targetPortal);

			// Since we just entered a new scene, we can assume that 
			// there are no known blocked tiles in this scene to avoid.

			// ...We did, right?
			if (actor.CurrentScene != destination.Scene)
			{
				Debug.LogError("Uhh... That scene portal didn't work?");
			}

			navPath = Pathfinder.FindPath(
				TilemapInterface.WorldPosToScenePos(actor.transform.position, actor.CurrentScene),
				destination.Position,
				actor.CurrentScene,
				null);

			if (navPath == null)
			{
				// No path found
				IsRunning = false;
				callback?.Invoke(false);
				yield break;
			}

			// Finish navigation
			isWaitingForNavigationToFinish = true;
			nav.FollowPath(navPath, actor.CurrentScene, OnNavigationFinished);
		}
		else
		{
			// Destination is on same scene
			IList<Vector2> navPath = Pathfinder.FindPath(
				TilemapInterface.WorldPosToScenePos(
					actor.transform.position,
					actor.CurrentScene),
				new Vector2(
					destination.x,
					destination.y),
				actor.CurrentScene,
				blockedTilesInScene);

			if (navPath == null)
			{
				callback?.Invoke(false);
				IsRunning = false;
				yield break;
			}

			isWaitingForNavigationToFinish = true;
			nav.FollowPath(
				navPath,
				actor.CurrentScene,
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
		IsRunning = false;

		yield break;
	}
}
