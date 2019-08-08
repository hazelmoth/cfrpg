using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls what the NPC does, and keeps track of what it's doing.
public class NPCBehaviourAI : MonoBehaviour
{
	public enum Activity {
		None,
		Eat,
		ScavengeForFood,
		ScavengeForWood,
		StashWood,
		Wander
	}

	private Actor actor;
	private ActorPhysicalCondition actorCondition;
	private NPCActivityExecutor executor;
	private NPCTaskList taskList;

    // Update is called once per frame
    void Update()
    {
		ExecuteActivity (EvaluateBehaviour());
    }

	// TODO instead of evaluating sequentially, weight and compare possible activities numerically
	Activity EvaluateBehaviour () 
	{
		if (actor == null) {
			actor = GetComponent<Actor> ();
		}
		if (actorCondition == null) {
			actorCondition = GetComponent<ActorPhysicalCondition> ();
		}
		if (executor == null) {
			executor = GetComponent<NPCActivityExecutor> ();
		}
		if (taskList == null)
		{
			taskList = GetComponent<NPCTaskList>();
		}
		Activity nextActivity = Activity.None;


		// Start by checking for critical needs
		if (actorCondition != null && actorCondition.CurrentNutrition < 0.3f) {
			// Check if the actor has any food
			foreach (Item item in actor.Inventory.GetAllItems()) 
			{
				if (item != null && item.IsEdible) {
					nextActivity = Activity.Eat;
					break;
				}
			}
			// If we don't have food, go look for some
			if (executor.CurrentActivity != Activity.ScavengeForFood)
				nextActivity = Activity.ScavengeForFood;
		}
		else if (taskList.Tasks.Count > 0)
		{
			// Find and execute the most recently assigned task
			NPCTaskList.AssignedTask mostRecentTask = taskList.Tasks[0];
			for (int i = 1; i < taskList.Tasks.Count; i++)
			{
				if (taskList.Tasks[i].timeAssigned < mostRecentTask.timeAssigned)
					mostRecentTask = taskList.Tasks[i];
			}
			nextActivity = mostRecentTask.task.activity;
		}

		if (nextActivity == Activity.ScavengeForWood && actor.Inventory.IsFull(includeApparelSlots: false))
		{
			nextActivity = Activity.StashWood;
		}

		// Wander if we have nothing else to do
		if (nextActivity == Activity.None && executor.CurrentActivity == Activity.None) {
			nextActivity = Activity.Wander;
		}

		return nextActivity;
	}

	private void ExecuteActivity (Activity activity) {
		if (executor == null) {
			executor = GetComponent<NPCActivityExecutor> ();
		}
		switch (activity) 
		{
		case Activity.Eat:
			executor.Execute_EatSomething ();
			break;
		case Activity.Wander:
			executor.Execute_Wander();
			break;
		case Activity.ScavengeForFood:
			executor.Execute_ScavengeForFood ();
			break;
		case Activity.ScavengeForWood:
			executor.Execute_ScavengeForWood();
			break;
		case Activity.StashWood:
			executor.Execute_StashWood();
			break;
		// If there's nothing to do, keep doing whatever it is we were doing
		case Activity.None:
		default:
			break;
		}
	}
}
