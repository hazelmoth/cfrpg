using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatSomethingBehaviour : IAiBehaviour
{
	NPC npc;
	Coroutine eatCoroutine;
	NPCActivityExecutor.ExecutionCallbackFailable callback;

	public bool IsRunning { get; private set; }

	public void Cancel()
	{
		if (eatCoroutine != null)
			npc.StopCoroutine(eatCoroutine);
		IsRunning = false;
		callback?.Invoke(false);
	}
	public void Execute()
	{
		IsRunning = true;
		eatCoroutine = npc.StartCoroutine(EatSomethingCoroutine());
	}
	public EatSomethingBehaviour (NPC npc, NPCActivityExecutor.ExecutionCallbackFailable callback)
	{
		this.npc = npc;
		this.callback = callback;
	}

	IEnumerator EatSomethingCoroutine()
	{
		foreach (Item item in npc.Inventory.GetAllItems())
		{
			if (item != null && item.IsEdible)
			{
				Debug.Log(npc.ActorId + " is eating a " + item);

				yield return new WaitForSeconds(2f);

				ActorEatingSystem.AttemptEat(npc, item);
				bool didRemove = npc.Inventory.RemoveOneInstanceOf(item);
				if (!didRemove)
				{
					Debug.LogWarning("Item removal upon eating failed.");
				}
				IsRunning = false;
				callback?.Invoke(true);
				yield break;
			}
		}
		Debug.Log(npc.ActorId + " tried to eat but has no food!");
		IsRunning = false;
		callback?.Invoke(false);
	}
}
