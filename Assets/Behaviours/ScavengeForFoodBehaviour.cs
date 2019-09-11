using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScavengeForFoodBehaviour : IAiBehaviour
{
	const float searchRadius = 15f;
	const float harvestTimeout = 10f;
	const float navTimeout = 30f;
	const int randomWalkSteps = 20;
	const float randomWalkTimeout = 20f;

	NPC npc;
	Coroutine scavengeLoopCoroutine;
	Coroutine harvestSubroutine;
	GameObject targetPlantObject = null;
	IAiBehaviour navSubBehaviour;
	IAiBehaviour harvestSubBehaviour;
	IAiBehaviour randomMoveSubBehaviour;

	public bool IsRunning {get; private set;}

	public void Cancel()
	{
		if (harvestSubroutine != null) npc.StopCoroutine(harvestSubroutine);
		if (scavengeLoopCoroutine != null) npc.StopCoroutine(scavengeLoopCoroutine);
		navSubBehaviour?.Cancel();
		harvestSubBehaviour?.Cancel();
		randomMoveSubBehaviour?.Cancel();
		IsRunning = false;
	}

	public void Execute()
	{
		scavengeLoopCoroutine = npc.StartCoroutine(ScavengeLoopCoroutine());
		IsRunning = true;
	}

	public ScavengeForFoodBehaviour(NPC npc)
	{
		this.npc = npc;
		IsRunning = false;
	}

	IEnumerator ScavengeLoopCoroutine()
	{
		while (true)
		{
			// Pause for a bit before walking again
			yield return new WaitForSeconds(Random.Range(1f, 3f));

			// Choose a new target plant
			Vector2Int discoveredPlantLocation = new Vector2Int();
			targetPlantObject = NearbyObjectLocaterSystem.FindClosestEntityWithComponent<HarvestablePlant>(npc.transform.position, searchRadius, npc.CurrentScene, out discoveredPlantLocation);

			// If no nearby plant was found
			if (targetPlantObject == null)
			{
				bool randomWalkFinished = false;
				randomMoveSubBehaviour = new MoveRandomlyBehaviour(npc, randomWalkSteps, () => { randomWalkFinished = true; });
				randomMoveSubBehaviour.Execute();

				float randomWalkStartTime = Time.time;
				while (!randomWalkFinished)
				{
					if (Time.time - randomWalkStartTime >= randomWalkTimeout)
					{
						randomMoveSubBehaviour.Cancel();
						break;
					}
					yield return null;
				}
				continue;
			}

			bool harvestDidFinish = false;
			bool harvestDidSucceed = false;
			harvestSubroutine = npc.StartCoroutine(FindAndHarvestPlantSubroutine((bool success) => { harvestDidFinish = true; harvestDidSucceed = success; }));


			while (!harvestDidFinish)
			{
				yield return null;
			}
		}
	}

	IEnumerator FindAndHarvestPlantSubroutine(NPCActivityExecutor.ExecutionCallbackFailable localCallback)
	{
		bool navDidFinish = false;
		bool navDidSucceed = false;
		navSubBehaviour = new NavigateNextToObjectBehaviour(npc, targetPlantObject, npc.CurrentScene, (bool success) => { navDidFinish = true; navDidSucceed = success; });
		navSubBehaviour.Execute();

		// Wait for navigation
		float navStartTime = Time.time;
		while (navDidFinish == false)
		{
			if (Time.time - navStartTime >= navTimeout)
			{
				Debug.Log("Timed out navigating to plant.");
				localCallback(false);
				yield break;
			}
			yield return null;
		}

		if (targetPlantObject == null || !navDidSucceed)
		{
			localCallback(false);
			yield break;
		}

		Direction direction = (targetPlantObject.transform.position.ToVector2() - npc.transform.position.ToVector2()).ToDirection();
		npc.Navigator.ForceDirection(direction);

		yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));

		if (targetPlantObject == null)
		{
			localCallback(false);
			yield break;
		}

		HarvestablePlant targetPlant = targetPlantObject.GetComponent<HarvestablePlant>();
		if (targetPlant == null)
		{
			Debug.LogError("Tried to harvest a plant that's...not a plant?");
			localCallback(false);
			yield break;
		}

		bool harvestDidFinish = false;
		bool harvestDidSucceed = false;
		harvestSubBehaviour = new HarvestPlantBehaviour(npc, targetPlant, (bool success) => { harvestDidFinish = true; harvestDidSucceed = success; });
		harvestSubBehaviour.Execute();

		float harvestStartTime = Time.time;
		while (!harvestDidFinish)
		{
			if (Time.time - harvestStartTime >= harvestTimeout)
			{
				Debug.Log("Timed out harvesting plant.");
				localCallback(false);
				yield break;
			}
			yield return null;
		}
		localCallback(true);
	}
}
