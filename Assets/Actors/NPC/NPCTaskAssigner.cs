using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NPCTaskAssigner
{
	/// <summary>
	/// Finds the object for the specified NPC and adds the task to its task list.
	/// </summary>
	/// <returns>Task ID of task, so the task can be cancelled later</returns>
	public static string AssignTask(AssignableNpcTask task, string assignerId, string assigneeId)
	{
		NPC assignee = NPCObjectRegistry.GetNPCObject(assigneeId);
		if (assignee == null)
		{
			Debug.LogError("Attempted to assign task to NPC not found in NPCObjectRegistry!");
			return null;
		}
		NPCTaskList assigneeTaskList = assignee.GetComponent<NPCTaskList>();
		if (assigneeTaskList == null)
		{
			Debug.LogError("NPC to assign task to has no NPCTaskList component!");
			return null;
		}
		return assigneeTaskList.AddTask(task, assignerId);
	}
}
