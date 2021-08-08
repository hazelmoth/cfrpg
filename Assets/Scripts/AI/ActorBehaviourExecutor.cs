using System;
using System.Collections.Generic;
using System.Linq;
using AI.Behaviours;
using AI.Trees;
using AI.Trees.Nodes;
using UnityEngine;

// Manages the top level behaviour that an actor is currently running, and exposes
// functions to change it.
namespace AI
{
	public class ActorBehaviourExecutor : MonoBehaviour {

		public delegate void ExecutionCallbackFailable(bool didSucceed);
		public delegate void ExecutionCallbackDroppedItemsFailable(bool didSucceed, List<DroppedItem> items);

		private Actor actor;
		private Node currentBehaviourTree;
		public string CurrentBehaviourName => CurrentTask != null ? CurrentTask.nodeType.Name : "null";
		public Task CurrentTask { get; private set; }

		private void Awake () {
			actor = this.GetComponent<Actor> ();
		}

		private void Update()
		{
			if (PauseManager.Paused) return;
			
			if (CurrentTask != null && actor.GetData().Health.IsDead)
			{
				CancelTasks();
			}

			currentBehaviourTree?.Update();
		}

		public void CancelTasks ()
		{
			if (currentBehaviourTree != null && !currentBehaviourTree.Stopped) currentBehaviourTree.Cancel();
			currentBehaviourTree = null;
			CurrentTask = null;
		}

		// Constructs a Node for the given Task with the given args runs it on
		// this actor, if an identical Task is not already running. Stops any
		// running behaviour if given Task is null.
		public void Execute(Task behaviourTask)
		{
			// Consider tasks of different types as different.
			if (CurrentTask != null && behaviourTask != null && CurrentTask.nodeType == behaviourTask.nodeType)
			{
				// Compare argument lists.
				if (behaviourTask.args.Length == CurrentTask.args.Length &&
				    !behaviourTask.args.Where((t, i) => !t.Equals(CurrentTask.args[i])).Any())
				{
					// This is the same as the task we're already running.
					return;
				}
			}

			CurrentTask = behaviourTask;
			currentBehaviourTree = behaviourTask?.CreateNode();
		}
	}
}