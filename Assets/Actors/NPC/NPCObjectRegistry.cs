using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Stores references from NPC IDs to their respective NPC objects in the scene
public static class NPCObjectRegistry
{
	static IDictionary<string, NPC> objectDict;

	static void Init ()
	{
		objectDict = new Dictionary<string, NPC>();
		SceneChangeManager.OnSceneExit += OnUnitySceneExit;
	}

	static void OnUnitySceneExit ()
	{
		objectDict.Clear();
	}

	public static NPC GetNPCObject (string npcId)
	{
		if (objectDict == null)
		{
			objectDict = new Dictionary<string, NPC>();
			return null;
		}
		if (objectDict.ContainsKey(npcId))
			return objectDict[npcId];
		else
			return null;
	}
	public static List<NPC> GetAllNpcs()
	{
		return objectDict != null ? new List<NPC>(objectDict.Values) : null;
	}
	public static void UnregisterNpcObject (string npcId)
	{
		if (objectDict == null)
		{
			objectDict = new Dictionary<string, NPC>();
		}
		else if (objectDict.ContainsKey(npcId))
			objectDict.Remove(npcId);
	}
	public static void RegisterNPCObject (NPC npc)
	{
		if (npc == null)
		{
			Debug.LogError("Tried to register a null NPC object!");
			return;
		}

		if (objectDict == null)
		{
			objectDict = new Dictionary<string, NPC>();
		}
		else if (objectDict.ContainsKey(npc.NpcId))
		{
			objectDict[npc.NpcId] = npc;
			return;
		}

		objectDict.Add(npc.NpcId, npc);
	}
}
