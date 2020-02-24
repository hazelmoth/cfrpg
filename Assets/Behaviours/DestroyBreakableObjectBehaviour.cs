using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Assumes that we're already next to the object to be destroyed
public class DestroyBreakableObjectBehaviour : IAiBehaviour
{
	const float breakTimeout = 60f;

	NPC npc;
	BreakableObject target;
	ActorPunchExecutor puncher;
	NPCBehaviourExecutor.ExecutionCallbackDroppedItemsFailable callback;

	Coroutine runningCoroutine = null;

	public bool IsRunning { get; private set; } = false;
	public void Cancel()
	{
		if (runningCoroutine != null)
			npc.StopCoroutine(runningCoroutine);
		IsRunning = false;
		callback?.Invoke(false, null);
	}
	public void Execute()
	{
		runningCoroutine = npc.StartCoroutine(DestroyBreakableObjectCoroutine());
		IsRunning = true;
	}

	public DestroyBreakableObjectBehaviour(NPC npc, BreakableObject target, NPCBehaviourExecutor.ExecutionCallbackDroppedItemsFailable callback)
	{
		this.npc = npc;
		this.target = target;
		puncher = npc.GetComponent<ActorPunchExecutor>();
		this.callback = callback;
	}

	IEnumerator DestroyBreakableObjectCoroutine()
	{
		if (puncher == null && target != null)
		{
			puncher = npc.GetComponent<ActorPunchExecutor>();
			if (puncher == null)
				puncher = npc.gameObject.AddComponent<ActorPunchExecutor>();
		}
		
		Vector2 punchDir = (npc.transform.position.ToVector2() - target.transform.position.ToVector2()).ToDirection().Invert().ToVector2();

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
