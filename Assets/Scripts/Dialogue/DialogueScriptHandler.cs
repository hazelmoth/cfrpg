using System;
using System.Reflection;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Popcron.Console;
using UnityEngine;

namespace Dialogue
{
    public static class DialogueScriptHandler
    {
        /// Captures terms inside '&lt;' and '&gt;', excluding those characters themselves
        private const string ExpressionRegex = @"(?<=\<).*?(?=\>)";

        private const int MaxExpressions = 100;

        /// Populates any properties in the provided condition based on the provided context,
        /// and evaluates the condition based on the operator found in the string. Returns
        /// false if no valid operator is found.
        public static bool CheckCondition(string condition, DialogueContext context)
        {
            condition = PopulatePhrase(condition, context);
            string leftValue = ParseConditionKey(condition);
            string rightValue = ParseConditionLiteral(condition);
            string operatorStr = ParseConditionOperator(condition);
            try
            {
                return operatorStr switch
                {
                    "<" => double.Parse(leftValue) < double.Parse(rightValue),
                    ">" => double.Parse(leftValue) > double.Parse(rightValue),
                    "<=" => double.Parse(leftValue) <= double.Parse(rightValue),
                    ">=" => double.Parse(leftValue) >= double.Parse(rightValue),
                    "==" => rightValue.Equals(leftValue),
                    _ => throw new Exception("Invalid operator: " + operatorStr)
                };
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to apply operator '{operatorStr}' in '{condition}'.\n{e}");
                return false;
            }
        }

        /// Locates any expressions between "&lt;" and "&gt;" in the provided phrase, and
        /// replaces them with the result of evaluating them as property names.
        //* (That is, between < and >.)
        public static string PopulatePhrase(string phrase, DialogueContext context)
        {
            int count = 0;
            while (Regex.IsMatch(phrase, ExpressionRegex) && count < MaxExpressions)
            {
                Match match = Regex.Match(phrase, ExpressionRegex);
                phrase = phrase.Substring(0, match.Index - 1)
                    + EvaluateProperty(match.Value, context)
                    + phrase.Substring(match.Index + match.Length + 1);
                count++;
            }

            return phrase;
        }

        /// Populates any expressions (between "&lt;" and "&gt;") in the given command string and executes it.
        public static void ExecuteCommand(string command, DialogueContext context)
        {
            command = PopulatePhrase(command.Trim(), context);
            Debug.Log("Attempting to execute command \"" + command + "\"");
            Parser.Run(command);
        }

        /// Returns the part of the condition before the operator.
        private static string ParseConditionKey(string condition)
        {
            string key = condition.Split(new[] {ParseConditionOperator(condition)}, StringSplitOptions.None)[0];
            return key.Trim();
        }

        /// Returns the part of the condition after the operator.
        private static string ParseConditionLiteral(string condition)
        {
            string value = condition.Split(new[] {ParseConditionOperator(condition)}, StringSplitOptions.None)[1];
            return value.Trim();
        }

        private static string ParseConditionOperator(string condition)
        {
            if (condition.Contains(">=")) return ">=";

            if (condition.Contains("<=")) return "<=";

            if (condition.Contains("==")) return "==";

            if (condition.Contains(">")) return ">";

            if (condition.Contains("<"))
            {
                return "<";
            }

            Debug.LogError(
                "DialogueScriptHandler tried to parse a condition string that doesn't "
                + "seem to have a proper comparison operator: "
                + condition);
            return "==";
        }

        /// Locates a property of either the speaker or target based on the provided
        /// expression, and returns its value converted to a string.
        /// (propertyString example: player.Health.CurrentHealth)
        public static object EvaluateProperty(string propertyString, DialogueContext context)
        {
            string[] parts = propertyString.Split(new[] {'.'}, 2);
            string subjectString = parts[0];
            Actor subject = subjectString.ToLower() == "player"
                ? ActorRegistry.Get(context.playerId).actorObject
                : ActorRegistry.Get(context.nonPlayerId).actorObject;

            try
            {
                string[] propertyParts = parts[1].Split('.');
                string propertyName = propertyParts[0];
                PropertyInfo propertyInfo = typeof(ActorData).GetProperty(propertyName);
                object firstPropertyValue = propertyInfo!.GetValue(subject.GetData());
                if (propertyParts.Length == 1) return firstPropertyValue;

                string subPropertyName = propertyParts[1];
                return propertyInfo!.PropertyType.GetProperty(subPropertyName)!.GetValue(firstPropertyValue);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to read property {propertyString} in dialogue script.\n" + e);
                return null;
            }
        }

        /// Returns the result of evaluating the given expression for a conversation between
        /// the player and himself.
        [UsedImplicitly]
        [Command("testdialogueeval")]
        public static string TestDialogueEval(string expression)
        {
            string actorId = PlayerController.PlayerActorId;
            return EvaluateProperty(expression, new DialogueContext(actorId, actorId)).ToString();
        }

        /// Returns the result of checking the given condition for a conversation between
        /// the player and himself.
        [UsedImplicitly]
        [Command("testdialoguecondition")]
        public static bool TestDialogueCondition(string condition)
        {
            string actorId = PlayerController.PlayerActorId;
            return CheckCondition(condition, new DialogueContext(actorId, actorId));
        }
    }
}
