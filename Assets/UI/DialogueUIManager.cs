using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueUIManager : MonoBehaviour {

	public delegate void DialogueUIEvent (); // for generic events that don't need parameters -not currently used-
	public delegate void DialogueButtonPressEvent (int index); // includes index of selected option in list of available options
	public static event DialogueButtonPressEvent OnDialogueOptionChosen;
	[SerializeField] TextMeshProUGUI npcDialogueText;
	[SerializeField] GameObject dialogueResponseScrollView;
	[SerializeField] GameObject scrollViewContentPanel;
	[SerializeField] GameObject dialogueOptionPrefab;
	[SerializeField] TextMeshProUGUI speakerNameText;
	List<string> currentResponses;

	// Use this for initialization
	void Start () {
		DialogueManager.OnInitiateDialogue += OnDialogueStart;
		DialogueManager.OnRequestResponse += OnDialogueResponseRequested;
		DialogueManager.OnAvailableResponsesUpdated += OnAvailableResponsesUpdate;
		DialogueManager.OnNpcDialogueUpdate += OnNpcDialogueUpdate;
		currentResponses = new List<string> ();
		SwitchToNpcDialogueView ();
	}
	void Update () {
		// TODO move this to input class
		if (Input.GetMouseButtonDown(0) && !PauseManager.GameIsPaused) {
			OnAdvanceDialogueInput ();
		}
	}
	// Called from OnInitiateDialogue event in DialogueManager
	void OnDialogueStart (NPC npc, DialogueDataMaster.DialogueNode startNode) {
		NPCData npcData = NPCDataMaster.GetNpcFromId (npc.NpcId);
		SwitchToNpcDialogueView ();
		SetNpcDialogue (startNode.phrases [0].text);
		SetNameText (npcData.NpcName);
	}
	// Called from OnNpcDialogueUpdate event in DialogueManager
	void OnNpcDialogueUpdate (string dialogue) {
		SetNpcDialogue (dialogue);
	}
	// Called from OnAvailableResponsesUpdated event in DialogueManager
	void OnAvailableResponsesUpdate (List<string> responses) {
		SetResponseOptions (responses);
	}
	// Called from OnRequestResponse event in DialogueManager
	void OnDialogueResponseRequested () {
		SwitchToPlayerResponseView ();
	}
	void SetResponseOptionsFromData (List<DialogueDataMaster.GenericResponseNode> responses) {
		List<string> responseStrings = new List<string>();
		foreach (DialogueDataMaster.GenericResponseNode node in responses) {
			responseStrings.Add (node.response.text);
		}
		currentResponses = responseStrings;
		SetResponseOptions (responseStrings);
	}

	void SetResponseOptions (List<string> responses) {
		for (int i = 0; i < responses.Count; i++) {
			GameObject option = Instantiate (dialogueOptionPrefab, scrollViewContentPanel.transform);
			RectTransform rect = option.GetComponent<RectTransform> ();
			rect.localPosition = new Vector2(scrollViewContentPanel.GetComponent<RectTransform>().rect.width / 2, i * rect.rect.height * -1);
			option.GetComponentInChildren<TextMeshProUGUI> ().text = responses [i];
		}
	}
	void SetNpcDialogue (string dialogue) {
		npcDialogueText.text = dialogue;
	}
	void SetNameText (string name) {
		speakerNameText.text = name;
	}
	void SwitchToNpcDialogueView () {
		DestroyDialogueButtons ();
		dialogueResponseScrollView.SetActive (false);
		npcDialogueText.gameObject.SetActive (true);
	}
	void SwitchToPlayerResponseView () {
		dialogueResponseScrollView.SetActive (true);
		npcDialogueText.gameObject.SetActive (false);
	}
	// Called when the player provides input to continue to the next phrase of NPC dialogue
	void OnAdvanceDialogueInput () {
		Debug.Log ("OnAdvanceDialogueInput");
		// direct calls are probably not ideal so maybe rework this somehow
		// perhaps an extra class to handle only dialogue screen input that DialogueManager can interface
		DialogueManager.AdvanceDialogue ();
	}
	void DestroyDialogueButtons () {
		foreach (Transform child in scrollViewContentPanel.transform) {
			Destroy (child.gameObject);
		}
	}
	// Called from dialogue button handler scripts
	public void OnDialogueOptionButton (GameObject button) {
		DestroyDialogueButtons ();
		SwitchToNpcDialogueView (); // We're assuming that after a dialogue option is chosen we always want to go back to the NPC dialogue screen
		DialogueManager.SelectDialogueResponse (FindIndexOfButtonObject (button));
	} 

	int FindIndexOfButtonObject (GameObject button) {
		for (int i = 0; i < scrollViewContentPanel.transform.childCount; i++) {
			if (scrollViewContentPanel.transform.GetChild (i).gameObject.GetInstanceID () == button.GetInstanceID ())
				return i;
		}
		Debug.LogError ("DialogueUIManager failed to find a button index for a gameobject that registered a dialogue button press!");
		return 0;
	}
}
