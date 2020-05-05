using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigateBehaviour : IAiBehaviour
{
	// The max number of times we will recalculate a route if navigation fails
	private const int MAX_NAV_RETRIES = 3;

	private Actor Actor;
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
		this.Actor = Actor;
		nav = Actor.GetComponent<ActorNavigator>();
		this.destination = destination;
		this.callback = callback;
		blockedTiles = new HashSet<TileLocation>();
	}

	public void Execute()
	{
		IsRunning = true;
		coroutineObject = Actor.StartCoroutine(TravelCoroutine(destination, callback));
	}
	public void Cancel()
	{
		if (coroutineObject != null)
		{
			Actor.StopCoroutine(coroutineObject);
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
			Actor.StopCoroutine(coroutineObject);
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
			string scene = Actor.CurrentScene;
			Vector2Int scenePos = TilemapInterface.WorldPosToScenePos(discoveredObstacleWorldPos, Actor.CurrentScene).ToVector2Int();
			blockedTiles.Add(new TileLocation(scenePos.x, scenePos.y, Actor.CurrentScene));
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

			if (tile.Scene == Actor.CurrentScene)
			{
				blockedTilesInScene.Add(new Vector2(tile.x, tile.y));
			}
		}

		nav.CancelNavigation();
		if (destination.Scene != Actor.CurrentScene)
		{
			// Find a portal to traverse scenes
			// TODO not have every Actor use the same portal every time (take the closest one instead)
			ScenePortal targetPortal = ScenePortalLibrary.GetPortalsBetweenScenes(Actor.GetComponent<Actor>().CurrentScene, destination.Scene)[0];
			if (targetPortal == null)
			{
				Debug.LogWarning("Cross-scene navigation failed; no suitable scene portal exists!");
				callback?.Invoke(false);
				yield break;
			}

			Vector2 targetLocation = Pathfinder.GetValidAdjacentTiles(
				Actor.CurrentScene,
				TilemapInterface.WorldPosToScenePos(targetPortal.transform.position,
				targetPortal.gameObject.scene.name),
				blockedTilesInScene)[0];

			isWaitingForNavigationToFinish = true;

			IList<Vector2> navPath = Pathfinder.FindPath(
				Actor.transform.localPosition,
				targetLocation,
				Actor.CurrentScene,
				blockedTilesInScene);

			if (navPath == null)
			{
				// No path found
				callback?.Invoke(false);
				yield break;
			}

			nav.FollowPath(
				navPath,
				Actor.CurrentScene,
				OnNavigationFinished);

			while (isWaitingForNavigationToFinish)
			{
				yield return null;
			}

			// Turn towards scene portal
			nav.ForceDirection(Pathfinder.GetDirectionToLocation(Actor.transform.position, targetPortal.transform.position));
			// Pause for a sec
			yield return new WaitForSeconds(0.3f);
			// Activate portal
			Actor.GetComponent<ActorBehaviourExecutor>().ActivateScenePortal(targetPortal);

			// Since we just entered a new scene, we can assume that 
			// there are no known blocked tiles in this scene to avoid

			// Finish navigation
			isWaitingForNavigationToFinish = true;
			nav.FollowPath(Pathfinder.FindPath(
				TilemapInterface.WorldPosToScenePos(Actor.transform.position, Actor.CurrentScene),
				destination.Position,
				Actor.CurrentScene,
				null
			), Actor.CurrentScene, OnNavigationFinished);
		}
		else
		{
			// Destination is on same scene
			IList<Vector2> navPath = Pathfinder.FindPath(
				TilemapInterface.WorldPosToScenePos(
					Actor.transform.position,
					Actor.CurrentScene),
				new Vector2(
					destination.x,
					destination.y),
				Actor.CurrentScene,
				blockedTilesInScene);

			if (navPath == null)
			{
				callback?.Invoke(false);
				yield break;
			}

			isWaitingForNavigationToFinish = true;
			nav.FollowPath(
				navPath,
				Actor.CurrentScene,
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
