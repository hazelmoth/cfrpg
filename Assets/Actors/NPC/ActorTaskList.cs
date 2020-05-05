using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorTaskList : MonoBehaviour
{
	public struct AssignedTask
	{
		public AssignableTask task;
		public string assignerId;
		public float timeAssigned;
		public string taskId;

		public AssignedTask(AssignableTask task, string assignerId, float timeAssigned, string taskId)
		{
			this.task = task;
			this.assignerId = assignerId;
			this.timeAssigned = timeAssigned;
			this.taskId = taskId;
		}
	}

	private List<AssignedTask> taskList;

	public List<AssignedTask> Tasks
	{
		get {
			if (taskList == null)
			{
				taskList = new List<AssignedTask>();
			}
			return taskList;
		}
	}

	/// <returns>The unique ID of the assigned task, so it can be canceled later.</returns>
	public string AddTask (AssignableTask task, string assignerId)
	{
		if (taskList == null)
		{
			taskList = new List<AssignedTask>();
		}
		string taskId = assignerId + "_" + task + "_" + Time.time;
		AssignedTask newTask = new AssignedTask(task, assignerId, Time.time, taskId);
		taskList.Add(newTask);
		return taskId;
	}
	public void CancelTask (string taskId)
	{
		if (taskList == null)
		{
			taskList = new List<AssignedTask>();
		}

		for (int i=0; i < taskList.Count; i++)
		{
			if (taskList[i].taskId == taskId)
			{
				taskList.RemoveAt(i);
				return;
			}
		}
		Debug.LogWarning("Task ID to cancel doesn't match any ID in this task list!");
	}
	public void CancelAllTasks()
	{
		if (taskList == null)
		{
			taskList = new List<AssignedTask>();
		}

		taskList.Clear();
	}
    
}
