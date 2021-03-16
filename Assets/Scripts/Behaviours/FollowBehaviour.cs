using System.Collections;
using UnityEngine;

namespace Behaviours
{
	public class FollowBehaviour : IAiBehaviour
	{
		private const float DefaultTargetDist = 2f;
		private const float DefaultBuffer = 0.5f;
		private float targetDist; // The minimum distance at which we'll move towards the target
		private float buffer; // The distance to the target zone within which we won't bother navigating

		private IAiBehaviour navBehaviour;
		private Coroutine routine;
		public readonly Actor actor;
		public readonly Actor target;
		private bool isRunning;

		public FollowBehaviour(Actor actor, Actor target)
		{
			this.actor = actor;
			this.target = target;
			targetDist = DefaultTargetDist;
			buffer = DefaultBuffer;
		}
		public FollowBehaviour(Actor actor, Actor target, float targetDist, float buffer)
		{
			this.actor = actor;
			this.target = target;
			this.targetDist = targetDist;
			this.buffer = buffer;
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
					navBehaviour = new NavigateBehaviour((Actor)actor, targetLocation,
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
							actor.GetComponent<ActorAnimController>().SetDirection((targetLocation.Vector2 - actor.transform.position.ToVector2()).ToDirection());
							break;
						}
						yield return null;
					}
				}

				yield return null;
			}
		}

		private bool NearTarget => Vector2.Distance(actor.transform.position, target.transform.position) <= targetDist;
		private bool WithinTargetMargin
		{
			get
			{
				if (actor.CurrentScene != target.CurrentScene) return false;
				return Vector2.Distance(actor.transform.position, target.transform.position) <= targetDist + buffer;
			} }
	}
}
