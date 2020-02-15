using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Stores references from NPC IDs to their respective NPC objects in the scene
public static class NPCObjectRegistry
{
	static IDictionary<string, NPC> objectDict;
	static bool hasInited = false;

	static void Init ()
	{
		objectDict = new Dictionary<string, NPC>();
		SceneChangeActivator.OnSceneExit += OnUnitySceneExit;
		hasInited = true;
	}

	static void OnUnitySceneExit ()
	{
		objectDict.Clear();
		hasInited = false;
	}

	public static NPC GetNPCObject (string npcId)
	{
		if (!hasInited)
			Init();

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
		if (!hasInited)
			Init();

		else if (objectDict.ContainsKey(npcId))
			objectDict.Remove(npcId);
	}
	public static void RegisterNPCObject (NPC npc)
	{
		if (!hasInited)
			Init();

		if (npc == null)
		{
			Debug.LogError("Tried to register a null NPC object!");
			return;
		}

		if (objectDict.ContainsKey(npc.ActorId))
		{
			objectDict[npc.ActorId] = npc;
			return;
		}

		objectDict.Add(npc.ActorId, npc);
	}
}
