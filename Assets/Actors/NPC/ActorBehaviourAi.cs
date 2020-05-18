using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Decides what the Actor should do, and keeps track of what it's doing.
public class ActorBehaviourAi : MonoBehaviour
{
	public enum Activity {
		None,
		Accompany,
		Eat,
		ScavengeForFood,
		ScavengeForWood,
		StashWood,
		Wander
	}

	private Actor actor;
	private ActorPhysicalCondition actorCondition;
	private ActorBehaviourExecutor executor;
	private ActorTaskList taskList;

    // Update is called once per frame
    private void Update()
    {
	    if (actor == null)
	    {
		    actor = GetComponent<Actor>();
	    }
		if (actor.PlayerControlled)
	    {
		    return;
	    }

		ExecuteActivity (EvaluateBehaviour());
    }

	// TODO instead of evaluating sequentially, weight and compare possible activities numerically
	private Activity EvaluateBehaviour () 
	{
		if (actorCondition == null) {
			actorCondition = actor.GetData().PhysicalCondition;
		}
		if (executor == null) {
			executor = GetComponent<ActorBehaviourExecutor> ();
		}
		if (taskList == null)
		{
			taskList = GetComponent<ActorTaskList>();
		}

		Activity nextActivity = Activity.None;


		// Start by checking for critical needs
		if (actorCondition != null && actorCondition.CurrentNutrition < 0.3f)
		{
			bool hasFood = false;

			// Check if the actor has any food
			foreach (ItemData item in actor.GetData().Inventory.GetAllItems()) 
			{
				if (item != null && item.IsEdible) {
					hasFood = true;
					break;
				}
			}

			// If we don't have food, go look for some
			if (!hasFood && executor.CurrentActivity != Activity.ScavengeForFood)
			{
				nextActivity = Activity.ScavengeForFood;
			}
			// Otherwise eat that food
			else if (hasFood && executor.CurrentActivity != Activity.Eat)
			{
				nextActivity = Activity.Eat;
			}
		}
		else if (actor.GetData().FactionStatus.AccompanyTarget != null)
		{
			nextActivity = Activity.Accompany;
		}
		else if (taskList.Tasks.Count > 0)
		{
			// Find and execute the most recently assigned task
			ActorTaskList.AssignedTask mostRecentTask = taskList.Tasks[0];
			for (int i = 1; i < taskList.Tasks.Count; i++)
			{
				if (taskList.Tasks[i].timeAssigned < mostRecentTask.timeAssigned)
					mostRecentTask = taskList.Tasks[i];
			}
			nextActivity = mostRecentTask.task.activity;
		}

		if (nextActivity == Activity.ScavengeForWood && actor.GetData().Inventory.IsFull(includeApparelSlots: false))
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
		if (actor == null)
		{
			actor = GetComponent<Actor>();
		}
		if (executor == null)
		{
			executor = GetComponent<ActorBehaviourExecutor> ();
		}

		if (actor.GetData().PhysicalCondition.IsDead)
		{
			// Don't perform behaviours if this actor is dead
			return;
		}

		switch (activity) 
		{
		case Activity.Accompany:
			executor.Execute_Accompany();
			break;
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
