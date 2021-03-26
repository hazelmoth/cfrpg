using System.Collections;
using UnityEngine;

namespace AI.Behaviours
{
	public class ScavengeForWoodBehaviour : IAiBehaviour
	{
		private const float searchRadius = 20f;
		private const float treeHarvestTimeout = 60f;
		private const float navTimeout = 60f;
		private const int randomWalkSteps = 20;
		private const float randomWalkTimeout = 20f;

		private Actor Actor;
		private GameObject currentTargetTree;
		private Coroutine currentScavengeLoopCoroutine;
		private Coroutine currentHarvestSubroutine;
		private IAiBehaviour navSubBehaviour;
		private IAiBehaviour harvestSubBehaviour;
		private IAiBehaviour randomMoveSubBehaviour;

		public bool IsRunning { get; private set; } = false;
		public void Cancel()
		{
			if (currentHarvestSubroutine != null) Actor.StopCoroutine(currentHarvestSubroutine);
			if (currentScavengeLoopCoroutine != null) Actor.StopCoroutine(currentScavengeLoopCoroutine);
			navSubBehaviour?.Cancel();
			harvestSubBehaviour?.Cancel();
			randomMoveSubBehaviour?.Cancel();
			IsRunning = false;
		}
		public void Execute()
		{
			currentScavengeLoopCoroutine = Actor.StartCoroutine(ScavengeLoopCoroutine());
			IsRunning = true;
		}

		public ScavengeForWoodBehaviour (Actor Actor)
		{
			this.Actor = Actor;
		}


		private IEnumerator ScavengeLoopCoroutine()
		{
			while (true)
			{
				// Pause for a bit before walking again
				yield return new WaitForSeconds(Random.Range(1f, 3f));

				// Choose a new target tree to cut
				Vector2Int discoveredTreeLocation = new Vector2Int();
				currentTargetTree = NearbyObjectLocaterSystem.FindClosestEntityWithComponent<BreakableTree>(Actor.transform.position, searchRadius, Actor.CurrentScene, out discoveredTreeLocation);

				// If no nearby tree was found
				if (currentTargetTree == null)
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
				currentHarvestSubroutine = Actor.StartCoroutine(FindAndHarvestTreeSubroutine((bool success) => { harvestDidFinish = true; harvestDidSucceed = success; }));

				while (!harvestDidFinish)
				{
					yield return null;
				}
			}
		}

		// The process for locating, navigating to, and cutting, a single tree
		private IEnumerator FindAndHarvestTreeSubroutine (ActorBehaviourExecutor.ExecutionCallbackFailable callback)
		{
			if (currentTargetTree == null)
			{
				callback(false);
				yield break;
			}

			bool navDidFinish = false;
			bool navDidSucceed = false;

			navSubBehaviour = new NavigateNextToObjectBehaviour(Actor, currentTargetTree, Actor.CurrentScene, (bool success) => { navDidFinish = true; navDidSucceed = success; });
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
			Direction direction = (currentTargetTree.transform.position.ToVector2() - Actor.transform.position.ToVector2()).ToDirection();
			Actor.Navigator.ForceDirection(direction);

			BreakableTree breakableTree = currentTargetTree.GetComponent<BreakableTree>();
			if (breakableTree == null)
			{
				Debug.Log("Tree to harvest is not a tree!");
				callback(false);
				yield break;
			}

			bool harvestDidFinish = false;
			harvestSubBehaviour = new HarvestTreeBehaviour(Actor, breakableTree, (bool success) => { harvestDidFinish = true; });
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
}
