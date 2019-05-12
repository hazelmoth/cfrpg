using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class DialogueParser
{
	const string startOfDialogueId = "[";
	const string endOfDialogueId = "]";

	public static void ParseDialogue(string dialogueFileText)
	{
		// TODO figure this stuff out
	}
	public static List<DialogueLibrary.DialogueLine> ParseDialogueFile(string filePath)
	{
		List<DialogueLibrary.DialogueLine> dialogue = new List<DialogueLibrary.DialogueLine>();
		string currentDialogueId = null;

		StreamReader reader = new StreamReader(filePath);
		while (!reader.EndOfStream)
		{
			string line = reader.ReadLine();
			if (line != null)
			{
				if (line.StartsWith(startOfDialogueId) && line.EndsWith(endOfDialogueId))
				{
					currentDialogueId = line.Substring(1, line.Length - 2);
				}
				else
				{

				}
			}
		}
		Debug.Log(reader.ReadToEnd());
		reader.Close();
		return dialogue;
	}
}
