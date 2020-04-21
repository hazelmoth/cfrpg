using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionBehaviour : IAiBehaviour
{
	// The minimum distance at which we'll move towards the target
	private const float TargetDist = 2f;
	private const float BufferRadius = 0.5f; // The distance to the target zone within which we won't bother navigating

	private IAiBehaviour navBehaviour;
	private Coroutine routine;
	public readonly Actor actor;
	public readonly Actor target;
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
			if (!WithinTargetMargin)
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
				navBehaviour.Execute();
				yield return new WaitForSeconds(0.5f);
				while (!navFinished)
				{
					if (NearTarget)
					{
						navBehaviour.Cancel();
						actor.GetComponent<HumanAnimController>().SetDirection((targetLocation.Position - actor.transform.position.ToVector2()).ToDirection());
						break;
					}
					yield return null;
				}
			}

			yield return null;
		}
	}

	private bool NearTarget => Vector2.Distance(actor.transform.position, target.transform.position) <= TargetDist;
	private bool WithinTargetMargin => Vector2.Distance(actor.transform.position, target.transform.position) <= TargetDist + BufferRadius;
}
