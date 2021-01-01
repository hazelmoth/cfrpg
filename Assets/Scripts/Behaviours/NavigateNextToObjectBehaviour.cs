using System.Collections.Generic;
using UnityEngine;

// A behaviour that navigates the actor to a valid tile next to a given object.
public class NavigateNextToObjectBehaviour : IAiBehaviour
{
	private Actor Actor;
	private GameObject targetObject;
	private string targetScene;
	private ActorBehaviourExecutor.ExecutionCallbackFailable callback;

	private IAiBehaviour navigationSubBehaviour;
	private bool isRunning = false;

	public NavigateNextToObjectBehaviour(Actor Actor, GameObject targetObject, string targetScene, ActorBehaviourExecutor.ExecutionCallbackFailable callback)
	{
		this.Actor = Actor;
		this.targetObject = targetObject;
		this.targetScene = targetScene;
		this.callback = callback;
	}

	public bool IsRunning => isRunning;

	public void Cancel()
	{
		navigationSubBehaviour?.Cancel();
		callback(false);
	}

	public void Execute()
	{
		isRunning = true;
		StartNavigation(targetObject, targetScene);
	}

	private void StartNavigation(GameObject gameObject, string scene)
	{
		TileLocation navDest;
		if (TryFindAdjacentTile(gameObject, scene, out navDest))
		{
			navigationSubBehaviour = new NavigateBehaviour(Actor, navDest, OnNavFinished);
			navigationSubBehaviour.Execute();
		}
		else
		{
			Cancel();
		}
	}

	private void OnNavFinished (bool didSucceed)
	{
		isRunning = false;
		callback(didSucceed);
	}

	private bool TryFindAdjacentTile(GameObject gameObject, string scene, out TileLocation navDest)
	{
		Vector2Int rootPos = TilemapInterface.WorldPosToScenePos(gameObject.transform.position, scene).ToVector2Int();

		List<Vector2Int> objectTiles = new List<Vector2Int>();
		if (gameObject.TryGetComponent(out EntityObject entity))
		{
			foreach (Vector2 pos in ContentLibrary.Instance.Entities.Get(entity.EntityId).baseShape)
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
			Debug.LogWarning(Actor.name + " tried to navigate to an object with no valid adjacent tiles", gameObject);
			OnNavFinished(false);
			navDest = new TileLocation();
			return false;
		}

		// Just take the first one, I guess
		navDest = new TileLocation(validAdjacentTiles[0], scene);
		return true;
	}
}
