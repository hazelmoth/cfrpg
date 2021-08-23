using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ContentLibraries;
using MyBox;
using UnityEngine;

namespace Dialogue
{
	public class DialogueManager : MonoBehaviour {
		public static event Action OnExitDialogue;
		public static event Action OnRequestResponse;
		public static event Action<string> OnActorDialogueUpdate;
		public static event Action<ImmutableList<string>> OnAvailableResponsesUpdated;
		public static event Action<Actor> OnInitiateDialogue;

		private static DialogueManager instance;

		[SerializeField] private TextAsset inkJsonAsset;

		private InkStory dialogue;

		/// The Actor the player is currently interacting with
		private Actor currentActor;

		/// The responses currently available to the player
		private List<InkStory.DialogueChoice> currentDialogueResponses;

		private bool isAwaitingResponse = false;
		private bool isInDialogue = false;
		public static bool IsInDialogue => instance.isInDialogue;

		/// Returns the most recent dialogue line spoken to the player.
		public static string LastReceivedDialogueLine { get; private set; }

		// Use this for initialization
		private void Start () {
			instance = this;
			instance.dialogue = new InkStory(
				inkJsonAsset.text,
				command => DialogueScriptHandler.ExecuteCommand(command, DialogueContext.WithPlayer(instance.dialogue.ConversantActorId)),
				propertyName => DialogueScriptHandler.EvaluateProperty(propertyName, DialogueContext.WithPlayer(instance.dialogue.ConversantActorId))
					?? "");
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
			DialogueContext context = new DialogueContext(actor.ActorId, PlayerController.PlayerActorId);

				currentActor = actor;
				instance.dialogue.StartConversation(context.nonPlayerId);
				instance.dialogue.NextDialogue();
				instance.HandleLastDialogueLine();

				OnInitiateDialogue?.Invoke (actor);
				OnActorDialogueUpdate?.Invoke(instance.dialogue.LastDialogueLine);
		}

		/// Advances dialogue to the next node, which is either a dialogue line or a set
		/// of response options.
		public static void AdvanceDialogue ()
		{
			if (instance.dialogue.Ended)
			{
				instance.ExitDialogue();
				return;
			}
			if (instance.isAwaitingResponse)
			{
				Debug.LogWarning("Can't advance dialogue; awaiting response.");
				return;
			}

			if (instance.dialogue.WaitingForDialogueChoice)
			{
				instance.isAwaitingResponse = true;
				instance.HandleResponses();
			}
			else
			{
				instance.dialogue.NextDialogue();
				instance.HandleLastDialogueLine();
			}
		}

		public static void SelectDialogueResponse (int responseIndex) {
			Debug.Assert(instance.isAwaitingResponse);
			instance.dialogue.Choose(responseIndex);
			instance.isAwaitingResponse = false;
			AdvanceDialogue();
		}

		private void HandleResponses()
		{
			Debug.Assert(isAwaitingResponse);
			currentDialogueResponses = instance.dialogue.GetChoices().ToList();

			// Get response strings and call responses update event
			ImmutableList<string> responseStrings = currentDialogueResponses.Select(
					response => ProcessDialogue(
						response.text,
						new DialogueContext(PlayerController.PlayerActorId, currentActor.ActorId),
						isPlayerSpeaking: true))
				.ToImmutableList();

			OnAvailableResponsesUpdated?.Invoke(responseStrings);
			OnRequestResponse?.Invoke ();
		}

		private void HandleLastDialogueLine ()
		{
			// Get the actual dialogue string
			string dialogueText = instance.dialogue.LastDialogueLine;
			if (dialogueText != null)
			{
				dialogueText = ProcessDialogue(
					dialogueText,
					new DialogueContext(
						currentActor.ActorId,
						ActorRegistry.Get(PlayerController.PlayerActorId).actorObject.ActorId),
					isPlayerSpeaking: false);
			}
			else
			{
				Debug.LogWarning("Line is null");
			}
			OnActorDialogueUpdate?.Invoke(dialogueText);
		}

		private void ExitDialogue () {
			isInDialogue = false;
			OnExitDialogue?.Invoke ();
		}

		private static string ProcessDialogue(string id, DialogueContext context, bool isPlayerSpeaking)
		{
			string speakerActorId = isPlayerSpeaking ? context.playerId : context.nonPlayerId;
			Actor speaker = ActorRegistry.Get(speakerActorId).actorObject;
			PersonalityData personality = ContentLibrary.Instance.Personalities.GetById(speaker.GetData().Personality);
			DialoguePack dialogue = personality.GetDialoguePack();
			string actorPhrase = dialogue.GetLine(id);
			if (actorPhrase != null)
				return DialogueScriptHandler.PopulatePhrase(actorPhrase, context);

			Debug.LogWarning("Line in master dialogue file \"" + id + "\" isn't a valid phrase ID");
			return id;
		}
	}
}
