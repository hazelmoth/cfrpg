using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestPlantBehaviour : IAiBehaviour
{
	NPC npc;
	HarvestablePlant targetPlant;
	NPCActivityExecutor.ExecutionCallbackFailable callback;
	Coroutine harvestCoroutine;

	public bool IsRunning { get; private set; }

	public void Cancel()
	{
		if (harvestCoroutine != null)
			npc.StopCoroutine(harvestCoroutine);
		IsRunning = false;
		callback(false);
	}

	public void Execute()
	{
		npc.StartCoroutine(HarvestPlantCoroutine());
		IsRunning = true;
	}

	public HarvestPlantBehaviour(NPC npc, HarvestablePlant targetPlant, NPCActivityExecutor.ExecutionCallbackFailable callback)
	{
		this.npc = npc;
		this.targetPlant = targetPlant;
		this.callback = callback;
		IsRunning = false;
	}

	IEnumerator HarvestPlantCoroutine()
	{
		DroppedItem item = null;
		if (targetPlant != null)
			targetPlant.Harvest(out item);

		// Wait a bit before picking up the item
		yield return new WaitForSeconds(0.5f);

		if (item != null && npc.Inventory.AttemptAddItemToInv(ContentLibrary.Instance.Items.GetItemById(item.ItemId)))
		{
			GameObject.Destroy(item.gameObject);
			callback?.Invoke(true);
		}
		else
		{
			callback?.Invoke(false);
		}

		IsRunning = false;
	}
}
