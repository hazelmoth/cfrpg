using ActorComponents;
using UnityEngine;
using SettlementSystem;

// Decides what the Actor should do, and keeps track of what it's doing.
public class ActorBehaviourAi : MonoBehaviour
{
	public enum Activity {
		None,
		ChillAtHome,
		Trade,
		Accompany,
		Eat,
		ScavengeForFood,
		ScavengeForWood,
		Settler,
		StashWood,
		Wander
	}

	private Actor actor;
	private ActorPhysicalCondition actorCondition;
	private ActorBehaviourExecutor executor;
	private ActorTaskList taskList;
	private SettlementManager settlement;

    // Update is called once per frame
    private void Update()
    {
	    if (actor == null)
	    {
		    actor = GetComponent<Actor>();
	    }
		if (settlement == null)
		{
			settlement = GameObject.FindObjectOfType<SettlementManager>();
			if (settlement == null)
			{
				Debug.LogError("SettlementManager object not found");
			}
		}
		if (actor.PlayerControlled)
	    {
		    return;
	    }

		ExecuteActivity (EvaluateBehaviour());
    }

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

		// Traders always trade
		if (actor.GetData().GetComponent<Trader>() != null)
		{
			nextActivity = Activity.Trade;
		}
		else if (actor.GetData().FactionStatus.AccompanyTarget != null)
		{
			nextActivity = Activity.Accompany;
		}
		else if (nextActivity == Activity.ScavengeForWood && actor.GetData().Inventory.IsFull(includeApparelSlots: false))
		{
			nextActivity = Activity.StashWood;
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


		// Wander if we have nothing else to do
		if (nextActivity == Activity.None) 
		{
			if (settlement.GetHouse(actor.ActorId) != null)
			{
				nextActivity = Activity.ChillAtHome;
			}
			else nextActivity = Activity.Wander;
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

		//TEST:
		string faction = actor.GetData().FactionStatus.FactionId;
		if (!string.IsNullOrEmpty(faction) && faction == ActorRegistry.Get(PlayerController.PlayerActorId).data.FactionStatus.FactionId)
		{
			executor.Execute_Settler();
			return;
		}

		switch (activity) 
		{
			case Activity.Accompany:
				executor.Execute_Accompany();
				break;
			case Activity.ChillAtHome:
				executor.Execute_ChillAtHome();
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
			case Activity.Trade:
				executor.Execute_Trade();
				break;
			// If there's nothing to do, keep doing whatever it is we were doing
			case Activity.None:
			default:
				break;
		}
	}
}
