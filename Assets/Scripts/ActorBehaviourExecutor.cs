using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Contains functions that force an Actor to start or stop different tasks.
// These functions should be called by ActorBehaviourAi.
public class ActorBehaviourExecutor : MonoBehaviour {

	public delegate void ExecutionCallback ();
	public delegate void ExecutionCallbackFailable(bool didSucceed);
	public delegate void ExecutionCallbackDroppedItemsFailable(bool didSucceed, List<DroppedItem> items);

	private Actor Actor;

	private IAiBehaviour currentBehaviour;

	private void Awake () {
		Actor = this.GetComponent<Actor> ();
	}

	private void Update()
	{
		if (currentBehaviour != null && Actor.GetData().PhysicalCondition.IsDead)
		{
			ForceCancelBehaviours();
		}
	}

	public void ForceCancelBehaviours ()
	{
		if (currentBehaviour != null)
		{
			currentBehaviour.Cancel();
			currentBehaviour = null;
		}
	}

	// Constructs an IAIBehaviour of the given type with the given args and executes it, if an identical
	// behaviour is not already running.
	public void Execute(Type behaviourType, object[] args)
	{
		if (!behaviourType.GetInterfaces().Contains(typeof(IAiBehaviour)))
		{
			Debug.LogError("Given type does not implement IAIBehaviour! - " + behaviourType.FullName);
			return;
		}
		// TODO: consider behaviours of the same type but different argument lists as different.
		if (currentBehaviour != null && currentBehaviour.GetType() == behaviourType && currentBehaviour.IsRunning)
		{
			// A behaviour of the given type is already running. We'll ignore this call.
			return;
		}

		currentBehaviour?.Cancel();
		currentBehaviour = (IAiBehaviour)Activator.CreateInstance(behaviourType, args);
		currentBehaviour.Execute();
	}
}