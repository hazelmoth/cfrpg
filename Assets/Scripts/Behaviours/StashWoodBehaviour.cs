using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviours
{
	public class StashWoodBehaviour : IAiBehaviour
	{
		private const float searchRadius = 20f;

		private Actor Actor;
		private ActorBehaviourExecutor.ExecutionCallbackFailable callback;
		private Coroutine activeCoroutine;
		private IAiBehaviour navSubBehaviour;

		public bool IsRunning { get; private set; }
		public void Cancel()
		{
			if (activeCoroutine != null)
			{
				Actor.StopCoroutine(activeCoroutine);
			}
			IsRunning = false;
			callback?.Invoke(false);
		}
		public void Execute()
		{
			activeCoroutine = Actor.StartCoroutine(StashWoodCoroutine());
			IsRunning = true;
		}
		public StashWoodBehaviour(Actor Actor, ActorBehaviourExecutor.ExecutionCallbackFailable callback)
		{
			this.Actor = Actor;
			this.callback = callback;
		}

		private IEnumerator StashWoodCoroutine()
		{
			WoodPile woodPile = null;
			List<Vector2Int> knownLocations = Actor.GetData().Memories.GetLocationsOfEntity("woodpile");

			// Remove any known woodpiles that are full
			for (int i = knownLocations.Count - 1; i >= 0; i--)
			{
				WoodPile pileToCheck = RegionMapManager.GetEntityObjectAtPoint(knownLocations[i], Actor.CurrentScene).GetComponent<WoodPile>();
				if (pileToCheck.IsFull)
				{
					knownLocations.RemoveAt(i);
				}
			}
			if (knownLocations.Count > 0)
			{
				// Find the closest object in the list
				Vector2Int dest = Actor.transform.position.ToVector2Int().ClosestFromList(knownLocations);
				GameObject woodpileObject = RegionMapManager.GetEntityObjectAtPoint(dest, Actor.CurrentScene);
				woodPile = woodpileObject.GetComponent<WoodPile>();
			}
			else
			{
				// If we don't know any woodpile locations then see if there's one nearby
				GameObject foundObject = NearbyObjectLocaterSystem.FindClosestEntityWithComponent<WoodPile>(Actor.transform.position.ToVector2(), searchRadius, Actor.CurrentScene);
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
			navSubBehaviour = new NavigateNextToObjectBehaviour(Actor, woodPile.gameObject, SceneObjectManager.GetSceneIdForObject(woodPile.gameObject), (bool success) => { navDidFinish = true; navDidSucceed = success; });
			navSubBehaviour.Execute();

			while (!navDidFinish)
			{
				yield return null;
			}
			if (navDidSucceed)
			{
				Actor.GetData().Inventory.TransferMatchingItemsToContainer("log", woodPile);
				callback?.Invoke(true);
			}
			else
			{
				callback?.Invoke(false);
			}
			IsRunning = false;
		}
	}
}
