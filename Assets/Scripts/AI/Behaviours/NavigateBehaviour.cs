using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Behaviours
{
	public class NavigateBehaviour : IAiBehaviour
	{
		// The max number of times we will recalculate a route if navigation fails
		private const int MAX_NAV_RETRIES = 3;

		private Actor actor;
		private ActorNavigator nav;
		private Location destination;
		private ISet<TileLocation> blockedTiles;
		private ActorBehaviourExecutor.ExecutionCallbackFailable callback;
		private Coroutine coroutineObject;
		private Actor ignoreActor; // Ignore this actor when collision checking, if not null

		private bool isWaitingForNavigationToFinish = false;
		private bool navDidFail = false;
		private int failedAttempts = 0;

		public bool IsRunning { get; private set; } = false;

		public NavigateBehaviour(Actor Actor, Location destination, ActorBehaviourExecutor.ExecutionCallbackFailable callback, Actor ignoreActor = null)
		{
			this.actor = Actor;
			nav = Actor.GetComponent<ActorNavigator>();
			this.destination = destination;
			this.callback = callback;
			blockedTiles = new HashSet<TileLocation>();
			this.ignoreActor = ignoreActor;
		}

		public void Execute()
		{
			if (coroutineObject != null)
			{
				actor.StopCoroutine(coroutineObject);
		
			}
			if (actor.Location == destination)
			{
				// We're already there! instant success.
				callback?.Invoke(true);
				return;
			}

			IsRunning = true;
			coroutineObject = actor.StartCoroutine(TravelCoroutine(destination, callback, ignoreActor));
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
				Vector2Int scenePos = TilemapInterface.WorldPosToScenePos(discoveredObstacleWorldPos, actor.CurrentScene).ToVector2Int();
				blockedTiles.Add(new TileLocation(scenePos.x, scenePos.y, actor.CurrentScene));
			}
			navDidFail = !didSucceed;
		}



		private IEnumerator TravelCoroutine(Location destination, ActorBehaviourExecutor.ExecutionCallbackFailable callback, Actor ignoreActor = null)
		{
			nav.CancelNavigation();
			throw new NotImplementedException();
		}
	}
}
