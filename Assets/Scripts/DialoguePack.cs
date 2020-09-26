using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

// A DialoguePack stores all of the raw dialogue lines associated with a particular
// personality archetype.
public class DialoguePack
{
	private const string KeyRegex = @"(?<=\[).*?(?=\])";
	private const string CommentTrigger = "#";

	private readonly IDictionary<string, List<string>> lines;

	public DialoguePack(TextAsset txtFile)
	{
		lines = ParseDialogueFile(txtFile);
	}
	public string GetLine(string lineId)
	{
		if (lines.ContainsKey(lineId))
		{
			return lines[lineId][Random.Range(0, lines[lineId].Count)];
		}
		return null;
	}

	private static IDictionary<string, List<string>> ParseDialogueFile(TextAsset file)
	{
		IDictionary<string, List<string>> dict = new Dictionary<string, List<string>>();
		TextReader stream = new StringReader(file.text);
		string currentKey = null;
		while (true)
		{
			string line = stream.ReadLine();

			if (line == null) break; // Line will be null at end of stream
			if (string.IsNullOrWhiteSpace(line) || line.StartsWith(CommentTrigger)) continue;

			if (Regex.IsMatch(line, KeyRegex))
			{
				currentKey = Regex.Match(line, KeyRegex).Value;
			}
			else if (currentKey != null)
			{
				if (!dict.ContainsKey(currentKey))
				{
					dict.Add(currentKey, new List<string>());
				}
				dict[currentKey].Add(line.Trim());
			}
		}

		return dict;
	}
}
