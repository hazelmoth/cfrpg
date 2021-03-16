using System.Collections;
using UnityEngine;

namespace Behaviours
{
	public class EatSomethingBehaviour : IAiBehaviour
	{
		private Actor Actor;
		private Coroutine eatCoroutine;
		private ActorBehaviourExecutor.ExecutionCallbackFailable callback;

		public bool IsRunning { get; private set; }

		public void Cancel()
		{
			if (eatCoroutine != null)
				Actor.StopCoroutine(eatCoroutine);
			IsRunning = false;
			callback?.Invoke(false);
		}
		public void Execute()
		{
			IsRunning = true;
			eatCoroutine = Actor.StartCoroutine(EatSomethingCoroutine());
		}
		public EatSomethingBehaviour (Actor Actor, ActorBehaviourExecutor.ExecutionCallbackFailable callback)
		{
			this.Actor = Actor;
			this.callback = callback;
		}

		private IEnumerator EatSomethingCoroutine()
		{
			foreach (ItemStack item in Actor.GetData().Inventory.GetAllItems())
			{
				if (item != null && item.GetData().IsEdible)
				{
					Debug.Log(Actor.ActorId + " is eating a " + item);

					yield return new WaitForSeconds(2f);

					ActorEatingSystem.AttemptEat(Actor, item);
					bool didRemove = Actor.GetData().Inventory.RemoveOneInstanceOf(item.id);
					if (!didRemove)
					{
						Debug.LogWarning("Item removal upon eating failed.");
					}
					IsRunning = false;
					callback?.Invoke(true);
					yield break;
				}
			}
			Debug.Log(Actor.ActorId + " tried to eat but has no food!");
			IsRunning = false;
			callback?.Invoke(false);
		}
	}
}
