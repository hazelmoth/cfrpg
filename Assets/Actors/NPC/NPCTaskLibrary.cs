using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NPCTaskLibrary
{
	static List<AssignableNpcTask> tasks = new List<AssignableNpcTask>
	{
		new AssignableNpcTask(NPCBehaviourAI.Activity.Eat, "eat", "eat", false),
		new AssignableNpcTask(NPCBehaviourAI.Activity.ScavengeForFood, "scavenge_for_food", "scavenge for food", true),
		new AssignableNpcTask(NPCBehaviourAI.Activity.ScavengeForWood, "scavenge_for_wood", "scavenge for wood", true),
		new AssignableNpcTask(NPCBehaviourAI.Activity.Wander, "wander", "wander", false)
	};

	public static List<AssignableNpcTask> GetAllTasks ()
	{
		return tasks;
	}
	public static AssignableNpcTask GetTaskById (string id)
	{
		foreach (AssignableNpcTask task in tasks)
		{
			if (task.taskId == id)
				return task;
		}
		return null;
	}

}
