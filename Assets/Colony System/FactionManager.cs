using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Stores all groups in the world, and handles accessing them
public static class FactionManager
{
	private const string DefaultFactionName = "guys";
	public static Dictionary<string, ActorGroupData> factions;

	public static void LoadSettlements(Dictionary<string, ActorGroupData> factions)
	{
		FactionManager.factions = factions;
	}

	/// <returns>The unique ID of the newly created settlement.</returns>
	public static string CreateFaction(string founderId)
	{
		Actor founder = ActorRegistry.Get(founderId).actorObject;
		string factionName = founder.GetData().ActorName + "'s " + DefaultFactionName;
		return CreateFaction(factionName, founderId);
	}

	/// <returns>The unique ID of the newly created settlement.</returns>
	public static string CreateFaction (string name, string founderId)
	{
		if (factions == null)
		{
			factions = new Dictionary<string, ActorGroupData>();
		}
		List<string> members = new List<string>();
        members.Add(founderId);

        string id = CreateUniqueFactionId(name);

		ActorGroupData settlement = new ActorGroupData(id, name, members);
		factions.Add(id, settlement);
		return id;
	}

	public static ActorGroupData Get(string id)
	{
		factions.TryGetValue(id, out ActorGroupData faction);
		return faction;
	}

	private static string CreateUniqueFactionId(string name)
	{
		string id = name.Replace(' ', '_').ToLower();
		id = id.Replace(@"'", ""); // Remove apostrophes
		bool isUnique = true;
		foreach (ActorGroupData group in factions.Values)
		{
			if (group.GroupId == id)
			{
				isUnique = false;
			}
		}

		if (isUnique)
		{
			return id;
		}
		int currentI = 2;

		while (true)
		{
			isUnique = true;
			// Make sure group ids are unique
			foreach (ActorGroupData group in factions.Values)
			{
				if (group.GroupId == id + "_" + currentI)
				{
					currentI++;
					isUnique = false;
					break;
				} 
			}

			if (isUnique)
			{
				return id + "_" + currentI;
			}
		}
	}
}
