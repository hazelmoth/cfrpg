using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRandomlyBehaviour : IAiBehaviour
{
	NPC npc;
	NPCActivityExecutor.ExecutionCallback callback;
	NPCNavigator nav;
	IAiBehaviour navSubBehaviour;
	int stepsToWalk;

	Coroutine activeCoroutine;

	public bool IsRunning { get; private set; }

	public void Cancel()
	{
		if (activeCoroutine != null)
			npc.StopCoroutine(activeCoroutine);
		navSubBehaviour?.Cancel();
		IsRunning = false;
		callback?.Invoke();
	}

	public void Execute()
	{
		activeCoroutine = npc.StartCoroutine(MoveRandomlyCoroutine());
		IsRunning = true;
	}
	public MoveRandomlyBehaviour(NPC npc, int stepsToWalk, NPCActivityExecutor.ExecutionCallback callback)
	{
		this.npc = npc;
		this.callback = callback;
		this.stepsToWalk = stepsToWalk;
		nav = npc.GetComponent<NPCNavigator>();
	}

	IEnumerator MoveRandomlyCoroutine()
	{
		Vector2 destVector = TileNavigationHelper.FindRandomNearbyPathTile(TilemapInterface.WorldPosToScenePos(npc.transform.position, npc.CurrentScene), stepsToWalk, npc.CurrentScene);
		TileLocation destination = new TileLocation(destVector.ToVector2Int(), npc.CurrentScene);

		bool navDidFinish = false;
		navSubBehaviour = new NavigateBehaviour(npc, destination, (bool success) => { navDidFinish = true; });
		navSubBehaviour.Execute();

		while (!navDidFinish)
		{
			yield return null;
		}
		IsRunning = false;
		callback?.Invoke();
	}
}
