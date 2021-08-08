using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ContentLibraries;
using MyBox;
using UnityEngine;

public class DialogueManager : MonoBehaviour {
	public static event Action OnExitDialogue;
	public static event Action OnRequestResponse;
	public static event Action<string> OnActorDialogueUpdate;
	public static event Action<ImmutableList<string>> OnAvailableResponsesUpdated;
	public static event Action<Actor, DialogueDataMaster.DialogueNode> OnInitiateDialogue;

	private static DialogueManager instance;
	/// The Actor the player is currently interacting with
	private Actor currentActor;
	/// The dialogue node that is currently being said or responded to
	private DialogueDataMaster.DialogueNode currentDialogueNode;
	/// The responses currently available to the player
	private List<DialogueDataMaster.DialogueResponse> currentDialogueResponses;
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
		OnActorDialogueUpdate = null;
		OnRequestResponse = null;
		OnAvailableResponsesUpdated = null;
	}

	private void InitiateDialogue (Actor actor) {
		isInDialogue = true;
		if (TryFindStartingNode(actor, out DialogueDataMaster.DialogueNode startNode)) {
			currentActor = actor;
			OnInitiateDialogue?.Invoke (actor, startNode);
			GoToDialogueNode (startNode);
		}
	}

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
		ImmutableList<string> responseStrings = currentDialogueResponses.Select(
				response => EvaluatePhraseId(
					response.phraseId,
					new DialogueContext(
						ActorRegistry.Get(PlayerController.PlayerActorId).actorObject.ActorId,
						currentActor.ActorId)))
			.ToImmutableList();
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
		string actorPhrase = dialogue.GetLine(id);
		if (actorPhrase != null)
			return DialogueScriptHandler.PopulatePhrase(actorPhrase, context);
		
		Debug.LogWarning("Line in master dialogue file \"" + id + "\" isn't a valid phrase ID");
		return id;
	}
	
	private static bool TryFindStartingNode (Actor actor, out DialogueDataMaster.DialogueNode startNode) {
		List<DialogueDataMaster.DialogueNode> possibleNodes = (
			from node in DialogueDataMaster.DialogueNodes
			where node.isStartDialogue
			where node.preconditions.TrueForAll(condition => DialogueScriptHandler.CheckCondition(condition, actor))
			select node).ToList();
		
		if (possibleNodes.Count == 0) {
			startNode = new DialogueDataMaster.DialogueNode ();
			return false;
		}
		startNode = possibleNodes.MaxBy(node => node.priority);
		return true;
	}
}
