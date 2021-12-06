using System.Collections.Generic;
using System.Collections.Immutable;
using Dialogue;
using TMPro;
using UnityEngine;

namespace GUI
{
	public class DialogueUIManager : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI actorDialogueText;
		[SerializeField] private GameObject dialogueResponseScrollView;
		[SerializeField] private GameObject scrollViewContentPanel;
		[SerializeField] private GameObject dialogueOptionPrefab;
		[SerializeField] private TextMeshProUGUI speakerNameText;
		private TextScroller textScroller;
		private List<string> currentResponses;

		// Use this for initialization
		private void Start()
		{
			DialogueManager.OnInitiateDialogue += OnDialogueStart;
			DialogueManager.OnRequestResponse += OnDialogueResponseRequested;
			DialogueManager.OnAvailableResponsesUpdated += OnAvailableResponsesUpdate;
			DialogueManager.OnActorDialogueUpdate += OnActorDialogueUpdate;
			textScroller = FindObjectOfType<TextScroller>();
			currentResponses = new List<string>();
			SwitchToActorDialogueView();
		}

		private void Update()
		{
			if (!PauseManager.Paused && Input.GetMouseButtonDown(0))
			{
				if (textScroller.Scrolling) textScroller.FinishScroll();
				else if (DialogueManager.IsInDialogue) DialogueManager.AdvanceDialogue();
			}
		}
		// Called from OnInitiateDialogue event in DialogueManager
		private void OnDialogueStart(Actor actor)
		{
			ActorData actorData = ActorRegistry.Get(actor.ActorId).data;
			SwitchToActorDialogueView();
			SetNameText(actorData.ActorName);
		}
		// Called from OnActorDialogueUpdate event in DialogueManager
		private void OnActorDialogueUpdate(string dialogue)
		{
			ShowActorDialogue(dialogue);
		}
		// Called from OnAvailableResponsesUpdated event in DialogueManager
		private void OnAvailableResponsesUpdate(ImmutableList<string> responses)
		{
			SetResponseOptions(responses);
		}
		// Called from OnRequestResponse event in DialogueManager
		private void OnDialogueResponseRequested()
		{
			SwitchToPlayerResponseView();
		}

		private void SetResponseOptions(IReadOnlyList<string> responses)
		{
			for (int i = 0; i < responses.Count; i++)
			{
				GameObject option = Instantiate(dialogueOptionPrefab, scrollViewContentPanel.transform);
				RectTransform rect = option.GetComponent<RectTransform>();
				rect.localPosition = new Vector2(scrollViewContentPanel.GetComponent<RectTransform>().rect.width / 2, i * rect.rect.height * -1);
				option.GetComponentInChildren<TextMeshProUGUI>().text = responses[i];
			}
		}

		private void ShowActorDialogue(string dialogue)
		{
			if (!textScroller) actorDialogueText.text = dialogue;
			else textScroller.ScrollText(actorDialogueText, dialogue);
		}

		private void SetNameText(string name)
		{
			speakerNameText.text = name;
		}

		private void SwitchToActorDialogueView()
		{
			DestroyDialogueButtons();
			dialogueResponseScrollView.SetActive(false);
			actorDialogueText.gameObject.SetActive(true);
		}

		private void SwitchToPlayerResponseView()
		{
			dialogueResponseScrollView.SetActive(true);
			actorDialogueText.gameObject.SetActive(false);
		}

		private void DestroyDialogueButtons()
		{
			foreach (Transform child in scrollViewContentPanel.transform)
			{
				Destroy(child.gameObject);
			}
		}
		// Called from dialogue button handler scripts
		public void OnDialogueOptionButton(GameObject button)
		{
			DestroyDialogueButtons();
			SwitchToActorDialogueView();
			DialogueManager.SelectDialogueResponse(FindIndexOfButtonObject(button));
		}

		private int FindIndexOfButtonObject(GameObject button)
		{
			for (int i = 0; i < scrollViewContentPanel.transform.childCount; i++)
			{
				if (scrollViewContentPanel.transform.GetChild(i).gameObject.GetInstanceID() == button.GetInstanceID())
					return i;
			}
			Debug.LogError("DialogueUIManager failed to find a button index for a gameobject that registered a dialogue button press!");
			return 0;
		}
	}
}
