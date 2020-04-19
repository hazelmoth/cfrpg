﻿using System;
using System.Collections;
using System.Collections.Generic;
using Popcron.Console;
using UnityEngine;

public static class GlobalCommands
{
	[Command("FollowMe")]
	public static void FollowMe(string actorId)
	{
		Actor actor = ActorObjectRegistry.GetActorObject(actorId);
		throw new NotImplementedException();
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
