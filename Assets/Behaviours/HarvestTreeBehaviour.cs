using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Has the NPC destroy a nearby tree and pick up the items it drops
public class HarvestTreeBehaviour : IAiBehaviour
{
	const float breakTreeTimeout = 20f;

	NPC npc;
	BreakableTree targetTree;
	NPCActivityExecutor.ExecutionCallbackFailable callback;

	Coroutine runningCoroutine = null;
	IAiBehaviour breakObjectSubBehaviour = null;

	bool isRunning = false;

	public bool IsRunning => isRunning;

	public void Cancel()
	{
		if (runningCoroutine != null)
			npc.StopCoroutine(runningCoroutine);
		if (breakObjectSubBehaviour != null)
			breakObjectSubBehaviour.Cancel();
		isRunning = false;
		callback?.Invoke(false);
	}

	public void Execute()
	{
		npc.StartCoroutine(HarvestTreeCoroutine());
		isRunning = true;
	}

	public HarvestTreeBehaviour (NPC npc, BreakableTree tree, NPCActivityExecutor.ExecutionCallbackFailable callback)
	{
		this.npc = npc;
		this.targetTree = tree;
		this.callback = callback;
	}

	IEnumerator HarvestTreeCoroutine()
	{
		BreakableObject breakable = targetTree.GetComponent<BreakableObject>();
		bool finishedBreaking = false;
		bool didSucceed = false;
		List<DroppedItem> returnedItems = null;

		if (breakable == null)
		{
			Debug.LogWarning("Tried to harvest a tree that doesn't have a BreakableObject component!");
			isRunning = false;
			callback.Invoke(false);
			yield break;
		}

		breakObjectSubBehaviour = new DestroyBreakableObjectBehaviour(
			npc,
			breakable,
			(bool success, List<DroppedItem> items) => { finishedBreaking = true; didSucceed = success; returnedItems = items; }
		);

		breakObjectSubBehaviour.Execute();

		// wait for object breaking behaviour to finish
		float treeBreakStartTime = Time.time;
		while (finishedBreaking == false)
		{
			yield return null;
			if (Time.time - treeBreakStartTime > breakTreeTimeout)
			{
				Debug.Log("Tree break timeout exceeded. Cancelling tree harvest.");
				breakObjectSubBehaviour.Cancel();
				isRunning = false;
				callback.Invoke(false);
				yield break;
			}
		}
		Debug.Log("Destruction complete");

		if (!didSucceed)
		{
			Debug.Log("Object breaking failed. Cancelling tree harvest.");
			Cancel();
		}

		yield return new WaitForSeconds(0.5f);

		for (int i = 0; i < returnedItems.Count; i++)
		{
			if (returnedItems[i] != null && npc.Inventory.AttemptAddItemToInv(ItemManager.GetItemById(returnedItems[i].ItemId)))
			{
				yield return new WaitForSeconds(0.5f);
				GameObject.Destroy(returnedItems[i].gameObject);
			}
		}

		isRunning = false;
		callback?.Invoke(true);
	}
}
