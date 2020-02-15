using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class TaskAssignmentUIManager : MonoBehaviour
{
	[SerializeField] Image npcImage_Body;
	[SerializeField] Image npcImage_Hair;
	[SerializeField] Image npcImage_Hat;
	[SerializeField] Image npcImage_Shirt;
	[SerializeField] Image npcImage_Pants;
	[SerializeField] TextMeshProUGUI npcNameText;
	[SerializeField] GameObject taskListContent;
	[SerializeField] GameObject taskListItemPrefab;

	public delegate void TaskUIEvent();
	public static event TaskUIEvent OnExitTaskUI;

	static TaskAssignmentUIManager instance;

	string currentTargetNpc;
	TaskListUIObject currentSelectedTaskItem;

	// Start is called before the first frame update
	void Start()
	{
		instance = this;

		PopulateTaskList();

		PlayerInteractionManager.OnPlayerInitiateTaskGiving += SetTargetedNpc;
		SceneChangeActivator.OnSceneExit += OnUnitySceneExit;
		UIManager.OnOpenTaskAssignmentScreen += OnOpened;
	}
	void OnOpened ()
	{
		PopulateTaskList();
		SetAllTasksUnhighlighted();
		currentSelectedTaskItem = null;
	}
	public static void SetTargetedNpc (NPC npcObject)
	{
		NPCData npc = NPCDataMaster.GetNpcFromId(npcObject.ActorId);
		instance.currentTargetNpc = npc.NpcId;
		instance.npcNameText.text = npc.NpcName;
		instance.UpdateImageSprites();
	}
	public static void OnTaskSelected (TaskListUIObject taskListObject)
	{
		instance.currentSelectedTaskItem = taskListObject;
		instance.SetAllTasksUnhighlighted();
		taskListObject.SetHighlighted(true);
	}
	public void OnGiveTaskButton()
	{
		if (currentSelectedTaskItem == null)
		{
			return;
		}
		AssignableNpcTask taskToGive = NPCTaskLibrary.GetTaskById(currentSelectedTaskItem.taskId);
		NPCTaskAssigner.AssignTask(taskToGive, "player", currentTargetNpc);
		OnExitTaskUI?.Invoke();
	}
	public void OnExitButton()
	{
		OnExitTaskUI?.Invoke();
	}
	void UpdateImageSprites ()
	{
		NPC npcObject = NPCObjectRegistry.GetNPCObject(currentTargetNpc);
		HumanSpriteController sprites = npcObject.GetComponent<HumanSpriteController>();
		npcImage_Hair.sprite = sprites.CurrentHairSprite;
		npcImage_Hat.sprite = sprites.CurrentHatSprite;
		npcImage_Body.sprite = sprites.CurrentBodySprite;
		npcImage_Shirt.sprite = sprites.CurrentShirtSprite;
		npcImage_Pants.sprite = sprites.CurrentPantsSprite;

		npcImage_Hair.SetAlphaIfNullSprite();
		npcImage_Hat.SetAlphaIfNullSprite();
		npcImage_Body.SetAlphaIfNullSprite();
		npcImage_Shirt.SetAlphaIfNullSprite();
		npcImage_Pants.SetAlphaIfNullSprite();
	}
	void ClearTaskList ()
	{
		currentSelectedTaskItem = null;
		for (int i = 0; i < taskListContent.transform.childCount; i++)
		{
			Destroy(taskListContent.transform.GetChild(i).gameObject);
		}
	}
	void SetAllTasksUnhighlighted ()
	{
		for (int i = 0; i < taskListContent.transform.childCount; i++)
		{
			taskListContent.transform.GetChild(i).GetComponent<TaskListUIObject>()?.SetHighlighted(false);
		}
	}
	void PopulateTaskList()
	{
		// only show tasks that are assignable by player
		List<AssignableNpcTask> tasks = NPCTaskLibrary.GetAllTasks();
		for (int i = tasks.Count - 1; i >= 0; i--)
		{
			if (!tasks[i].assignableByPlayer)
				tasks.RemoveAt(i);
		}
		PopulateTaskList(tasks);
	}
	void PopulateTaskList (List<AssignableNpcTask> tasks)
	{
		ClearTaskList();

		foreach (AssignableNpcTask task in tasks)
		{
			TaskListUIObject newListItem = Instantiate(taskListItemPrefab, taskListContent.transform, false).GetComponent<TaskListUIObject>();
			newListItem.SetText(task.taskName);
			newListItem.taskId = task.taskId;
		}
	}
	void OnUnitySceneExit ()
	{
		OnExitTaskUI = null;
		instance = null;
	}
}
