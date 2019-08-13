using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettlementData
{

	string settlementId;
	string settlementName;

	string leaderActorId;
	List<string> memberActorIds;

	public SettlementData(string id, string name, string leaderId) : this(id, name, leaderId, new List<string>()) { }
	public SettlementData(string id, string name, string leaderId, List<string> memberIds)
	{
		this.settlementId = id;
		this.settlementName = name;
		this.leaderActorId = leaderId;
		this.memberActorIds = memberIds;
	}
}
