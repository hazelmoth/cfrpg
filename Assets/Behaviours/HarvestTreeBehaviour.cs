using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Has the Actor destroy a nearby tree and pick up the items it drops
public class HarvestTreeBehaviour : IAiBehaviour
{
	private const float breakTreeTimeout = 20f;

	private Actor Actor;
	private BreakableTree targetTree;
	private ActorBehaviourExecutor.ExecutionCallbackFailable callback;

	private Coroutine runningCoroutine = null;
	private IAiBehaviour breakObjectSubBehaviour = null;

	private bool isRunning = false;

	public bool IsRunning => isRunning;

	public void Cancel()
	{
		if (runningCoroutine != null)
			Actor.StopCoroutine(runningCoroutine);
		breakObjectSubBehaviour?.Cancel();
		isRunning = false;
		callback?.Invoke(false);
	}

	public void Execute()
	{
		Actor.StartCoroutine(HarvestTreeCoroutine());
		isRunning = true;
	}

	public HarvestTreeBehaviour (Actor Actor, BreakableTree tree, ActorBehaviourExecutor.ExecutionCallbackFailable callback)
	{
		this.Actor = Actor;
		this.targetTree = tree;
		this.callback = callback;
	}

	private IEnumerator HarvestTreeCoroutine()
	{
		BreakableObject breakable = targetTree.GetComponent<BreakableObject>();
		bool finishedBreaking = false;
		bool didSucceed = false;
		List<DroppedItem> returnedItems = new List<DroppedItem>();

		if (breakable == null)
		{
			Debug.LogWarning("Tried to harvest a tree that doesn't have a BreakableObject component!");
			isRunning = false;
			callback.Invoke(false);
			yield break;
		}

		breakObjectSubBehaviour = new DestroyBreakableObjectBehaviour(
			Actor,
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

		if (returnedItems != null)
		{
			for (int i = 0; i < returnedItems.Count; i++)
			{
				yield return new WaitForSeconds(0.5f);

				if (returnedItems == null) break;

				if (returnedItems[i] != null &&
					Actor.GetData().Inventory.AttemptAddItemToInv(ContentLibrary.Instance.Items.Get(returnedItems[i].ItemId)))
				{
					GameObject.Destroy(returnedItems[i].gameObject);
				}
			}
		}

		isRunning = false;
		callback?.Invoke(true);
	}
}
