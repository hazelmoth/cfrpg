using System;
using System.Collections;
using System.Collections.Generic;
using Popcron.Console;
using UnityEngine;

public static class GlobalCommands
{
	[Command("ClockTimeScale")]
	public static float ClockTimeScale
	{
		get { return TimeKeeper.timeScale; }
		set { TimeKeeper.timeScale = value; }
	}

	[Command("FollowPlayer")]
	public static void FollowPlayer(string actorId)
	{
		Actor actor = ActorRegistry.Get(actorId).actorObject;
		actor.GetData().FactionStatus.AccompanyTarget = ActorRegistry.Get(PlayerController.PlayerActorId).actorObject.ActorId;
	}

	[Command("GetFaction")]
	public static string GetFaction(string actorId)
	{
		Actor actor = ActorRegistry.Get(actorId).actorObject;
		string id = actor.GetData().FactionStatus.FactionId;
		if (id == null)
		{
			Console.print(actor.GetData().ActorName + " is not in a faction.");
			return null;
		}
		string name = FactionManager.Get(id).GroupName;
		Console.Print(actor.GetData().ActorName + " is a member of \"" + name + "\"");
		return id;
	}

	[Command("GetPlayerFaction")]
	public static string GetPlayerFaction()
	{
		string id = ActorRegistry.Get(PlayerController.PlayerActorId).data.FactionStatus.FactionId;
		if (id == null)
		{
			Console.print("Player is not in a faction.");
			return null;
		}
		string name = FactionManager.Get(id).GroupName;
		Console.Print("Player is a member of \"" + name + "\"");
		return id;
	}

	[Command("Give")]
	public static void Give(string itemId)
	{
		Actor player = ActorRegistry.Get(PlayerController.PlayerActorId).actorObject;
		ItemData itemData = ContentLibrary.Instance.Items.Get(itemId);
		bool success = player.GetData().Inventory.AttemptAddItemToInv(itemData);
	}

	[Command("Give")]
	public static void Give(string actorId, string itemId)
	{
		Actor actor = ActorRegistry.Get(actorId).actorObject;
		ItemData itemData = ContentLibrary.Instance.Items.Get(itemId);
		bool success = actor.GetData().Inventory.AttemptAddItemToInv(itemData);
	}

	[Command("InMyFaction")]
	public static bool InMyFaction(string actorId)
	{
		Actor actor = ActorRegistry.Get(actorId).actorObject;
		string myFaction = ActorRegistry.Get(PlayerController.PlayerActorId).data.FactionStatus.FactionId;
		string otherFaction = actor.GetData().FactionStatus.FactionId;
		if (myFaction == null || otherFaction == null)
		{
			return false;
		}
		return myFaction == otherFaction;
	}

	[Command("RealTimeScale")]
	public static float RealTimeScale
	{
		get { return Time.timeScale; }
		set { Time.timeScale = value; }
	}

	[Command("Recruit")]
	public static void Recruit(string actorId)
	{
		Actor actor = ActorRegistry.Get(actorId).actorObject;
		// Create a new faction if the player doesn't already have one
		if (ActorRegistry.Get(PlayerController.PlayerActorId).data.FactionStatus.FactionId == null)
		{
			ActorRegistry.Get(PlayerController.PlayerActorId).data.FactionStatus.FactionId = FactionManager.CreateFaction(ActorRegistry.Get(PlayerController.PlayerActorId).actorObject.ActorId);
		}
		actor.GetData().FactionStatus.FactionId = ActorRegistry.Get(PlayerController.PlayerActorId).data.FactionStatus.FactionId;
	}

	[Command("SetTime")]
	public static void SetTime(float time)
	{
		TimeKeeper.SetTime(time);
	}
}
