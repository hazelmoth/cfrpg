using System.Collections;
using UnityEngine;

public class MoveRandomlyBehaviour : IAiBehaviour
{
	private Actor Actor;
	private ActorBehaviourExecutor.ExecutionCallback callback;
	private ActorNavigator nav;
	private IAiBehaviour navSubBehaviour;
	private int stepsToWalk;

	private Coroutine activeCoroutine;

	public bool IsRunning { get; private set; }

	public void Cancel()
	{
		if (activeCoroutine != null)
			Actor.StopCoroutine(activeCoroutine);
		navSubBehaviour?.Cancel();
		IsRunning = false;
		callback?.Invoke();
	}

	public void Execute()
	{
		activeCoroutine = Actor.StartCoroutine(MoveRandomlyCoroutine());
		IsRunning = true;
	}
	public MoveRandomlyBehaviour(Actor Actor, int stepsToWalk, ActorBehaviourExecutor.ExecutionCallback callback)
	{
		this.Actor = Actor;
		this.callback = callback;
		this.stepsToWalk = stepsToWalk;
		nav = Actor.GetComponent<ActorNavigator>();
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
		IsRunning = false;
		callback?.Invoke();
	}
}
