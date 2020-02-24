using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScavengeForWoodBehaviour : IAiBehaviour
{
	const float searchRadius = 20f;
	const float treeHarvestTimeout = 60f;
	const float navTimeout = 60f;
	const int randomWalkSteps = 20;
	const float randomWalkTimeout = 20f;

	NPC npc;
	GameObject currentTargetTree;
	Coroutine currentScavengeLoopCoroutine;
	Coroutine currentHarvestSubroutine;
	IAiBehaviour navSubBehaviour;
	IAiBehaviour harvestSubBehaviour;
	IAiBehaviour randomMoveSubBehaviour;

	public bool IsRunning { get; private set; } = false;
	public void Cancel()
	{
		if (currentHarvestSubroutine != null) npc.StopCoroutine(currentHarvestSubroutine);
		if (currentScavengeLoopCoroutine != null) npc.StopCoroutine(currentScavengeLoopCoroutine);
		navSubBehaviour?.Cancel();
		harvestSubBehaviour?.Cancel();
		randomMoveSubBehaviour?.Cancel();
		IsRunning = false;
	}
	public void Execute()
	{
		currentScavengeLoopCoroutine = npc.StartCoroutine(ScavengeLoopCoroutine());
		IsRunning = true;
	}

	public ScavengeForWoodBehaviour (NPC npc)
	{
		this.npc = npc;
	}


	IEnumerator ScavengeLoopCoroutine()
	{
		while (true)
		{
			// Pause for a bit before walking again
			yield return new WaitForSeconds(Random.Range(1f, 3f));

			// Choose a new target tree to cut
			Vector2Int discoveredTreeLocation = new Vector2Int();
			currentTargetTree = NearbyObjectLocaterSystem.FindClosestEntityWithComponent<BreakableTree>(npc.transform.position, searchRadius, npc.CurrentScene, out discoveredTreeLocation);

			// If no nearby tree was found
			if (currentTargetTree == null)
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
			currentHarvestSubroutine = npc.StartCoroutine(FindAndHarvestTreeSubroutine((bool success) => { harvestDidFinish = true; harvestDidSucceed = success; }));

			while (!harvestDidFinish)
			{
				yield return null;
			}
		}
	}

	// The process for locating, navigating to, and cutting, a single tree
	IEnumerator FindAndHarvestTreeSubroutine (NPCBehaviourExecutor.ExecutionCallbackFailable callback)
	{
		if (currentTargetTree == null)
		{
			callback(false);
			yield break;
		}

		bool navDidFinish = false;
		bool navDidSucceed = false;

		navSubBehaviour = new NavigateNextToObjectBehaviour(npc, currentTargetTree, npc.CurrentScene, (bool success) => { navDidFinish = true; navDidSucceed = success; });
		navSubBehaviour.Execute();

		// Wait for navigation to complete 
		float navStartTime = Time.time;
		while (navDidFinish == false)
		{
			if (Time.time - navStartTime >= navTimeout)
			{
				Debug.Log("Timed out navigating to tree.");
				callback(false);
				yield break;
			}
			yield return null;
		}

		if (!navDidSucceed || currentTargetTree == null)
		{
			callback(false);
			yield break;
		}

		// Turn to face the tree
		Direction direction = (currentTargetTree.transform.position.ToVector2() - npc.transform.position.ToVector2()).ToDirection();
		npc.Navigator.ForceDirection(direction);

		BreakableTree breakableTree = currentTargetTree.GetComponent<BreakableTree>();
		if (breakableTree == null)
		{
			Debug.Log("Tree to harvest is not a tree!");
			callback(false);
			yield break;
		}

		bool harvestDidFinish = false;
		harvestSubBehaviour = new HarvestTreeBehaviour(npc, breakableTree, (bool success) => { harvestDidFinish = true; });
		harvestSubBehaviour.Execute();

		// Wait for harvest to finish
		float harvestStartTime = Time.time;
		while (harvestDidFinish == false)
		{
			if (Time.time - harvestStartTime >= treeHarvestTimeout)
			{
				Debug.Log("Tree harvest timed out.");
				callback(false);
				yield break;
			}
			yield return null;
		}
		callback(true);
	}
}
