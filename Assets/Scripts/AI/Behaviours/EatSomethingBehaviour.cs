using System.Collections;
using Items;
using UnityEngine;

namespace AI.Behaviours
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
				if (item != null && item.GetData() is IEdible)
				{
					Debug.Log(Actor.ActorId + " is eating a " + item);

					yield return new WaitForSeconds(2f);

					bool ate = ActorEatingSystem.AttemptEat(Actor, item.GetData());
					if (ate)
					{
						Actor.GetData().Inventory.RemoveOneInstanceOf(item.Id);
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
