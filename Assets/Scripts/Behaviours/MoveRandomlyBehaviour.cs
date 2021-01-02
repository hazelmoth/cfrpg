using System;
using System.Collections;
using UnityEngine;

public class MoveRandomlyBehaviour : IAiBehaviour
{
	private Actor Actor;
	private Action callback;
	private IAiBehaviour navSubBehaviour;
	private int stepsToWalk;

	private Coroutine activeCoroutine;

	public bool IsRunning { get; private set; }

	public void Cancel()
	{
		if (activeCoroutine != null)
			Actor.StopCoroutine(activeCoroutine);

		IsRunning = false;
		navSubBehaviour?.Cancel();
		callback?.Invoke();
	}

	public void Execute()
	{
		activeCoroutine = Actor.StartCoroutine(MoveRandomlyCoroutine());
		IsRunning = true;
	}
	public MoveRandomlyBehaviour(Actor Actor, int stepsToWalk, Action callback)
	{
		this.Actor = Actor;
		this.callback = callback;
		this.stepsToWalk = stepsToWalk;
	}

	private IEnumerator MoveRandomlyCoroutine()
	{
		Vector2 destVector = Pathfinder.FindRandomNearbyPathTile(TilemapInterface.WorldPosToScenePos(Actor.transform.position, Actor.CurrentScene), stepsToWalk, Actor.CurrentScene);
		TileLocation destination = new TileLocation(destVector.ToVector2Int(), Actor.CurrentScene);

		bool navDidFinish = false;
		navSubBehaviour = new NavigateBehaviour(Actor, destination, success => { navDidFinish = true; });
		navSubBehaviour.Execute();

		while (!navDidFinish)
		{
			yield return null;
		}
		if (navSubBehaviour.IsRunning) Debug.LogError("Uh, this thing that claimed to finish is still running!", Actor);

		IsRunning = false;
		callback?.Invoke();
	}
}
