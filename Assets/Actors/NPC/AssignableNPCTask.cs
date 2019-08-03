using System;

public class AssignableNpcTask
{
	public NPCBehaviourAI.Activity activity;
	public string taskId;
	public string taskName;
	public bool assignableByPlayer;

	public AssignableNpcTask(NPCBehaviourAI.Activity activity, string taskId, string taskName, bool assignableByPlayer)
	{
		this.activity = activity;
		this.taskId = taskId;
		this.taskName = taskName;
		this.assignableByPlayer = assignableByPlayer;
	}
}
