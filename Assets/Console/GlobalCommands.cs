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
		Actor actor = ActorObjectRegistry.GetActorObject(actorId);
		actor.FactionStatus.AccompanyTarget = Player.instance.ActorId;
	}

	[Command("GetFaction")]
	public static string GetFaction(string actorId)
	{
		Actor actor = ActorObjectRegistry.GetActorObject((actorId));
		string id = actor.FactionStatus.FactionId;
		if (id == null)
		{
			Console.print(actor.ActorName + " is not in a faction.");
			return null;
		}
		string name = FactionManager.Get(id).GroupName;
		Console.Print(actor.ActorName + " is a member of \"" + name + "\"");
		return id;
	}

	[Command("GetPlayerFaction")]
	public static string GetPlayerFaction()
	{
		string id = Player.instance.FactionStatus.FactionId;
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
		Actor player = Player.instance;
		Item itemData = ContentLibrary.Instance.Items.GetItemById(itemId);
		bool success = player.Inventory.AttemptAddItemToInv(itemData);
	}

	[Command("Give")]
	public static void Give(string actorId, string itemId)
	{
		Actor actor = ActorObjectRegistry.GetActorObject(actorId);
		Item itemData = ContentLibrary.Instance.Items.GetItemById(itemId);
		bool success = actor.Inventory.AttemptAddItemToInv(itemData);
	}

	[Command("InMyFaction")]
	public static bool InMyFaction(string actorId)
	{
		Actor actor = ActorObjectRegistry.GetActorObject(actorId);
		string myFaction = Player.instance.FactionStatus.FactionId;
		string otherFaction = actor.FactionStatus.FactionId;
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
		Actor actor = ActorObjectRegistry.GetActorObject(actorId);
		// Create a new faction if the player doesn't already have one
		if (Player.instance.FactionStatus.FactionId == null)
		{
			Player.instance.FactionStatus.FactionId = FactionManager.CreateFaction(Player.instance.ActorId);
		}
		actor.FactionStatus.FactionId = Player.instance.FactionStatus.FactionId;
	}
}
