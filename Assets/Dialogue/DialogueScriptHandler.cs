using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public static class DialogueScriptHandler {

	private const string ExpressionRegex = @"(?<=\<).*?(?=\>)";

	public static bool CheckCondition (string condition, NPC npc) 
	{
		string key = GetConditionKey(condition);
		string value = GetConditionValue (condition);
		string operatorStr = GetConditionOperator(condition);

		NPCData npcData = NPCDataMaster.GetNpcFromId (npc.ActorId);
		switch (key) 
		{
		case "name":
			return (npcData.NpcName == value);
		case "relationship":
			if (npcData.Relationships.Count == 0)
				return false;
			// TODO handle specific relationships instead of only the relationship with the player
			if (operatorStr == "==")
				return (npcData.Relationships[0].value == float.Parse (value));
			else if (operatorStr == ">=")
				return (npcData.Relationships[0].value >= float.Parse (value));
			else if (operatorStr == "<=")
				return (npcData.Relationships[0].value <= float.Parse (value));
			
			Debug.LogError ("DialogueScriptHandler is trying to handle a comparison operator, \""
			+ operatorStr + "\", which is not == nor >= nor <=");
			return false;
		default:
			return false;
		}
	}

	public static string PopulatePhrase(string phrase, DialogueContext context)
	{
		while (Regex.IsMatch(phrase, ExpressionRegex))
		{
			Match match = Regex.Match(phrase, ExpressionRegex);
			phrase = phrase.Substring(0, match.Index - 1) + EvaluateExpression(match.Value, context) +
			         phrase.Substring(match.Index + match.Length + 1);
		}
		return phrase;
	}

	static string GetConditionKey (string condition) 
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
	static string GetConditionValue (string condition) 
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
	static string GetConditionOperator (string condition) 
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
			subject = ActorObjectRegistry.GetActorObject(context.targetActorId);
		}
		else
		{
			subject = ActorObjectRegistry.GetActorObject(context.speakerActorId);
		}

		switch (expression.Split('.')[1].ToUpper())
		{
			case "NAME":
				return subject.ActorName;
			default:
				return null;
		}
	}
}
