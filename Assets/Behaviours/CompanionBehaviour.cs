using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionBehaviour : IAiBehaviour
{
	// The minimum distance at which we'll move towards the target
	private const float TargetDist = 4f;

	private IAiBehaviour navBehaviour;
	private Coroutine routine;
	private readonly Actor actor;
	private readonly Actor target;
	private bool isRunning;

	public CompanionBehaviour(Actor actor, Actor target)
	{
		this.actor = actor;
		this.target = target;
	}

	bool IAiBehaviour.IsRunning => isRunning;

	void IAiBehaviour.Cancel()
	{
		if (routine != null)
		{
			actor.StopCoroutine(routine);
		}
		navBehaviour?.Cancel();
		isRunning = false;
	}

	void IAiBehaviour.Execute()
	{
		routine = actor.StartCoroutine(Routine());
		isRunning = true;
	}

	private IEnumerator Routine()
	{
		while (true)
		{
			if (!NearTarget)
			{
				bool navFinished = false;
				bool navSucceeded = false;
				TileLocation targetLocation = target.Location;
				navBehaviour = new NavigateBehaviour((NPC) actor, targetLocation,
					succeeded =>
					{
						navFinished = true;
						navSucceeded = succeeded;
					}
				);
				while (!navFinished)
				{
					if (NearTarget)
					{
						navBehaviour.Cancel();
						break;
					}
					yield return null;
				}
			}
			yield return null;
		}
	}

	private bool NearTarget => Vector2.Distance(actor.transform.position, target.transform.position) <= TargetDist;
}
