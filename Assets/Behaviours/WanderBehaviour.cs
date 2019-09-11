using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderBehaviour : IAiBehaviour
{
	const int randomWalkSteps = 20;
	const float navTimeout = 20f;

	NPC npc;
	Coroutine activeCoroutine;
	IAiBehaviour navSubBehaviour;

	public bool IsRunning { get; private set; }
	public void Cancel()
	{
		if (activeCoroutine != null)
		{
			npc.StopCoroutine(activeCoroutine);
		}
		navSubBehaviour.Cancel();
		IsRunning = false;
	}
	public void Execute()
	{
		activeCoroutine = npc.StartCoroutine(WanderCoroutine());
		IsRunning = true;
	}

	public WanderBehaviour(NPC npc)
	{
		this.npc = npc;
	}

	IEnumerator WanderCoroutine()
	{
		while (true)
		{
			Vector2 destVector = TileNavigationHelper.FindRandomNearbyPathTile(TilemapInterface.WorldPosToScenePos(npc.transform.position, npc.CurrentScene), 20, npc.CurrentScene);
			TileLocation dest = new TileLocation(destVector.ToVector2Int(), npc.CurrentScene);

			bool navDidFinish = false;
			navSubBehaviour = new NavigateBehaviour(npc, dest, (bool success) => { navDidFinish = true; });
			navSubBehaviour.Execute();

			float navStartTime = Time.time;
			while (!navDidFinish)
			{
				if (Time.time - navStartTime >= navTimeout)
				{
					Debug.Log("Nav timed out.");
					Cancel();
				}
				yield return null;
			}

			// Pause for a bit before walking again
			yield return new WaitForSeconds(Random.Range(1f, 5f));
		}
	}
}
