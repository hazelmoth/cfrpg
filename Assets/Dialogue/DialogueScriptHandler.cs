using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Popcron.Console;
using UnityEngine;

public static class DialogueScriptHandler {

	// Captures terms inside '<' and '>', excluding those characters themselves
	private const string ExpressionRegex = @"(?<=\<).*?(?=\>)";

	private const int MaxExpressions = 100;

	public static bool CheckCondition (string condition, Actor Actor) 
	{
		string key = GetConditionKey(condition);
		string value = GetConditionValue (condition);
		string operatorStr = GetConditionOperator(condition);

		ActorData ActorData = ActorRegistry.Get (Actor.ActorId).data;
		switch (key) 
		{
		case "name":
			return (ActorData.ActorName == value);
		case "relationship":
			if (ActorData.Relationships == null || ActorData.Relationships.Count == 0)
				return false;
			// TODO handle specific relationships instead of only the relationship with the player
			if (operatorStr == "==")
				return (ActorData.Relationships[0].value == float.Parse (value));
			else if (operatorStr == ">=")
				return (ActorData.Relationships[0].value >= float.Parse (value));
			else if (operatorStr == "<=")
				return (ActorData.Relationships[0].value <= float.Parse (value));
			
			Debug.LogError ("DialogueScriptHandler is trying to handle a comparison operator, \""
			+ operatorStr + "\", which is not == nor >= nor <=");
			return false;
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
}
