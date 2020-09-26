using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ActorTaskLibrary
{
	private static List<AssignableTask> tasks = new List<AssignableTask>
	{
		new AssignableTask(ActorBehaviourAi.Activity.Eat, "eat", "eat", false),
		new AssignableTask(ActorBehaviourAi.Activity.ScavengeForFood, "scavenge_for_food", "scavenge for food", true),
		new AssignableTask(ActorBehaviourAi.Activity.ScavengeForWood, "scavenge_for_wood", "scavenge for wood", true),
		new AssignableTask(ActorBehaviourAi.Activity.Wander, "wander", "wander", false)
	};

	public static List<AssignableTask> GetAllTasks ()
	{
		return tasks;
	}
	public static AssignableTask GetTaskById (string id)
	{
		foreach (AssignableTask task in tasks)
		{
			if (task.taskId == id)
				return task;
		}
		return null;
	}

}
