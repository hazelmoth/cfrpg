using System;
using System.Collections.Generic;
using System.Linq;
using Behaviours;
using UnityEngine;

// Manages the top level behaviour that an actor is currently running, and exposes
// functions to change it.
public class ActorBehaviourExecutor : MonoBehaviour {

	public delegate void ExecutionCallbackFailable(bool didSucceed);
	public delegate void ExecutionCallbackDroppedItemsFailable(bool didSucceed, List<DroppedItem> items);

	private Actor actor;
	private IAiBehaviour currentBehaviour;

	public string CurrentBehaviourName => currentBehaviour != null ? currentBehaviour.GetType().Name : "null";
	public IAiBehaviour CurrentBehaviour => currentBehaviour;

	private void Awake () {
		actor = this.GetComponent<Actor> ();
	}

	private void Update()
	{
		if (currentBehaviour != null && actor.GetData().PhysicalCondition.IsDead)
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

		Debug.Assert(currentBehaviour == null || currentBehaviour.IsRunning == false, "Cancelled behaviour is still running!", actor);

		currentBehaviour = (IAiBehaviour)Activator.CreateInstance(behaviourType, args);
		currentBehaviour.Execute();

		Debug.Assert(currentBehaviour.IsRunning, "Executed behaviour isn't running!", actor);
	}
}