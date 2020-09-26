using UnityEngine;

public static class ActorTaskAssigner
{
	/// <summary>
	/// Finds the object for the specified Actor and adds the task to its task list.
	/// </summary>
	/// <returns>Task ID of task, so the task can be cancelled later</returns>
	public static string AssignTask(AssignableTask task, string assignerId, string assigneeId)
	{
		Actor assignee = ActorRegistry.Get(assigneeId).actorObject;
		if (assignee == null)
		{
			Debug.LogError("Attempted to assign task to Actor not found in ActorRegistry!");
			return null;
		}
		ActorTaskList assigneeTaskList = assignee.GetComponent<ActorTaskList>();
		if (assigneeTaskList == null)
		{
			Debug.LogError("Actor to assign task to has no ActorTaskList component!");
			return null;
		}
		return assigneeTaskList.AddTask(task, assignerId);
	}
}
