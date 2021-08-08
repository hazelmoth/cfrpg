using System;
using System.Reflection;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Popcron.Console;
using UnityEngine;

public static class DialogueScriptHandler {

	/// Captures terms inside '<' and '>', excluding those characters themselves
	private const string ExpressionRegex = @"(?<=\<).*?(?=\>)";

	private const int MaxExpressions = 100;

	public static bool CheckCondition (string condition, Actor actor) 
	{
		string key = GetConditionKey(condition);
		string value = GetConditionValue (condition);
		string operatorStr = GetConditionOperator(condition);

		ActorData actorData = ActorRegistry.Get (actor.ActorId).data;
		
		switch (key) 
		{
		case "name":
			return actorData.ActorName == value;
		case "relationship":
			if (actorData.Relationships == null || actorData.Relationships.Count == 0)
				return false;
			switch (operatorStr)
			{
				// TODO handle specific relationships instead of only the relationship with the player
				case "==":
					return (Math.Abs(actorData.Relationships[0].value - float.Parse (value)) < 0.0001);
				case ">=":
					return (actorData.Relationships[0].value >= float.Parse (value));
				case "<=":
					return (actorData.Relationships[0].value <= float.Parse (value));
				default:
					Debug.LogError ("DialogueScriptHandler is trying to handle a comparison operator, \""
						+ operatorStr + "\", which is not == nor >= nor <=");
					return false;
			}
		default:
			return false;
		}
	}

	public static string PopulatePhrase(string phrase, DialogueContext context)
	{
		int count = 0;
		while (Regex.IsMatch(phrase, ExpressionRegex) && count < MaxExpressions)
		{
			Match match = Regex.Match(phrase, ExpressionRegex);
			phrase = phrase.Substring(0, match.Index - 1) + EvaluateExpression(match.Value, context) +
			         phrase.Substring(match.Index + match.Length + 1);
			count++;
		}
		return phrase;
	}

	public static void ExecuteCommand(string command, DialogueContext context)
	{
		command = PopulatePhrase(command.Trim(), context);
		Debug.Log("Attempting to execute command \"" + command + "\"");
		Parser.Run(command);
	}

	private static string GetConditionKey (string condition) 
	{
		string key = "";
		if (condition.Contains (">="))
			key = condition.Split ('>') [0];
		else if (condition.Contains ("<="))
			key = condition.Split ('<') [0];
		else if (condition.Contains ("=="))
			key = condition.Split ('=') [0];
		else {
			Debug.LogError ("DialogueScriptHandler tried to parse a condition string that doesn't " +
			"seem to have a proper comparison operator. That string is: " + condition);
		}
		key = key.Trim ();
		return key;
	}

	private static string GetConditionValue (string condition) 
	{
		string value = "";
		if (condition.Contains (">="))
			value = condition.Split ('=') [1];
		else if (condition.Contains ("<="))
			value = condition.Split ('=') [1];
		else if (condition.Contains ("=="))
			value = condition.Split ('=') [2];
		else {
			Debug.LogError ("DialogueScriptHandler tried to parse a condition string that doesn't " +
				"seem to have a proper comparison operator. That string is: " + condition);
		}
		value = value.Trim ();
		return value;
	}

	private static string GetConditionOperator (string condition) 
	{
		if (condition.Contains (">="))
			return (">=");
		else if (condition.Contains ("<="))
			return ("<=");
		else if (condition.Contains ("=="))
			return ("==");
		else {
			Debug.LogError ("DialogueScriptHandler tried to parse a condition string that doesn't " +
				"seem to have a proper comparison operator. That string is: " + condition);
			return ("==");
		}
	}

	private static string EvaluateExpression(string expression, DialogueContext context)
	{
		Actor subject;
		string subjectString = expression.Split('.')[0];
		if (subjectString.ToLower() == "target")
		{
			subject = ActorRegistry.Get(context.targetActorId).actorObject;
		}
		else
		{
			subject = ActorRegistry.Get(context.speakerActorId).actorObject;
		}

		switch (expression.Split('.')[1].ToUpper())
		{
			case "NAME":
				return subject.GetData().ActorName;
			case "ID":
				return subject.ActorId;
			default:
				return null;
		}
	}

	/// Returns the result of evaluating the given expression for a conversation between
	/// the player and himself.
	[UsedImplicitly]
	[Command("testdialogueeval")]
	public static string TestDialogueEval(string expression)
	{
		string playerId = PlayerController.PlayerActorId;
		DialogueContext context = new DialogueContext(playerId, playerId);
		return EvaluateProperty(expression, context).ToString();
	}
	
	/// Uses reflection to access a property of ActorData for either the speaker or target
	/// of the given dialogue context. Provided expression must begin with either
	/// "speaker." or "target.", followed by the desired property, and, optionally, a
	/// sub-property.
	private static object EvaluateProperty(string expression, DialogueContext context)
	{
		ActorData actorData;
		if (expression.StartsWith("speaker.")) actorData = ActorRegistry.Get(context.speakerActorId).data;
		else if (expression.StartsWith("target.")) actorData = ActorRegistry.Get(context.targetActorId).data;
		else
		{
			Debug.LogError("Unknown subject for given expression: " + expression);
			return null;
		}
		
		try
		{
			string[] parts = expression.Split('.');
			string propertyName = parts[1];
			PropertyInfo propertyInfo = typeof(ActorData).GetProperty(propertyName);
			object propertyValue = propertyInfo!.GetValue(actorData);

			if (parts.Length != 3) return propertyValue;
			
			string subPropertyName = parts[2];
			PropertyInfo subPropertyInfo = propertyValue.GetType().GetProperty(subPropertyName);
			return subPropertyInfo!.GetValue(propertyValue);
		}
		catch (Exception e)
		{
			Debug.LogError($"Failed to read value {expression} in dialogue script.\n" + e);
			return null;
		}
	}
}
