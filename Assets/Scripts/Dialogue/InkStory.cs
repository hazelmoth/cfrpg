using System;
using System.Collections.Immutable;
using System.Linq;
using Ink.Runtime;
using MyBox;
using UnityEngine;

namespace Dialogue
{
    /// A dialogue tree implemented in the Ink scripting language.
    public class InkStory
    {
        private const string CommandPrefix = ">>>";
        private readonly Action<string> commandHandler;

        private readonly Story story;

        public InkStory(string inkJson, Action<string> commandHandler, Func<string, object> evaluator)
        {
            story = new Story(inkJson);
            this.commandHandler = commandHandler;
            story.BindExternalFunction<string>("eval", evaluator.Invoke);
        }

        /// The most recent dialogue line (not option or command).
        public string LastDialogueLine { get; private set; }

        /// The ID of the non-player actor in the current conversation.
        public string ConversantActorId { get; private set; }

        /// Whether there are currently dialogue options available.
        public bool WaitingForDialogueChoice =>
            !story.canContinue && story.currentChoices.Any();

        /// Whether the dialogue exchange has reached a dead end.
        public bool Ended =>
            !story.canContinue && !WaitingForDialogueChoice;

        /// Starts a new conversation with the provided context.
        public void StartConversation(string conversantActorId)
        {
            ConversantActorId = conversantActorId;
            LastDialogueLine = null;
            story.ResetState();
            ConsumeCommandsAndEmptyLines();
        }

        /// Reads the next dialogue line, then executes any commands following it.
        /// Returns the dialogue line.
        public string NextDialogue()
        {
            Debug.Assert(ConversantActorId != null, "Context hasn't been set; there is no conversation!");
            if (!story.canContinue)
            {
                Debug.LogError("No dialogue line to read.");
                return null;
            }

            string result;
            try
            {
                result = story.Continue();
                ConsumeCommandsAndEmptyLines();
            }
            catch (StoryException e)
            {
                result = $"ERROR: {e.Message}";
                Debug.LogError(e);
            }

            LastDialogueLine = result;
            return result;
        }

        /// Returns the list of available choices, if any.
        public ImmutableList<DialogueChoice> GetChoices()
        {
            return story.currentChoices.Select(choice => new DialogueChoice(choice.text, choice.index))
                .ToImmutableList();
        }

        /// Chooses the given dialogue option.
        public void Choose(int choiceIndex)
        {
            story.ChooseChoiceIndex(choiceIndex);
            ConsumeCommandsAndEmptyLines();
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
        private void ConsumeCommandsAndEmptyLines()
        {
            if (!story.canContinue) return;

            string nextLine = PeekNextLine();
            ImmutableList<string> commands = ParseCommands(nextLine);
            if (!string.IsNullOrWhiteSpace(nextLine) && commands.Count == 0) return;

            // This is a command line or blank line, so we continue on
            commands.ForEach(command => commandHandler.Invoke(command));
            story.Continue();
            ConsumeCommandsAndEmptyLines();
        }


        /// Returns a list of commands if this is a command line;
        /// returns an empty list otherwise.
        private ImmutableList<string> ParseCommands(string line)
        {
            line = line.Trim();
            if (!line.StartsWith(CommandPrefix) || line == CommandPrefix)
                return ImmutableList<string>.Empty;

            return line.Split(new[] {CommandPrefix}, StringSplitOptions.None)[1]
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
    }
}
