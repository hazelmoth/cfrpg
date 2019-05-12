using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueLibrary : MonoBehaviour
{
    public struct DialogueLine
	{
		public string personalityTypeId;
		public string dialogueId;
		public string dialogue;
	}
	// maps personality ids to lists of dialogue
	static Dictionary<string, List<DialogueLine>> dialogueDict;

    public static string GetDialogueLine (string personalityTypeId, string dialogueId)
	{
		if (dialogueDict.ContainsKey(personalityTypeId))
		{
			 foreach (DialogueLine line in dialogueDict[personalityTypeId])
			{
				if (line.dialogueId == dialogueId)
					return line.dialogue;
			}
			Debug.LogError("Personality type doesn't have a dialogue line with the given ID!");
			return null;
		}
		Debug.LogError("The given personality type doesn't exist!");
		return null;
	}
}
