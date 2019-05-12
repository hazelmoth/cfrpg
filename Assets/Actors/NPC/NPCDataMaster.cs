using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Stores the master list of all NPCs that exist.
// An NPC must be in this list for its sprites to be loaded normally.
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

		return null;
	}

    public static void AddNPC (NPCData npc)
    {
        npcList.Add(npc);
    }
	public static string GetUnusedId (string name)
	{
		string id = name.ToLower().Replace(' ', '_');
		int num = 0;
		while (GetNpcFromId(id + "_" + num) != null)
		{
			num++;
		}
		return id + "_" + num;
	}
}

