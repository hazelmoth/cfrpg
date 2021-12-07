using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ContentLibraries;
using UnityEngine;

namespace Dialogue
{
    public class DialogueManager : MonoBehaviour
    {
        private static DialogueManager instance;

        [SerializeField] private TextAsset inkJsonAsset;

        /// The Actor the player is currently interacting with
        private Actor currentActor;

        /// The responses currently available to the player
        private List<InkDialogue.DialogueChoice> currentDialogueResponses;

        private InkDialogue dialogue;

        private bool isAwaitingResponse = false;
        private bool isInDialogue = false;
        public static bool IsInDialogue => instance.isInDialogue;

        // Use this for initialization
        private void Start()
        {
            instance = this;
            instance.dialogue = new InkDialogue(
                inkJsonAsset.text,
                command => DialogueScriptHandler.ExecuteCommand(
                    command,
                    DialogueContext.WithPlayer(instance.dialogue.ConversantActorId)),
                propertyName => DialogueScriptHandler.EvaluateProperty(
                        propertyName,
                        DialogueContext.WithPlayer(instance.dialogue.ConversantActorId)));
            ActorInteractionHandler.OnInteractWithActor += InitiateDialogue;
        }

        private void OnDestroy()
        {
            OnInitiateDialogue = null;
            OnExitDialogue = null;
            OnActorDialogueUpdate = null;
            OnRequestResponse = null;
            OnAvailableResponsesUpdated = null;
        }

        public static event Action OnExitDialogue;
        public static event Action OnRequestResponse;
        public static event Action<string> OnActorDialogueUpdate;
        public static event Action<ImmutableList<string>> OnAvailableResponsesUpdated;
        public static event Action<Actor> OnInitiateDialogue;

        /// Begins dialogue between the player and the given actor, unless the player is
        /// already in dialogue with the an actor.
        public void InitiateDialogue(Actor actor)
        {
            if (isInDialogue) return;

            isInDialogue = true;
            DialogueContext context = new DialogueContext(PlayerController.PlayerActorId, actor.ActorId);

            currentActor = actor;
            instance.dialogue.StartConversation(context.nonPlayerId);
            OnInitiateDialogue?.Invoke(actor);
            AdvanceDialogue();
        }

        /// Advances dialogue to the next node, which is either a dialogue line or a set
        /// of response options.
        public static void AdvanceDialogue()
        {
            if (instance.isAwaitingResponse)
            {
                Debug.LogWarning("Can't advance dialogue; awaiting response.");
                return;
            }

            InkDialogue.DialogueState state = instance.dialogue.Next();

            switch (state.status)
            {
                case InkDialogue.Status.Ended:
                {
                    instance.ExitDialogue();
                    break;
                }
                case InkDialogue.Status.Response:
                {
                    instance.isAwaitingResponse = true;
                    instance.HandleResponses(state.choices);
                    break;
                }
                case InkDialogue.Status.Line:
                {
                    instance.HandleDialogueLine(state.dialogueLine);
                    break;
                }
            }
        }

        public static void SelectDialogueResponse(int responseIndex)
        {
            Debug.Assert(instance.isAwaitingResponse);
            instance.dialogue.Choose(responseIndex);
            instance.isAwaitingResponse = false;
            AdvanceDialogue();
        }

        private void HandleResponses(IList<InkDialogue.DialogueChoice> responses)
        {
            Debug.Assert(isAwaitingResponse);

            // Get response strings and call responses update event
            ImmutableList<string> responseStrings = responses.Select(
                    response => ProcessDialogue(
                        response.text,
                        new DialogueContext(PlayerController.PlayerActorId, currentActor.ActorId),
                        isPlayerSpeaking: true))
                .ToImmutableList();

            OnAvailableResponsesUpdated?.Invoke(responseStrings);
            OnRequestResponse?.Invoke();
        }

        private void HandleDialogueLine(string dialogueText)
        {
            if (dialogueText != null)
                dialogueText = ProcessDialogue(
                    dialogueText,
                    new DialogueContext(
                        PlayerController.PlayerActorId,
                        currentActor.ActorId),
                    isPlayerSpeaking: false);
            else Debug.LogWarning("Line is null");

            OnActorDialogueUpdate?.Invoke(dialogueText);
        }

        private void ExitDialogue()
        {
            isInDialogue = false;
            OnExitDialogue?.Invoke();
        }

        /// Replaces a dialogue line ID with the line in the appropriate dialogue file, if
        /// such a line exists. Otherwise, returns the original line ID. In either case,
        /// populates variables in the returned string with the appropriate values.
        private static string ProcessDialogue(string dialogueId, DialogueContext context, bool isPlayerSpeaking)
        {
            string speakerActorId = isPlayerSpeaking ? context.playerId : context.nonPlayerId;
            Actor speaker = ActorRegistry.Get(speakerActorId).actorObject;
            PersonalityData personality = ContentLibrary.Instance.Personalities.GetById(speaker.GetData().Personality);
            DialoguePack dialogue = personality.GetDialoguePack();
            string actorPhrase = dialogue.GetLine(dialogueId);
            if (actorPhrase != null)
                return DialogueScriptHandler.PopulatePhrase(actorPhrase, context);

            return DialogueScriptHandler.PopulatePhrase(dialogueId, context);;
        }
    }
}
