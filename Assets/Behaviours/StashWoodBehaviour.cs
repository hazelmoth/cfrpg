using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StashWoodBehaviour : IAiBehaviour
{
	const float searchRadius = 20f;

	NPC npc;
	NPCBehaviourExecutor.ExecutionCallbackFailable callback;
	Coroutine activeCoroutine;
	IAiBehaviour navSubBehaviour;

	public bool IsRunning { get; private set; }
	public void Cancel()
	{
		if (activeCoroutine != null)
		{
			npc.StopCoroutine(activeCoroutine);
		}
		IsRunning = false;
		callback?.Invoke(false);
	}
	public void Execute()
	{
		activeCoroutine = npc.StartCoroutine(StashWoodCoroutine());
		IsRunning = true;
	}
	public StashWoodBehaviour(NPC npc, NPCBehaviourExecutor.ExecutionCallbackFailable callback)
	{
		this.npc = npc;
		this.callback = callback;
	}

	IEnumerator StashWoodCoroutine()
	{
		WoodPile woodPile = null;
		List<Vector2Int> knownLocations = npc.Memories.GetLocationsOfEntity("woodpile");

		// Remove any known woodpiles that are full
		for (int i = knownLocations.Count - 1; i >= 0; i--)
		{
			WoodPile pileToCheck = WorldMapManager.GetEntityObjectAtPoint(knownLocations[i], npc.CurrentScene).GetComponent<WoodPile>();
			if (pileToCheck.IsFull)
			{
				knownLocations.RemoveAt(i);
			}
		}
		if (knownLocations.Count > 0)
		{
			// Find the closest object in the list
			Vector2Int dest = npc.transform.position.ToVector2Int().ClosestFromList(knownLocations);
			GameObject woodpileObject = WorldMapManager.GetEntityObjectAtPoint(dest, npc.CurrentScene);
			woodPile = woodpileObject.GetComponent<WoodPile>();
		}
		else
		{
			// If we don't know any woodpile locations then see if there's one nearby
			GameObject foundObject = NearbyObjectLocaterSystem.FindClosestEntityWithComponent<WoodPile>(npc.transform.position.ToVector2(), searchRadius, npc.CurrentScene);
			if (foundObject != null)
			{
				woodPile = foundObject.GetComponent<WoodPile>();
			}
		}

		if (woodPile == null)
		{
			Cancel();
			yield break;
		}

		bool navDidFinish = false;
		bool navDidSucceed = false;
		navSubBehaviour = new NavigateNextToObjectBehaviour(npc, woodPile.gameObject, SceneObjectManager.GetSceneIdForObject(woodPile.gameObject), (bool success) => { navDidFinish = true; navDidSucceed = success; });
		navSubBehaviour.Execute();

		while (!navDidFinish)
		{
			yield return null;
		}
		if (navDidSucceed)
		{
			npc.Inventory.TransferMatchingItemsToContainer("log", woodPile);
			callback?.Invoke(true);
		}
		else
		{
			callback?.Invoke(false);
		}
		IsRunning = false;
	}
}
