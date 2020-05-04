using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SurvivorMenuManager : MonoBehaviour
{
	private const float JobPanelOffset = 2048f;
	private const string AccompanyText = "Accompany me!";
	private const string StopAccompanyText = "Stop accompanying";

	[SerializeField] private GameObject menuPanel;
	[SerializeField] private GameObject jobListPanel;
	[SerializeField] private Image npcImage_Body;
	[SerializeField] private Image npcImage_Hair;
	[SerializeField] private Image npcImage_Hat;
	[SerializeField] private Image npcImage_Shirt;
	[SerializeField] private Image npcImage_Pants;
	[SerializeField] private TextMeshProUGUI npcNameText;
	[SerializeField] private GameObject jobListContent;
	[SerializeField] private GameObject jobListItemPrefab;
	[SerializeField] private GameObject skillListContent;
	[SerializeField] private GameObject skillListItemPrefab;
	[SerializeField] private TextMeshProUGUI accompanyButtonText;

	public delegate void JobUIEvent();
	public static event JobUIEvent OnExit;

	static SurvivorMenuManager instance;

	private string npcId;
	private NPC currentTargetNpc;
	JobListItem currentSelectedJobItem;

	// Start is called before the first frame update
	void Start()
	{
		instance = this;

		PopulateJobList();

		PlayerInteractionManager.OnInteractWithSettler += SetTargetedNpc;
		SceneChangeActivator.OnSceneExit += OnUnitySceneExit;
		UIManager.OnOpenSurvivorMenu += OnOpened;
	}
	void OnOpened ()
	{
		PopulateJobList();
		SetAllJobsUnhighlighted();
		currentSelectedJobItem = null;
		UpdateAccompanyButtonText(currentTargetNpc.GetData().FactionStatus.AccompanyTarget != null);
	}
	public static void SetTargetedNpc (NPC npcObject)
	{
		NPCData npc = NPCDataMaster.GetNpcFromId(npcObject.ActorId);
		instance.npcId = npc.NpcId;
		instance.currentTargetNpc = npcObject;
		instance.npcNameText.text = npc.NpcName;
		instance.UpdateImageSprites();
		instance.UpdateAccompanyButtonText(npcObject.GetData().FactionStatus.AccompanyTarget != null);
	}
	public static void OnJobSelected (JobListItem listItem)
	{
		instance.currentSelectedJobItem = listItem;
		instance.SetAllJobsUnhighlighted();
		listItem.SetHighlighted(true);
	}
	public void OnGoToJobListButton()
	{
		TransitionToJobPanel();
	}

	public void OnConfirmJobAssignButton()
	{
		if (currentSelectedJobItem == null)
		{
			return;
		}

		string job = currentSelectedJobItem.jobId;
		currentTargetNpc.AssignedJob = job;
		OnExit?.Invoke();
	}

	public void OnExitJobListButton()
	{
		TransitionToMainPanel();
	}

	public void OnSetHomeButton()
	{
	}

	public void OnAccompanyButton()
	{
		if (currentTargetNpc.GetData().FactionStatus.AccompanyTarget == ActorRegistry.Get(PlayerController.PlayerActorId).gameObject.ActorId)
		{
			currentTargetNpc.GetData().FactionStatus.AccompanyTarget = null;
			UpdateAccompanyButtonText(false);
		}
		else
		{
			currentTargetNpc.GetData().FactionStatus.AccompanyTarget = ActorRegistry.Get(PlayerController.PlayerActorId).gameObject.ActorId;
			UpdateAccompanyButtonText(true);
		}
	}
	public void OnExitButton()
	{
		OnExit?.Invoke();
	}

	private void UpdateAccompanyButtonText(bool currentlyAccompanying)
	{
		if (currentlyAccompanying)
		{
			accompanyButtonText.text = StopAccompanyText;
		}
		else
		{
			accompanyButtonText.text = AccompanyText;
		}
	}
	void UpdateImageSprites ()
	{
		HumanSpriteController sprites = currentTargetNpc.GetComponent<HumanSpriteController>();
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
	void ClearJobList ()
	{
		currentSelectedJobItem = null;
		for (int i = 0; i < jobListContent.transform.childCount; i++)
		{
			Destroy(jobListContent.transform.GetChild(i).gameObject);
		}
	}
	void SetAllJobsUnhighlighted ()
	{
		for (int i = 0; i < jobListContent.transform.childCount; i++)
		{
			jobListContent.transform.GetChild(i).GetComponent<JobListItem>()?.SetHighlighted(false);
		}
	}
	void PopulateJobList()
	{
		// only show tasks that are assignable by player
		IDictionary<string, string> jobDict = JobLibrary.GetJobs();
		PopulateJobList(jobDict);
	}
	void PopulateJobList (IDictionary<string, string> jobs)
	{
		ClearJobList();

		foreach (string id in jobs.Keys)
		{
			JobListItem newListItem = Instantiate(jobListItemPrefab, jobListContent.transform, false).GetComponent<JobListItem>();
			newListItem.SetText(jobs[id]);
			newListItem.jobId = id;
		}
	}

	private void TransitionToJobPanel()
	{
		menuPanel.transform.localPosition = new Vector3(-JobPanelOffset, 0);
		jobListPanel.transform.localPosition = new Vector3(0, 0);
	}
	private void TransitionToMainPanel()
	{
		menuPanel.transform.localPosition = new Vector3(0, 0);
		jobListPanel.transform.localPosition = new Vector3(JobPanelOffset, 0);
	}
	void OnUnitySceneExit ()
	{
		OnExit = null;
		instance = null;
	}
}