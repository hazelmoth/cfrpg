using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Stores the master list of NPCs
public class NPCDataMaster : MonoBehaviour {
	 
	[SerializeField] TextAsset npcDataFile = null;

	static List<NPCData> npcList;

	// Use this for initialization
	void Start () {
		npcList = NPCDataParser.Parse (npcDataFile.text);
		if (npcList.Count == 0)
			Debug.LogWarning ("NPCDataMaster initialized without any NPCs in the NPC list! What gives?");
	}

	public static NPCData GetNpcFromId (string id) {
		foreach (NPCData npc in npcList) {
			if (npc.NpcId == id) {
				return npc;
			}
		}
		Debug.LogWarning ("NPCDataMaster was passed an NPC ID that doesn't seem to belong to any NPC!");
		// Return a default NPC
		return new NPCData (id, "Nameless Clone", "human_base", Gender.Male);
	}
}

