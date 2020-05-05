using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueUIManager : MonoBehaviour {

	public delegate void DialogueUIEvent (); // for generic events that don't need parameters -not currently used-
	public delegate void DialogueButtonPressEvent (int index); // includes index of selected option in list of available options
	public static event DialogueButtonPressEvent OnDialogueOptionChosen;
	[SerializeField] private TextMeshProUGUI ActorDialogueText;
	[SerializeField] private GameObject dialogueResponseScrollView;
	[SerializeField] private GameObject scrollViewContentPanel;
	[SerializeField] private GameObject dialogueOptionPrefab;
	[SerializeField] private TextMeshProUGUI speakerNameText;
	private List<string> currentResponses;

	private void OnDestroy ()
	{
		OnDialogueOptionChosen = null;
	}
	// Use this for initialization
	private void Start () {
		DialogueManager.OnInitiateDialogue += OnDialogueStart;
		DialogueManager.OnRequestResponse += OnDialogueResponseRequested;
		DialogueManager.OnAvailableResponsesUpdated += OnAvailableResponsesUpdate;
		DialogueManager.OnActorDialogueUpdate += OnActorDialogueUpdate;
		currentResponses = new List<string> ();
		SwitchToActorDialogueView ();
	}

	private void Update () {
		// TODO move this to input class
		if (Input.GetMouseButtonDown(0) && !PauseManager.GameIsPaused) {
			OnAdvanceDialogueInput ();
		}
	}
	// Called from OnInitiateDialogue event in DialogueManager
	private void OnDialogueStart (Actor Actor, DialogueDataMaster.DialogueNode startNode) {
		ActorData ActorData = ActorRegistry.Get (Actor.ActorId).data;
		SwitchToActorDialogueView ();
		SetActorDialogue (startNode.phrases [0].phraseId);
		SetNameText (ActorData.ActorName);
	}
	// Called from OnActorDialogueUpdate event in DialogueManager
	private void OnActorDialogueUpdate (string dialogue) {
		SetActorDialogue (dialogue);
	}
	// Called from OnAvailableResponsesUpdated event in DialogueManager
	private void OnAvailableResponsesUpdate (List<string> responses) {
		SetResponseOptions (responses);
	}
	// Called from OnRequestResponse event in DialogueManager
	private void OnDialogueResponseRequested () {
		SwitchToPlayerResponseView ();
	}

	private void SetResponseOptionsFromData (List<DialogueDataMaster.GenericResponseNode> responses) {
		List<string> responseStrings = new List<string>();
		foreach (DialogueDataMaster.GenericResponseNode node in responses) {
			responseStrings.Add (node.response.phraseId);
		}
		currentResponses = responseStrings;
		SetResponseOptions (responseStrings);
	}

	private void SetResponseOptions (List<string> responses) {
		for (int i = 0; i < responses.Count; i++) {
			GameObject option = Instantiate (dialogueOptionPrefab, scrollViewContentPanel.transform);
			RectTransform rect = option.GetComponent<RectTransform> ();
			rect.localPosition = new Vector2(scrollViewContentPanel.GetComponent<RectTransform>().rect.width / 2, i * rect.rect.height * -1);
			option.GetComponentInChildren<TextMeshProUGUI> ().text = responses [i];
		}
	}

	private void SetActorDialogue (string dialogue) {
		ActorDialogueText.text = dialogue;
	}

	private void SetNameText (string name) {
		speakerNameText.text = name;
	}

	private void SwitchToActorDialogueView () {
		DestroyDialogueButtons ();
		dialogueResponseScrollView.SetActive (false);
		ActorDialogueText.gameObject.SetActive (true);
	}

	private void SwitchToPlayerResponseView () {
		dialogueResponseScrollView.SetActive (true);
		ActorDialogueText.gameObject.SetActive (false);
	}
	// Called when the player provides input to continue to the next phrase of Actor dialogue
	private void OnAdvanceDialogueInput () {
		// direct calls are probably not ideal so maybe rework this somehow
		// perhaps an extra class to handle only dialogue screen input that DialogueManager can interface
		DialogueManager.AdvanceDialogue ();
	}

	private void DestroyDialogueButtons () {
		foreach (Transform child in scrollViewContentPanel.transform) {
			Destroy (child.gameObject);
		}
	}
	// Called from dialogue button handler scripts
	public void OnDialogueOptionButton (GameObject button) {
		DestroyDialogueButtons ();
		SwitchToActorDialogueView (); // We're assuming that after a dialogue option is chosen we always want to go back to the Actor dialogue screen
		DialogueManager.SelectDialogueResponse (FindIndexOfButtonObject (button));
	}

	private int FindIndexOfButtonObject (GameObject button) {
		for (int i = 0; i < scrollViewContentPanel.transform.childCount; i++) {
			if (scrollViewContentPanel.transform.GetChild (i).gameObject.GetInstanceID () == button.GetInstanceID ())
				return i;
		}
		Debug.LogError ("DialogueUIManager failed to find a button index for a gameobject that registered a dialogue button press!");
		return 0;
	}
}
