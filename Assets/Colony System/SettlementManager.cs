using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Stores all settlements in the world, and handles accessing them
public static class SettlementManager
{
	//TODO reset on scene exit
	public static List<SettlementData> Settlements { get; private set; }

	public static void LoadSettlements(List<SettlementData> settlements)
	{
		Settlements = settlements;
	}
	/// <returns>The unique ID of the newly created settlement.</returns>
	public static string CreateSettlement (string name, string leaderId)
	{
		// TODO make sure settlement IDs are unique
		string id = name.Replace(' ', '_').ToLower();
		SettlementData settlement = new SettlementData(id, name, leaderId);
		Settlements.Add(settlement);
		return id;
	}

}
