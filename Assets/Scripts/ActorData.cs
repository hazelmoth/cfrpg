using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ActorComponents;
using ContentLibraries;
using Newtonsoft.Json;
using UnityEngine;

/// An ActorData stores the data associated with an actor's identity, independent of their
/// current location or whether they are loaded in the scene.
[Serializable]
[JsonObject(MemberSerialization.OptIn)]
public class ActorData
{
	[JsonProperty(TypeNameHandling = TypeNameHandling.All)]
	private List<IActorComponent> components;

	[JsonProperty]
	private string actorId;
	
	public string ActorId => actorId;

	[JsonProperty]
	public string RaceId { get; set; }

	[JsonProperty]
	public string RoleId { get; set; }

	[JsonConstructor]
	public ActorData() { }

	public ActorData(string actorId, string actorRaceId, IEnumerable<IActorComponent> components)
	{
		this.actorId = actorId;
		RaceId = actorRaceId;
		this.components = components.ToList();
		CheckSerializable();
	}

	public ActorData(string actorId, string actorRaceId, params IActorComponent[] components)
	{
		this.actorId = actorId;
		RaceId = actorRaceId;
		this.components = components.ToList();
		CheckSerializable();
	}

	public T Get<T>() where T : IActorComponent
	{
		return components.OfType<T>().FirstOrDefault();
	}

	public ImmutableList<IActorComponent> GetComponents()
	{
		return components.ToImmutableList();
	}

	public string ActorName =>
		Get<ActorName>().value ?? ContentLibrary.Instance.Races.Get(RaceId).Name;

	/// Log an error if any components lack the [Serializable] attribute
	private void CheckSerializable()
	{
		foreach (IActorComponent component in components.Where(component => !component.GetType().IsSerializable))
		{
			Debug.LogError($"Component {component.GetType().Name} is not serializable");
		}
	}
}
