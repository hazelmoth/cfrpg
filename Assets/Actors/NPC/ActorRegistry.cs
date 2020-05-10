using System.Collections;
using System.Collections.Generic;
using Popcron.Console;
using UnityEngine;

// Stores references from Actor IDs to their respective data and game objects in the scene
public static class ActorRegistry
{
	private static IDictionary<string, ActorInfo> actors;
	private static bool hasInited = false;

	private static void Init ()
	{
		actors = new Dictionary<string, ActorInfo>();
		SceneChangeActivator.OnSceneExit += OnUnitySceneExit;
		hasInited = true;
	}

	private static void OnUnitySceneExit ()
	{
		actors.Clear();
		hasInited = false;
	}

	public static ActorInfo Get (string ActorId)
	{
		if (!hasInited)
			Init();

		if (ActorId != null && actors.ContainsKey(ActorId))
			return actors[ActorId];
		else
			return null;
	}
	public static List<string> GetAllIds()
	{
		return actors != null ? new List<string>(actors.Keys) : null;
	}
	public static void RegisterActor(ActorData data)
	{
		RegisterActor(data, null);
	}
	public static void RegisterActor(ActorData data, Actor game)
	{
		if (actors.ContainsKey(data.actorId))
		{
			Debug.LogWarning("Registering actor to already registered ID \"" + data.actorId + "\"!");
		}
		actors.Add(data.actorId, new ActorInfo(data, game));
	}
	public static void RegisterActorGameObject(Actor actor)
	{
		if (!hasInited)
			Init();

		if (actor == null)
		{
			Debug.LogError("Tried to register a null Actor object!");
			return;
		}

		if (actors.ContainsKey(actor.ActorId))
		{
			actors[actor.ActorId].gameObject = actor;
		}
		else
		{
			Debug.LogError("Tried to register a gameobject for an unregistered Actor!");
		}
	}
	public static bool IdIsRegistered(string actorId)
	{
		return actors.ContainsKey(actorId);
	}
	public static void UnregisterActorGameObject (string ActorId)
	{
		if (!hasInited)
			Init();

		else if (actors.ContainsKey(ActorId))
		{
			actors[ActorId].gameObject = null;
		}
	}


	[Command("DebugActorRegistry")]
	public static void DebugRegisteredActors()
	{
		foreach (string id in actors.Keys)
		{
			Console.Print("	" + id + " : " + actors[id].data.ActorName + ", " + actors[id].gameObject);
		}
	}

	public class ActorInfo
	{
		public ActorInfo(ActorData data, Actor gameObject)
		{
			this.data = data;
			this.gameObject = gameObject;
		}
		public ActorData data;
		public Actor gameObject;
	}

	public static string GetUnusedId(string name)
	{
		if (name == null) name = "";

		string id = name.ToLower().Replace(' ', '_');

		if (Get(id) == null)
		{
			return id;
		}

		int num = 1;
		while (Get(id + "_" + num) != null)
		{
			num++;
		}
		return id + "_" + num;
	}
}
