using System;

public class AssignableTask
{
	public ActorBehaviourAi.Activity activity;
	public string taskId;
	public string taskName;
	public bool assignableByPlayer;

	public AssignableTask(ActorBehaviourAi.Activity activity, string taskId, string taskName, bool assignableByPlayer)
	{
		this.activity = activity;
		this.taskId = taskId;
		this.taskName = taskName;
		this.assignableByPlayer = assignableByPlayer;
	}
}
