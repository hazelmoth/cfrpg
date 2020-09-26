using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour {

	public delegate void DialogueEvent ();
	public delegate void DialogueInitiationEvent (Actor Actor, DialogueDataMaster.DialogueNode startNode);
	public delegate void DialogueNodeEvent (DialogueDataMaster.DialogueNode node);
	public delegate void DialogueTextUpdateEvent (string dialogue);
	public delegate void DialogueResponsesUpdateEvent (List<string> responses);
	public static event DialogueInitiationEvent OnInitiateDialogue;
	public static event DialogueEvent OnExitDialogue;
	public static event DialogueNodeEvent OnGoToDialogueNode;
	public static event DialogueTextUpdateEvent OnActorDialogueUpdate;
	public static event DialogueEvent OnRequestResponse;
	public static event DialogueResponsesUpdateEvent OnAvailableResponsesUpdated;

	private static DialogueManager instance;
	private Actor currentActor; // The Actor the player is currently interacting with
	private DialogueDataMaster.DialogueNode currentDialogueNode; // The dialogue node that is currently being said or responded to
	private List<DialogueDataMaster.DialogueResponse> currentDialogueResponses; // The responses currently available to the player
	private int currentDialoguePhraseIndex;
	private bool isAwaitingResponse = false;
	private bool isInDialogue = false;
	public static bool IsInDialogue => instance.isInDialogue;

	// Use this for initialization
	private void Start () {
		instance = this;
		ActorInteractionHandler.OnInteractWithActor += InitiateDialogue;
	}

	private void OnDestroy ()
	{
		OnInitiateDialogue = null;
		OnExitDialogue = null;
		OnGoToDialogueNode = null;
		OnActorDialogueUpdate = null;
		OnRequestResponse = null;
		OnAvailableResponsesUpdated = null;
	}

	private void InitiateDialogue (Actor Actor) {
		isInDialogue = true;
		DialogueDataMaster.DialogueNode startNode = null;
		if (TryFindStartingNode(Actor, out startNode)) {
			currentActor = Actor;
			if (OnInitiateDialogue != null)
				OnInitiateDialogue (Actor, startNode);
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
			OnActorDialogueUpdate?.Invoke(EvaluatePhraseId(phraseId, new DialogueContext(instance.currentActor.ActorId, ActorRegistry.Get(PlayerController.PlayerActorId).actorObject.ActorId)));

			foreach (string command in instance.currentDialogueNode.phrases[instance.currentDialoguePhraseIndex]
				.commands)
			{
				DialogueScriptHandler.ExecuteCommand(command, new DialogueContext(instance.currentActor.ActorId, ActorRegistry.Get(PlayerController.PlayerActorId).actorObject.ActorId));
			}
		}
	}
	// Also called directly from DialogueUIManager
	public static void SelectDialogueResponse (int responseIndex) {
		instance.OnResponseChosen (responseIndex);
	}

	private void GoToDialogueNode (DialogueDataMaster.DialogueNode node) {
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
				if (!DialogueScriptHandler.CheckCondition(condition, currentActor)) {
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
			responseStrings.Add(EvaluatePhraseId(response.phraseId, new DialogueContext(ActorRegistry.Get(PlayerController.PlayerActorId).actorObject.ActorId, currentActor.ActorId)));
		}
		OnAvailableResponsesUpdated?.Invoke(responseStrings);

		// Now get the actual dialogue string
		string ActorPhraseId = node.phrases[0].phraseId;
		string ActorPhrase = ContentLibrary.Instance.Personalities.GetById(currentActor.GetData().Personality).GetDialoguePack()
			.GetLine(ActorPhraseId);
		if (ActorPhrase != null)
		{
			ActorPhrase = DialogueScriptHandler.PopulatePhrase(ActorPhrase,
				new DialogueContext(currentActor.ActorId, ActorRegistry.Get(PlayerController.PlayerActorId).actorObject.ActorId));
			OnActorDialogueUpdate?.Invoke(ActorPhrase);
		}
		else
		{
			Debug.LogWarning("Line in master dialogue file \"" + ActorPhraseId + "\" isn't a valid phrase ID");
			OnActorDialogueUpdate?.Invoke(ActorPhraseId);
		}

		// Finally call any commands associated with the first bit of dialogue in this node
		foreach (string command in node.phrases[0].commands)
		{
			DialogueScriptHandler.ExecuteCommand(command, new DialogueContext(currentActor.ActorId, ActorRegistry.Get(PlayerController.PlayerActorId).actorObject.ActorId));
		}
	}

	private void RequestResponse () {
		currentDialoguePhraseIndex = 0;
		isAwaitingResponse = true;
		OnRequestResponse?.Invoke ();
	}

	private void OnResponseChosen (int responseIndex) {
		isAwaitingResponse = false;
		DialogueDataMaster.DialogueResponse response = currentDialogueResponses [responseIndex];
		if (response.isExitResponse)
			ExitDialogue ();
		else {
			GoToDialogueNode (DialogueDataMaster.GetLinkedNodeFromResponse(response));
		}

		DialogueContext context = new DialogueContext(ActorRegistry.Get(PlayerController.PlayerActorId).actorObject.ActorId, currentActor.ActorId);
		foreach (string command in response.commands)
		{
			DialogueScriptHandler.ExecuteCommand(command, context);
		}
	}

	private void ExitDialogue () {
		isInDialogue = false;
		OnExitDialogue?.Invoke ();
	}

	private static string EvaluatePhraseId(string id, DialogueContext context)
	{
		Actor speaker = ActorRegistry.Get(context.speakerActorId).actorObject;
		PersonalityData personality = ContentLibrary.Instance.Personalities.GetById(speaker.GetData().Personality);
		DialoguePack dialogue = personality.GetDialoguePack();
		string ActorPhrase = dialogue.GetLine(id);

		if (ActorPhrase != null)
		{
			ActorPhrase = DialogueScriptHandler.PopulatePhrase(ActorPhrase, context);
			return ActorPhrase;
		}
		else
		{
			Debug.LogWarning("Line in master dialogue file \"" + id + "\" isn't a valid phrase ID");
			return id;
		}
	}

	private bool TryFindStartingNode (Actor Actor, out DialogueDataMaster.DialogueNode startNode) {
		List<DialogueDataMaster.DialogueNode> possibleNodes = new List<DialogueDataMaster.DialogueNode> ();
		foreach (DialogueDataMaster.DialogueNode node in DialogueDataMaster.DialogueNodes) {
			if (!node.isStartDialogue) { // Only look at nodes that have the isStart element set to true
				continue;
			}
			bool meetsConditions = true;
			foreach (string condition in node.preconditions) {
				if (!DialogueScriptHandler.CheckCondition (condition, Actor)) {
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
