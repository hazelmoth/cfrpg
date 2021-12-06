using System;
using System.Collections.Immutable;
using System.Linq;
using Ink.Runtime;
using MyBox;
using UnityEngine;

namespace Dialogue
{
    /// A dialogue tree implemented in the Ink scripting language.
    public class InkDialogue
    {
        public enum Status
        {
            Line,
            Response,
            Ended
        }

        /// For commands that execute immediately after the previous line.
        private const string PostCommandPrefix = ">>>";
        /// For commands that execute immediately before the next line.
        private const string PreCommandPrefix = "...>>>";
        private readonly Action<string> commandHandler;

        private readonly Story story;

        public InkDialogue(string inkJson, Action<string> commandHandler, Func<string, object> evaluator)
        {
            story = new Story(inkJson);
            this.commandHandler = commandHandler;
            story.BindExternalFunction<string>("eval", evaluator.Invoke);
        }

        /// The ID of the non-player actor in the current conversation.
        public string ConversantActorId { get; private set; }

        /// Whether there are currently dialogue options available.
        private bool WaitingForDialogueChoice =>
            !story.canContinue && story.currentChoices.Any();

        /// Whether the dialogue exchange has reached a dead end.
        private bool Ended =>
            !story.canContinue && !WaitingForDialogueChoice;

        /// Starts a new conversation with the provided context.
        public void StartConversation(string conversantActorId)
        {
            ConversantActorId = conversantActorId;
            story.ResetState();
            ConsumeCommandsAndEmptyLines(PostCommandPrefix);
        }

        /// Advances dialogue to the next line or choice, or to the end of the
        /// conversation. Returns the current state of the dialogue tree.
        public DialogueState Next()
        {
            ConsumeCommandsAndEmptyLines(PreCommandPrefix);

            if (Ended) return DialogueState.Ended();

            if (WaitingForDialogueChoice)
                return DialogueState.OfferingChoices(GetAvailableChoices());

            string dialogueLine;
            try
            {
                dialogueLine = story.Continue();
                ConsumeCommandsAndEmptyLines(PostCommandPrefix);
            }
            catch (StoryException e)
            {
                dialogueLine = $"ERROR: {e.Message}";
                Debug.LogError(e);
            }
            return DialogueState.DeliveringLine(dialogueLine);
        }

        /// Returns the list of available choices, if any.
        private ImmutableList<DialogueChoice> GetAvailableChoices()
        {
            return story.currentChoices.Select(choice => new DialogueChoice(choice.text, choice.index))
                .ToImmutableList();
        }

        /// Chooses the given dialogue option.
        public void Choose(int choiceIndex)
        {
            story.ChooseChoiceIndex(choiceIndex);
            ConsumeCommandsAndEmptyLines(PostCommandPrefix);
        }

        /// Returns the next dialogue line without changing the state. Returns null if
        /// there is no dialogue line available.
        private string PeekNextLine()
        {
            // Store current state to prepare for future traversal.
            string storedState = story.state.ToJson();

            string text = !story.canContinue ? null : story.Continue();

            // Return to original state.
            story.state.LoadJson(storedState);

            return text;
        }

        /// Reads and executes commands until we reach a line that isn't a command.
        /// Continues past any blank lines.
        private void ConsumeCommandsAndEmptyLines(string commandPrefix)
        {
            if (!story.canContinue) return;

            string nextLine = PeekNextLine();
            ImmutableList<string> commands = ParseCommands(nextLine, commandPrefix);
            if (!string.IsNullOrWhiteSpace(nextLine) && commands.Count == 0) return;

            // This is a command line or blank line, so we continue on
            commands.ForEach(command => commandHandler.Invoke(command));
            story.Continue();
            ConsumeCommandsAndEmptyLines(commandPrefix);
        }


        /// Returns a list of commands if this is a command line;
        /// returns an empty list otherwise.
        private ImmutableList<string> ParseCommands(string line, string commandPrefix)
        {
            line = line.Trim();
            if (!line.StartsWith(commandPrefix) || line == commandPrefix)
                return ImmutableList<string>.Empty;

            return line.Split(new[] {commandPrefix}, StringSplitOptions.None)[1]
                .Split(',')
                .Select(s => s.Trim())
                .Where(s => !s.IsNullOrEmpty())
                .ToImmutableList();
        }

        public class DialogueChoice
        {
            public readonly int index;
            public readonly string text;

            public DialogueChoice(string text, int index)
            {
                this.text = text;
                this.index = index;
            }
        }

        public class DialogueState
        {
            public readonly ImmutableList<DialogueChoice> choices;
            public readonly string dialogueLine;
            public readonly Status status;

            private DialogueState(Status status, string dialogueLine, ImmutableList<DialogueChoice> choices)
            {
                this.status = status;
                this.dialogueLine = dialogueLine;
                this.choices = choices;
            }

            public static DialogueState DeliveringLine(string line)
            {
                return new DialogueState(Status.Line, line, null);
            }

            public static DialogueState OfferingChoices(ImmutableList<DialogueChoice> choices)
            {
                return new DialogueState(Status.Response, null, choices);
            }

            public static DialogueState Ended()
            {
                return new DialogueState(Status.Ended, null, null);
            }
        }
    }
}
