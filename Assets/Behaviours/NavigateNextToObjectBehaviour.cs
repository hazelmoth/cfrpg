using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO use NavigateBehaviour for this, since it knows how to handle obstacle checking
public class NavigateNextToObjectBehaviour : IAiBehaviour
{
	NPC npc;
	GameObject targetObject;
	string targetScene;
	NPCBehaviourExecutor.ExecutionCallbackFailable callback;

	IAiBehaviour navigationSubBehaviour;
	bool isRunning = false;

	public NavigateNextToObjectBehaviour(NPC npc, GameObject targetObject, string targetScene, NPCBehaviourExecutor.ExecutionCallbackFailable callback)
	{
		this.npc = npc;
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

	void StartNavigation(GameObject gameObject, string scene)
	{
		// TODO: handle entities that cover multiple tiles
		Vector2 locationInScene = TilemapInterface.WorldPosToScenePos(gameObject.transform.position, scene);

		// Determine which side of the object is best to approach;
		// offset is (1,0), (-1, 0), (0, 1) or (0,-1)
		Vector2 offset = (TilemapInterface.WorldPosToScenePos(npc.transform.position, npc.CurrentScene) - locationInScene).ToDirection().ToVector2();
		Vector2 navigationTarget = locationInScene + offset;

		List<Vector2Int> validAdjacentTiles = TileNavigationHelper.GetValidAdjacentTiles(scene, locationInScene, null);
		// If the ideal target isn't walkable, just find one that works
		if (!validAdjacentTiles.Contains(Vector2Int.FloorToInt(navigationTarget)))
		{
			if (validAdjacentTiles.Count == 0)
			{
				// No valid adjacent tiles exist
				Debug.LogWarning(npc.name + " tried to navigate to an object with no valid adjacent tiles");
				OnNavFinished(false);
				return;
			}
			navigationTarget = validAdjacentTiles[0];
		}

		TileLocation navDest = new TileLocation(navigationTarget.ToVector2Int(), scene);
		navigationSubBehaviour = new NavigateBehaviour(npc, navDest, OnNavFinished);
		navigationSubBehaviour.Execute();
	}

	void OnNavFinished (bool didSucceed)
	{
		isRunning = false;
		callback(didSucceed);
	}

}
