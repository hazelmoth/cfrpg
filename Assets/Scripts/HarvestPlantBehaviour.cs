using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestPlantBehaviour : IAiBehaviour
{
	private Actor Actor;
	private HarvestablePlant targetPlant;
	private ActorBehaviourExecutor.ExecutionCallbackFailable callback;
	private Coroutine harvestCoroutine;

	public bool IsRunning { get; private set; }

	public void Cancel()
	{
		if (harvestCoroutine != null)
			Actor.StopCoroutine(harvestCoroutine);
		IsRunning = false;
		callback(false);
	}

	public void Execute()
	{
		Actor.StartCoroutine(HarvestPlantCoroutine());
		IsRunning = true;
	}

	public HarvestPlantBehaviour(Actor Actor, HarvestablePlant targetPlant, ActorBehaviourExecutor.ExecutionCallbackFailable callback)
	{
		this.Actor = Actor;
		this.targetPlant = targetPlant;
		this.callback = callback;
		IsRunning = false;
	}

	private IEnumerator HarvestPlantCoroutine()
	{
		DroppedItem item = null;
		if (targetPlant != null)
		{
			targetPlant.Harvest(out item);
		}

		// Wait a bit before picking up the item
		yield return new WaitForSeconds(0.5f);

		if (item != null && Actor.GetData().Inventory.AttemptAddItem(item.Item))
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
