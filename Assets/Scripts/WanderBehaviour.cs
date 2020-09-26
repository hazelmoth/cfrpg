using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderBehaviour : IAiBehaviour
{
	private const int randomWalkSteps = 20;
	private const float navTimeout = 20f;

	private Actor Actor;
	private Coroutine activeCoroutine;
	private IAiBehaviour navSubBehaviour;

	public bool IsRunning { get; private set; }
	public void Cancel()
	{
		if (activeCoroutine != null)
		{
			Actor.StopCoroutine(activeCoroutine);
		}
		navSubBehaviour.Cancel();
		IsRunning = false;
	}
	public void Execute()
	{
		activeCoroutine = Actor.StartCoroutine(WanderCoroutine());
		IsRunning = true;
	}

	public WanderBehaviour(Actor Actor)
	{
		this.Actor = Actor;
	}

	private IEnumerator WanderCoroutine()
	{
		while (true)
		{
			Vector2 destVector = Pathfinder.FindRandomNearbyPathTile(TilemapInterface.WorldPosToScenePos(Actor.transform.position, Actor.CurrentScene), 20, Actor.CurrentScene);
			TileLocation dest = new TileLocation(destVector.ToVector2Int(), Actor.CurrentScene);

			bool navDidFinish = false;
			navSubBehaviour = new NavigateBehaviour(Actor, dest, (bool success) => { navDidFinish = true; });
			navSubBehaviour.Execute();

			float navStartTime = Time.time;
			while (!navDidFinish)
			{
				if (Time.time - navStartTime >= navTimeout)
				{
					// Timed out.
					navSubBehaviour.Cancel();
					break;
				}
				yield return null;
			}

			// Pause for a bit before walking again
			yield return new WaitForSeconds(Random.Range(1f, 5f));
		}
	}
}
