using System.Collections.Generic;
using System.Linq;
using SimpleJSON;
using UnityEngine;

namespace Dialogue
{
	public class DialogueDataMaster : MonoBehaviour {
		private static DialogueDataMaster instance;

		private const string TypeDialogueNode = "dialogue";
		private const string TypeResponseNode = "response";
		[SerializeField] private TextAsset dialogueFile;

		private List<DialogueNode> dialogueNodes = new List<DialogueNode>();
		private List<GenericResponseNode> genResponseNodes = new List<GenericResponseNode>();

		// Use this for initialization
		private void Start () {
			instance = this;
			LoadDialogue ();
		}

		private void LoadDialogue () {
			// Dig through the JSON and file everything into structs
			JSONNode json = JSON.Parse (dialogueFile.text);
			for (int i = 0; i < JSONHelper.GetElementCount(json); i++) {
				if (json[i]["type"] == TypeDialogueNode) {
					DialogueNode node = new DialogueNode ();
					node.id = json [i] ["id"];
					node.isStartDialogue = json [i] ["isStart"];
					node.priority = json [i] ["priority"];
					node.blockGenericResponses = json [i] ["blockGenericResponses"];
					node.preconditions = new List<string> ();
					node.phrases = new List<DialoguePhrase> ();
					node.responses = new List<DialogueResponse> ();
					for (int j = 0; j < JSONHelper.GetElementCount(json[i]["preconditions"]); j++) {
						node.preconditions.Add (json [i] ["preconditions"] [j]);
					}
					for (int j = 0; j < JSONHelper.GetElementCount(json[i]["dialogue"]); j++) {
						DialoguePhrase dialogue = new DialoguePhrase
						{
							phraseId = json[i]["dialogue"][j]["text"], 
							commands = new List<string>()
						};
						for (int k = 0; k < JSONHelper.GetElementCount(json[i]["dialogue"][j]["commands"]); k++) {
							dialogue.commands.Add (json [i] ["dialogue"] [j] ["commands"] [k]);
						}
						node.phrases.Add (dialogue);
					}
					for (int j = 0; j < JSONHelper.GetElementCount(json[i]["responses"]); j++) {
						DialogueResponse response = new DialogueResponse
						{
							phraseId = json[i]["responses"][j]["text"],
							isExitResponse = json[i]["responses"][j]["isExit"],
							nextPhraseLink = json[i]["responses"][j]["nextSequence"],
							commands = new List<string>()
						};
						for (int k = 0; k < JSONHelper.GetElementCount(json[i]["responses"][j]["commands"]); k++) {
							response.commands.Add (json [i] ["responses"] [j] ["commands"] [k]);
						}
						node.responses.Add (response);
					}
					dialogueNodes.Add (node);
				} else if (json[i]["type"] == TypeResponseNode) {
					GenericResponseNode node = new GenericResponseNode ();
					node.preconditions = new List<string> ();
					node.response = new DialogueResponse ();
					for (int j = 0; j < JSONHelper.GetElementCount(json[i]["preconditions"]); j++) {
						node.preconditions.Add (json [i] ["preconditions"] [j]);
					}
					node.id = json [i] ["id"];
					node.response.phraseId = json [i] ["text"];
					node.response.isExitResponse = json [i] ["isExit"];
					node.response.nextPhraseLink = json [i] ["nextSequence"];
					node.response.commands = new List<string> ();
					for (int k = 0; k < JSONHelper.GetElementCount(json[i]["commands"]); k++) {
						node.response.commands.Add (json [i] ["commands"] [k]);
					}
					genResponseNodes.Add (node);
				}
			}
		}
		public static DialogueNode GetLinkedNodeFromResponse (DialogueResponse response) {
			string link = response.nextPhraseLink;
			return (GetNodeFromLink (link));
		}
		public static IEnumerable<DialogueNode> DialogueNodes => instance.dialogueNodes;

		public static IEnumerable<GenericResponseNode> ResponseNodes => instance.genResponseNodes;

		private static DialogueNode GetNodeFromLink (string link)
		{
			return DialogueNodes.FirstOrDefault(node => node.id == link);
		}
	
		public struct DialoguePhrase {
			public string phraseId;
			public List<string> commands;
		}
	
		public struct DialogueResponse {
			public string phraseId;
			public string nextPhraseLink;
			public bool isExitResponse;
			public List<string> commands;
		}
	
		public class DialogueNode {
			public string id;
			public bool isStartDialogue;
			public bool blockGenericResponses;
			public int priority;
			public List<string> preconditions;
			public List<DialoguePhrase> phrases;
			public List<DialogueResponse> responses;
		}
		public class GenericResponseNode {
			public string id;
			public List<string> preconditions;
			public DialogueResponse response;
		}
	}
}
