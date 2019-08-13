using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Stores all settlements in the world, and handles accessing them
public class SettlementManager : MonoBehaviour
{
	static List<SettlementData> settlements;


	/// <returns>The unique ID of the newly created settlement.</returns>
	public static string CreateSettlement (string name, string leaderId)
	{
		// TDOD make sure settlement IDs are unique
		string id = name.Replace(' ', '_').ToLower();
		SettlementData settlement = new SettlementData(id, name, leaderId);
		settlements.Add(settlement);
		return id;
	}
}
