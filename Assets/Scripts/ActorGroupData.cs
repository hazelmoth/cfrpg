using System.Collections.Generic;

public class ActorGroupData
{
    public string GroupName { get; }
    public string GroupId { get; }
    public List<RosterEntry> GroupRoster { get; }


    public struct RosterEntry
    {
        public string actorId;
        public int rank;
    }

	public ActorGroupData(string id, string name, List<string> memberIds)
	{
		GroupId = id;
		GroupName = name;
        
        GroupRoster = new List<RosterEntry>();
        foreach (string memberId in memberIds)
        {
            AddMember(memberId, 0);
        }
	}

    public void AddMember (string actorId, int rank)
    {
        RosterEntry newEntry = new RosterEntry
        {
            actorId = actorId,
            rank = rank
        };
        GroupRoster.Add(newEntry);
    }

    public int GetRank (string actorId)
    {
        return GetMember(actorId).rank;
    }

    // Returns a blank roster entry if member doesn't exist
    private RosterEntry GetMember (string actorId)
    {
        foreach (RosterEntry member in GroupRoster)
        {
            if (member.actorId == actorId)
            {
                return member;
            }
        }
        return new RosterEntry();
    }
}
