using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Assumes that we're already next to the object to be destroyed
namespace Behaviours
{
	public class DestroyBreakableObjectBehaviour : IAiBehaviour
	{
		private const float breakTimeout = 60f;

		private Actor Actor;
		private BreakableObject target;
		private ActorPunchExecutor puncher;
		private ActorBehaviourExecutor.ExecutionCallbackDroppedItemsFailable callback;

		private Coroutine runningCoroutine = null;

		public bool IsRunning { get; private set; } = false;
		public void Cancel()
		{
			if (runningCoroutine != null)
				Actor.StopCoroutine(runningCoroutine);
			IsRunning = false;
			callback?.Invoke(false, null);
		}
		public void Execute()
		{
			runningCoroutine = Actor.StartCoroutine(DestroyBreakableObjectCoroutine());
			IsRunning = true;
		}

		public DestroyBreakableObjectBehaviour(Actor Actor, BreakableObject target, ActorBehaviourExecutor.ExecutionCallbackDroppedItemsFailable callback)
		{
			this.Actor = Actor;
			this.target = target;
			puncher = Actor.GetComponent<ActorPunchExecutor>();
			this.callback = callback;
		}

		private IEnumerator DestroyBreakableObjectCoroutine()
		{
			if (puncher == null && target != null)
			{
				puncher = Actor.GetComponent<ActorPunchExecutor>();
				if (puncher == null)
					puncher = Actor.gameObject.AddComponent<ActorPunchExecutor>();
			}
		
			Vector2 punchDir = (Actor.transform.position.ToVector2() - target.transform.position.ToVector2()).ToDirection().Invert().ToVector2();

			target.OnDropItems += OnItemsDropped;
			bool itemsDidDrop = false;
			float punchingStartTime = Time.time;
			while (itemsDidDrop == false)
			{
				if (Time.time - punchingStartTime > breakTimeout)
				{
					Debug.Log("Break timeout exceeded. Cancelling.");
					Cancel();
				}
				puncher.InitiatePunch(punchDir);
				yield return null;
			}

			void OnItemsDropped(List<DroppedItem> items)
			{
				itemsDidDrop = true;
				callback?.Invoke(true, items);
			}

			IsRunning = false;
			yield break;
		}
	}
}
