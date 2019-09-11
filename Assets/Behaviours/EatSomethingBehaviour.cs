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
		callback(false);
	}
	public void Execute()
	{
		eatCoroutine = npc.StartCoroutine(EatSomethingCoroutine());
		IsRunning = true;
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
				Debug.Log(npc.NpcId + " is eating a " + item);

				yield return new WaitForSeconds(2f);

				ActorEatingSystem.AttemptEat(npc, item);
				npc.Inventory.RemoveOneInstanceOf(item);
				IsRunning = false;
				callback(true);
				yield break;
			}
		}
		Debug.Log(npc.NpcId + " tried to eat but has no food!");
		IsRunning = false;
		callback(false);
	}
}
