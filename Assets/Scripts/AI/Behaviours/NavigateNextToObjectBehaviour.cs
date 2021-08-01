using System.Collections.Generic;
using System.Linq;
using ContentLibraries;
using UnityEngine;

// A behaviour that navigates the actor to a valid tile next to a given object.
namespace AI.Behaviours
{
	public class NavigateNextToObjectBehaviour : IAiBehaviour
	{
		private Actor actor;
		private GameObject targetObject;
		private string targetScene;
		private ActorBehaviourExecutor.ExecutionCallbackFailable callback;

		private IAiBehaviour navigationSubBehaviour;
		private bool isRunning = false;

		public NavigateNextToObjectBehaviour(Actor Actor, GameObject targetObject, string targetScene, ActorBehaviourExecutor.ExecutionCallbackFailable callback)
		{
			this.actor = Actor;
			this.targetObject = targetObject;
			this.targetScene = targetScene;
			this.callback = callback;
		}

		public bool IsRunning => isRunning;

		public void Cancel()
		{
			if (!IsRunning) return;

			isRunning = false;
			navigationSubBehaviour?.Cancel();
			callback?.Invoke(false);
		}

		public void Execute()
		{
			navigationSubBehaviour?.Cancel();

			isRunning = true;
			StartNavigation(targetObject, targetScene);
		}

		private void StartNavigation(GameObject gameObject, string scene)
		{
			TileLocation navDest;
			if (TryFindAdjacentTile(gameObject, scene, out navDest))
			{
				if (actor.Location == navDest)
				{
					// We're already there.
					OnNavFinished(true);
					return;
				}
				navigationSubBehaviour = new NavigateBehaviour(actor, navDest, OnNavFinished);
				navigationSubBehaviour.Execute();
			}
			else
			{
				Debug.LogWarning("No suitable adjacent tile found next to target.", gameObject);
				Cancel();
			}
		}

		private void OnNavFinished (bool didSucceed)
		{
			isRunning = false;
			// Turn towards the entity
			if (didSucceed) actor.GetComponent<ActorMovementController>().ForceDirection(DirectionTowardsEntity(actor.transform.position, targetObject.GetComponent<EntityObject>()));
			callback?.Invoke(didSucceed);
		}

		private bool TryFindAdjacentTile(GameObject gameObject, string scene, out TileLocation navDest)
		{
			Vector2Int rootPos = TilemapInterface.WorldPosToScenePos(gameObject.transform.position, scene).ToVector2Int();

			List<Vector2Int> objectTiles = new List<Vector2Int>();
			if (gameObject.TryGetComponent(out EntityObject entity))
			{
				foreach (Vector2 pos in ContentLibrary.Instance.Entities.Get(entity.EntityId).BaseShape)
				{
					objectTiles.Add(rootPos + pos.ToVector2Int());
				}
			}
			else
			{
				objectTiles.Add(rootPos);
			}

			List<Vector2Int> validAdjacentTiles = new List<Vector2Int>();
			foreach (Vector2 pos in objectTiles)
			{
				foreach (Vector2Int tile in Pathfinder.GetValidAdjacentTiles(scene, pos.ToVector2Int(), null))
				{
					if (!objectTiles.Contains(tile)) validAdjacentTiles.Add(tile); // Don't add tiles which are part of the entity
				}
			}


			if (validAdjacentTiles.Count == 0)
			{
				// No valid adjacent tiles exist
				Debug.LogWarning(actor.name + " tried to navigate to an object with no valid adjacent tiles", gameObject);
				OnNavFinished(false);
				navDest = new TileLocation(Vector2Int.zero, null);
				return false;
			}

			// Just take the first one, I guess
			navDest = new TileLocation(validAdjacentTiles[0], scene);

			return true;
		}

		private Direction DirectionTowardsEntity(Vector2 currentWorldPos, EntityObject entity)
		{
			EntityData entData = entity.GetData();
			Vector2 entityRootPos = entity.transform.position;
			if (!entData.PivotAtCenterOfTile) entityRootPos += new Vector2(0.5f, 0.5f); // Make this the center of the entity root tile

			Vector2 closest = (from Vector2Int pos in entData.BaseShape
				orderby Vector2.Distance(currentWorldPos, entityRootPos + pos) ascending
				select entityRootPos + pos).First();

			return (closest - currentWorldPos).ToDirection();
		}
	}
}
