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
	[SerializeField] private Image ActorImage_Body;
	[SerializeField] private Image ActorImage_Hair;
	[SerializeField] private Image ActorImage_Hat;
	[SerializeField] private Image ActorImage_Shirt;
	[SerializeField] private Image ActorImage_Pants;
	[SerializeField] private TextMeshProUGUI ActorNameText;
	[SerializeField] private GameObject jobListContent;
	[SerializeField] private GameObject jobListItemPrefab;
	[SerializeField] private GameObject skillListContent;
	[SerializeField] private GameObject skillListItemPrefab;
	[SerializeField] private TextMeshProUGUI accompanyButtonText;

	public delegate void JobUIEvent();
	public static event JobUIEvent OnExit;

	private static SurvivorMenuManager instance;

	private string ActorId;
	private Actor currentTargetActor;
	private JobListItem currentSelectedJobItem;

	// Start is called before the first frame update
	private void Start()
	{
		instance = this;

		PopulateJobList();

		PlayerInteractionManager.OnInteractWithSettler += SetTargetedActor;
		SceneChangeActivator.OnSceneExit += OnUnitySceneExit;
		UIManager.OnOpenSurvivorMenu += OnOpened;
	}

	private void OnOpened ()
	{
		PopulateJobList();
		SetAllJobsUnhighlighted();
		currentSelectedJobItem = null;
		UpdateAccompanyButtonText(currentTargetActor.GetData().FactionStatus.AccompanyTarget != null);
	}
	public static void SetTargetedActor (Actor ActorObject)
	{
		ActorData Actor = ActorRegistry.Get(ActorObject.ActorId).data;
		instance.ActorId = Actor.actorId;
		instance.currentTargetActor = ActorObject;
		instance.ActorNameText.text = Actor.ActorName;
		instance.UpdateImageSprites();
		instance.UpdateAccompanyButtonText(ActorObject.GetData().FactionStatus.AccompanyTarget != null);
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
		currentTargetActor.GetData().FactionStatus.AssignedJob = job;
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
		if (currentTargetActor.GetData().FactionStatus.AccompanyTarget == ActorRegistry.Get(PlayerController.PlayerActorId).gameObject.ActorId)
		{
			currentTargetActor.GetData().FactionStatus.AccompanyTarget = null;
			UpdateAccompanyButtonText(false);
		}
		else
		{
			currentTargetActor.GetData().FactionStatus.AccompanyTarget = ActorRegistry.Get(PlayerController.PlayerActorId).gameObject.ActorId;
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

	private void UpdateImageSprites ()
	{
		HumanSpriteController sprites = currentTargetActor.GetComponent<HumanSpriteController>();
		ActorImage_Hair.sprite = sprites.CurrentHairSprite;
		ActorImage_Hat.sprite = sprites.CurrentHatSprite;
		ActorImage_Body.sprite = sprites.CurrentBodySprite;
		ActorImage_Shirt.sprite = sprites.CurrentShirtSprite;
		ActorImage_Pants.sprite = sprites.CurrentPantsSprite;

		ActorImage_Hair.SetAlphaIfNullSprite();
		ActorImage_Hat.SetAlphaIfNullSprite();
		ActorImage_Body.SetAlphaIfNullSprite();
		ActorImage_Shirt.SetAlphaIfNullSprite();
		ActorImage_Pants.SetAlphaIfNullSprite();
	}

	private void ClearJobList ()
	{
		currentSelectedJobItem = null;
		for (int i = 0; i < jobListContent.transform.childCount; i++)
		{
			Destroy(jobListContent.transform.GetChild(i).gameObject);
		}
	}

	private void SetAllJobsUnhighlighted ()
	{
		for (int i = 0; i < jobListContent.transform.childCount; i++)
		{
			jobListContent.transform.GetChild(i).GetComponent<JobListItem>()?.SetHighlighted(false);
		}
	}

	private void PopulateJobList()
	{
		// only show tasks that are assignable by player
		IDictionary<string, string> jobDict = JobLibrary.GetJobs();
		PopulateJobList(jobDict);
	}

	private void PopulateJobList (IDictionary<string, string> jobs)
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

	private void OnUnitySceneExit ()
	{
		OnExit = null;
		instance = null;
	}
}