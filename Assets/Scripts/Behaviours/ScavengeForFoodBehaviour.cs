using System.Collections;
using UnityEngine;

public class ScavengeForFoodBehaviour : IAiBehaviour
{
	private const float searchRadius = 15f;
	private const float harvestTimeout = 10f;
	private const float navTimeout = 30f;
	private const int randomWalkSteps = 20;
	private const float randomWalkTimeout = 20f;

	private Actor Actor;
	private Coroutine scavengeLoopCoroutine;
	private Coroutine harvestSubroutine;
	private GameObject targetPlantObject = null;
	private IAiBehaviour navSubBehaviour;
	private IAiBehaviour harvestSubBehaviour;
	private IAiBehaviour randomMoveSubBehaviour;

	public bool IsRunning {get; private set;}

	public void Cancel()
	{
		if (harvestSubroutine != null) Actor.StopCoroutine(harvestSubroutine);
		if (scavengeLoopCoroutine != null) Actor.StopCoroutine(scavengeLoopCoroutine);
		navSubBehaviour?.Cancel();
		harvestSubBehaviour?.Cancel();
		randomMoveSubBehaviour?.Cancel();
		IsRunning = false;
	}

	public void Execute()
	{
		scavengeLoopCoroutine = Actor.StartCoroutine(ScavengeLoopCoroutine());
		IsRunning = true;
	}

	public ScavengeForFoodBehaviour(Actor Actor)
	{
		this.Actor = Actor;
		IsRunning = false;
	}

	private IEnumerator ScavengeLoopCoroutine()
	{
		while (true)
		{
			// Pause for a bit before walking again
			yield return new WaitForSeconds(Random.Range(1f, 3f));

			// Choose a new target plant
			Vector2Int discoveredPlantLocation = new Vector2Int();
			targetPlantObject = NearbyObjectLocaterSystem.FindClosestEntityWithComponent<HarvestablePlant>(Actor.transform.position, searchRadius, Actor.CurrentScene, out discoveredPlantLocation);

			// If no nearby plant was found
			if (targetPlantObject == null)
			{
				bool randomWalkFinished = false;
				randomMoveSubBehaviour = new MoveRandomlyBehaviour(Actor, randomWalkSteps, () => { randomWalkFinished = true; });
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
			harvestSubroutine = Actor.StartCoroutine(FindAndHarvestPlantSubroutine((bool success) => { harvestDidFinish = true; harvestDidSucceed = success; }));


			while (!harvestDidFinish)
			{
				yield return null;
			}
		}
	}

	private IEnumerator FindAndHarvestPlantSubroutine(ActorBehaviourExecutor.ExecutionCallbackFailable localCallback)
	{
		bool navDidFinish = false;
		bool navDidSucceed = false;
		navSubBehaviour = new NavigateNextToObjectBehaviour(Actor, targetPlantObject, Actor.CurrentScene, (bool success) => { navDidFinish = true; navDidSucceed = success; });
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

		Direction direction = (targetPlantObject.transform.position.ToVector2() - Actor.transform.position.ToVector2()).ToDirection();
		Actor.Navigator.ForceDirection(direction);

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
		harvestSubBehaviour = new HarvestPlantBehaviour(Actor, targetPlant, (bool success) => { harvestDidFinish = true; harvestDidSucceed = success; });
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
