using System.Collections;
using System.Collections.Generic;
using Popcron.Console;
using UnityEngine;

// Stores references from Actor IDs to their respective Actor objects in the scene
public static class ActorObjectRegistry
{
	static IDictionary<string, Actor> objectDict;
	static bool hasInited = false;

	static void Init ()
	{
		objectDict = new Dictionary<string, Actor>();
		SceneChangeActivator.OnSceneExit += OnUnitySceneExit;
		hasInited = true;
	}

	static void OnUnitySceneExit ()
	{
		objectDict.Clear();
		hasInited = false;
	}

	public static Actor GetActorObject (string npcId)
	{
		if (!hasInited)
			Init();

		if (objectDict.ContainsKey(npcId))
			return objectDict[npcId];
		else
			return null;
	}
	public static List<Actor> GetAllActors()
	{
		return objectDict != null ? new List<Actor>(objectDict.Values) : null;
	}
	public static void UnregisterActorObject (string npcId)
	{
		if (!hasInited)
			Init();

		else if (objectDict.ContainsKey(npcId))
			objectDict.Remove(npcId);
	}
	public static void RegisterActorObject (Actor actor)
	{
		if (!hasInited)
			Init();

		if (actor == null)
		{
			Debug.LogError("Tried to register a null Actor object!");
			return;
		}

		if (objectDict.ContainsKey(actor.ActorId))
		{
			objectDict[actor.ActorId] = actor;
			return;
		}

		objectDict.Add(actor.ActorId, actor);
	}

	[Command("DebugActorRegistry")]
	public static void DebugRegisteredActors()
	{
		foreach (string id in objectDict.Keys)
		{
			Console.Print("	" + id + " : " + objectDict[id].ActorName);
		}
	}
}
