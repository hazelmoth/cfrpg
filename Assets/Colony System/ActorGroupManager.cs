using System.Collections;
using System.Collections.Generic;

// Stores all groups in the world, and handles accessing them
public static class ActorGroupManager
{
	public static List<ActorGroupData> ActorGroups { get; private set; }

	public static void LoadSettlements(List<ActorGroupData> settlements)
	{
		ActorGroups = settlements;
	}

	/// <returns>The unique ID of the newly created settlement.</returns>
	public static string CreateSettlement (string name, string founderId)
	{
        List<string> members = new List<string>();
        members.Add(founderId);

		string id = name.Replace(' ', '_').ToLower();

        // Make sure group ids are unique
        foreach (ActorGroupData group in ActorGroups)
        {
            if (group.GroupId == id)
            {
                id += "0";
            }
        }

		ActorGroupData settlement = new ActorGroupData(id, name, members);
		ActorGroups.Add(settlement);
		return id;
	}
}
