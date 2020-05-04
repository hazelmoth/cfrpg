using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour {

	public delegate void DialogueEvent ();
	public delegate void DialogueInitiationEvent (NPC npc, DialogueDataMaster.DialogueNode startNode);
	public delegate void DialogueNodeEvent (DialogueDataMaster.DialogueNode node);
	public delegate void DialogueTextUpdateEvent (string dialogue);
	public delegate void DialogueResponsesUpdateEvent (List<string> responses);
	public static event DialogueInitiationEvent OnInitiateDialogue;
	public static event DialogueEvent OnExitDialogue;
	public static event DialogueNodeEvent OnGoToDialogueNode;
	public static event DialogueTextUpdateEvent OnNpcDialogueUpdate;
	public static event DialogueEvent OnRequestResponse;
	public static event DialogueResponsesUpdateEvent OnAvailableResponsesUpdated;

	static DialogueManager instance;
	NPC currentNpc; // The NPC the player is currently interacting with
	DialogueDataMaster.DialogueNode currentDialogueNode; // The dialogue node that is currently being said or responded to
	List<DialogueDataMaster.DialogueResponse> currentDialogueResponses; // The responses currently available to the player
	int currentDialoguePhraseIndex;
	bool isAwaitingResponse = false;
	bool isInDialogue = false;
	public static bool IsInDialogue => instance.isInDialogue;

	// Use this for initialization
	void Start () {
		instance = this;
		NPCInteractionHandler.OnInteractWithNpc += InitiateDialogue;
	}
	void OnDestroy ()
	{
		OnInitiateDialogue = null;
		OnExitDialogue = null;
		OnGoToDialogueNode = null;
		OnNpcDialogueUpdate = null;
		OnRequestResponse = null;
		OnAvailableResponsesUpdated = null;
	}
	void InitiateDialogue (NPC npc) {
		isInDialogue = true;
		DialogueDataMaster.DialogueNode startNode = null;
		if (TryFindStartingNode(npc, out startNode)) {
			currentNpc = npc;
			if (OnInitiateDialogue != null)
				OnInitiateDialogue (npc, startNode);
			GoToDialogueNode (startNode);
		}
	}
	// Called directly from DialogueUIManager
	public static void AdvanceDialogue () {
		if (instance.currentDialogueNode == null)
			return;
		instance.currentDialoguePhraseIndex++;
		if (instance.currentDialoguePhraseIndex >= instance.currentDialogueNode.phrases.Count) {
			instance.RequestResponse ();
		}
		else
		{
			string phraseId = instance.currentDialogueNode.phrases[instance.currentDialoguePhraseIndex].phraseId;
			OnNpcDialogueUpdate?.Invoke(EvaluatePhraseId(phraseId, new DialogueContext(instance.currentNpc.ActorId, ActorRegistry.Get(PlayerController.PlayerActorId).gameObject.ActorId)));

			foreach (string command in instance.currentDialogueNode.phrases[instance.currentDialoguePhraseIndex]
				.commands)
			{
				DialogueScriptHandler.ExecuteCommand(command, new DialogueContext(instance.currentNpc.ActorId, ActorRegistry.Get(PlayerController.PlayerActorId).gameObject.ActorId));
			}
		}
	}
	// Also called directly from DialogueUIManager
	public static void SelectDialogueResponse (int responseIndex) {
		instance.OnResponseChosen (responseIndex);
	}
	void GoToDialogueNode (DialogueDataMaster.DialogueNode node) {
		if (node == null)
		{
			ExitDialogue();
		}
		isAwaitingResponse = false;
		currentDialoguePhraseIndex = 0;
		currentDialogueNode = node;

		// Populate currentDialogueResponses with all available responses
		currentDialogueResponses = new List<DialogueDataMaster.DialogueResponse> ();
		foreach (DialogueDataMaster.DialogueResponse response in node.responses) {
			currentDialogueResponses.Add (response);
		}
		foreach (DialogueDataMaster.GenericResponseNode responseNode in DialogueDataMaster.ResponseNodes) {
			bool meetsConditions = true;
			foreach (string condition in responseNode.preconditions) {
				if (!DialogueScriptHandler.CheckCondition(condition, currentNpc)) {
					meetsConditions = false;
				}
			}
			if (meetsConditions && !node.blockGenericResponses)
				currentDialogueResponses.Add (responseNode.response);
		}
		// Get response strings and call responses update event
		List<string> responseStrings = new List<string>();
		foreach (DialogueDataMaster.DialogueResponse response in currentDialogueResponses)
		{
			responseStrings.Add(EvaluatePhraseId(response.phraseId, new DialogueContext(ActorRegistry.Get(PlayerController.PlayerActorId).gameObject.ActorId, currentNpc.ActorId)));
		}
		OnAvailableResponsesUpdated?.Invoke(responseStrings);

		// Now get the actual dialogue string
		string npcPhraseId = node.phrases[0].phraseId;
		string npcPhrase = ContentLibrary.Instance.Personalities.GetById(currentNpc.GetData().Personality).GetDialoguePack()
			.GetLine(npcPhraseId);
		if (npcPhrase != null)
		{
			npcPhrase = DialogueScriptHandler.PopulatePhrase(npcPhrase,
				new DialogueContext(currentNpc.ActorId, ActorRegistry.Get(PlayerController.PlayerActorId).gameObject.ActorId));
			OnNpcDialogueUpdate?.Invoke(npcPhrase);
		}
		else
		{
			Debug.LogWarning("Line in master dialogue file \"" + npcPhraseId + "\" isn't a valid phrase ID");
			OnNpcDialogueUpdate?.Invoke(npcPhraseId);
		}

		// Finally call any commands associated with the first bit of dialogue in this node
		foreach (string command in node.phrases[0].commands)
		{
			DialogueScriptHandler.ExecuteCommand(command, new DialogueContext(currentNpc.ActorId, ActorRegistry.Get(PlayerController.PlayerActorId).gameObject.ActorId));
		}
	}
	void RequestResponse () {
		currentDialoguePhraseIndex = 0;
		isAwaitingResponse = true;
		OnRequestResponse?.Invoke ();
	}
	void OnResponseChosen (int responseIndex) {
		isAwaitingResponse = false;
		DialogueDataMaster.DialogueResponse response = currentDialogueResponses [responseIndex];
		if (response.isExitResponse)
			ExitDialogue ();
		else {
			GoToDialogueNode (DialogueDataMaster.GetLinkedNodeFromResponse(response));
		}

		DialogueContext context = new DialogueContext(ActorRegistry.Get(PlayerController.PlayerActorId).gameObject.ActorId, currentNpc.ActorId);
		foreach (string command in response.commands)
		{
			DialogueScriptHandler.ExecuteCommand(command, context);
		}
	}

	void ExitDialogue () {
		isInDialogue = false;
		OnExitDialogue?.Invoke ();
	}

	private static string EvaluatePhraseId(string id, DialogueContext context)
	{
		Actor speaker = ActorRegistry.Get(context.speakerActorId).gameObject;
		PersonalityData personality = ContentLibrary.Instance.Personalities.GetById(speaker.GetData().Personality);
		DialoguePack dialogue = personality.GetDialoguePack();
		string npcPhrase = dialogue.GetLine(id);

		if (npcPhrase != null)
		{
			npcPhrase = DialogueScriptHandler.PopulatePhrase(npcPhrase, context);
			return npcPhrase;
		}
		else
		{
			Debug.LogWarning("Line in master dialogue file \"" + id + "\" isn't a valid phrase ID");
			return id;
		}
	}

	bool TryFindStartingNode (NPC npc, out DialogueDataMaster.DialogueNode startNode) {
		List<DialogueDataMaster.DialogueNode> possibleNodes = new List<DialogueDataMaster.DialogueNode> ();
		foreach (DialogueDataMaster.DialogueNode node in DialogueDataMaster.DialogueNodes) {
			if (!node.isStartDialogue) { // Only look at nodes that have the isStart element set to true
				continue;
			}
			bool meetsConditions = true;
			foreach (string condition in node.preconditions) {
				if (!DialogueScriptHandler.CheckCondition (condition, npc)) {
					meetsConditions = false;
				}
			}
			if (meetsConditions) {
				possibleNodes.Add (node);
			}
		}
		if (possibleNodes.Count == 0) {
			startNode = new DialogueDataMaster.DialogueNode ();
			return false;
		}
		int currentBestImportance = int.MinValue;
		DialogueDataMaster.DialogueNode currentBestNode = new DialogueDataMaster.DialogueNode();
		foreach (DialogueDataMaster.DialogueNode node in possibleNodes) {
			if (node.priority > currentBestImportance) {
				currentBestNode = node;
				currentBestImportance = node.priority;
			}
		}
		startNode = currentBestNode;
		return true;
	}
}
