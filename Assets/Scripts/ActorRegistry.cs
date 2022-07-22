using System.Collections.Generic;
using Popcron.Console;
using UnityEngine;

/// Stores references from Actor IDs to their respective data and game objects in the scene
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
		if (!hasInited) Init();

		if (ActorId != null && actors.ContainsKey(ActorId))
			return actors[ActorId];
		else
			return null;
	}
	public static List<string> GetAllIds()
	{
		return actors != null ? new List<string>(actors.Keys) : null;
	}
	public static void Register(ActorData data)
	{
		Register(data, null);
	}
	public static void Register(ActorData data, Actor gameObject)
	{
		if (actors.ContainsKey(data.ActorId))
		{
			Debug.LogWarning("Registering actor to already registered ID \"" + data.ActorId + "\"!");
		}
		actors.Add(data.ActorId, new ActorInfo(data, gameObject));
	}
	public static void RegisterActorGameObject(Actor actor)
	{
		if (!hasInited) Init();

		if (actor == null)
		{
			Debug.LogError("Tried to register a null Actor object!");
			return;
		}

		if (actors.ContainsKey(actor.ActorId))
		{
			actors[actor.ActorId].actorObject = actor;
		}
		else
		{
			Debug.LogError("Tried to register a gameobject for an unregistered Actor!");
		}
	}
	public static bool IdIsRegistered(string actorId)
	{
		if (!hasInited) Init();
		return actors.ContainsKey(actorId);
	}
	
	public static bool IdIsAvailable(string actorId)
	{
		if (!hasInited) Init();
		return !actors.ContainsKey(actorId);
	}
	
	public static void UnregisterActorGameObject (string ActorId)
	{
		if (!hasInited) Init();

		else if (actors.ContainsKey(ActorId))
		{
			actors[ActorId].actorObject = null;
		}
	}


	[Command("DebugActorRegistry")]
	public static void DebugRegisteredActors()
	{
		foreach (string id in actors.Keys)
		{
			Console.Print("	" + id + " : " + actors[id].data.ActorName + ", " + actors[id].actorObject);
		}
	}

	public class ActorInfo
	{
		public ActorInfo(ActorData data, Actor actorObject)
		{
			this.data = data;
			this.actorObject = actorObject;
		}
		public ActorData data;
		public Actor actorObject;
	}
}
