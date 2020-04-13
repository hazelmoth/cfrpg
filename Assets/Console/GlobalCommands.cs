using System.Collections;
using System.Collections.Generic;
using Popcron.Console;
using UnityEngine;

public static class GlobalCommands
{
	[Command("Recruit")]
	public static void Recruit(string actorId)
	{
		Actor actor = ActorObjectRegistry.GetActorObject(actorId);
		actor.SettlementData.LeaderId = Player.instance.ActorId;
		Debug.Log("Recruit command called");
	}
}
