using System;
using System.Linq;
using ActorComponents;
using AI;
using ContentLibraries;
using Items;
using Popcron.Console;
using SettlementSystem;
using UnityEngine;
using ItemData = Items.ItemData;
using Object = UnityEngine.Object;

/// Provides a set of console commands for debugging purposes.
public static class DebugCommands
{
	[Command("clockscale")]
	public static float ClockTimeScale
	{
		get { return TimeKeeper.timeScale; }
		set { TimeKeeper.timeScale = value; }
	}

	[Command("date")]
	public static string GetDateTime()
	{
		return TimeKeeper.CurrentDateTime.ToString();
	}

	[Command("debugactor")]
	public static void DebugActor(string actorId)
	{
		string output = ("\nDebugging actor with ID: " + actorId + "\n\n");
		ActorRegistry.ActorInfo info = ActorRegistry.Get(actorId);
		if (info == null)
		{
			output += ("Actor not found.\n");
			Debug.Log(output);
			return;
		}

		output += $"Player-controlled: {info.actorObject != null && info.actorObject.PlayerControlled}\n";
		output += ("Name: " + info.data.ActorName + "\n");
		output += ("Race: " + info.data.RaceId + "\n");
		output += ("Components: " + string.Join("\n", info.data.GetComponents().Select(c => c.ToString())));

		output += ("Profession: " + info.data.RoleId + "\n");
		output += ("Spawned: " + (info.actorObject != null) + "\n");

		if (info.actorObject != null)
		{
			output += "Scene: " + info.actorObject.CurrentScene + "\n";
			output += "Top-level Task: "
				+ info.actorObject.GetComponent<ActorBehaviourExecutor>().CurrentBehaviourName
				+ "\n";
			output += "Hostile targets:\n";
			output = info.actorObject.HostileTargets.Aggregate(
				output,
				(current, actor) => current + $"\t{actor.ActorId}\n");
			output += "\n";
			output += "Behaviour Tree Debug:\n";
			output += TreeDebugger.DebugTree(
				info.actorObject.GetComponent<ActorBehaviourExecutor>().CurrentBehaviourTree);
			output += "\n";
		}

		Debug.Log(output);
	}

	[Command("debugall")]
	public static void DebugAllActors()
	{
		foreach (string id in ActorRegistry.GetAllIds())
		{
			DebugActor(id);
		}
	}

	[Command("debugitem")]
	public static string DebugCurrentItem()
	{
		ActorData playerData = ActorRegistry.Get(PlayerController.PlayerActorId).data;
		ActorInventory inventory = playerData.Get<ActorInventory>();
		if (inventory == null) return "Player has no ActorInventory component.";

		ItemStack item = inventory.EquippedItem;
		return item != null ? item.Id : "No item equipped.";
	}

	[Command("debugme")]
	public static void DebugMe()
	{
		DebugActor(PlayerController.PlayerActorId);
	}

	/// Debugs the actor nearest to the player.
	[Command("debugnearest")]
	public static void DebugNearestActor()
	{
		Actor player = ActorRegistry.Get(PlayerController.PlayerActorId).actorObject;
		Actor nearest = null;
		float nearestDistance = float.MaxValue;
		foreach (Actor actor in ActorRegistry.GetAllIds().Select(id => ActorRegistry.Get(id).actorObject))
		{
			if (actor == null) continue;
			if (actor.ActorId == PlayerController.PlayerActorId) continue;

			float distance = Vector3.Distance(actor.transform.position, player.transform.position);
			if (distance < nearestDistance)
			{
				nearest = actor;
				nearestDistance = distance;
			}
		}

		if (nearest != null)
			DebugActor(nearest.ActorId);
		else
			Console.Print("No actors found.");
	}

	[Command("debugsm")]
	public static void DebugSettlementManager()
	{
		SettlementManager sm = Object.FindObjectOfType<SettlementManager>();
		if (sm == null)
		{
			Debug.Log("Settlement manager not found.");
			return;
		}

		sm.DebugSettlements();
	}

	[Command("die")]
	public static void KillPlayer()
	{
		ActorHealth playerHealth = ActorRegistry.Get(PlayerController.PlayerActorId).data.Get<ActorHealth>();
		if (playerHealth == null)
		{
			Debug.Log("Player has no ActorHealth component.");
			return;
		}
		playerHealth.TakeHit(1000000);
	}

	[Command("fadeout")]
	public static void FadeOutScreen()
	{
		ScreenFadeAnimator.FadeOut(0.25f);
	}

	[Command("fadein")]
	public static void FadeInScreen()
	{
		ScreenFadeAnimator.FadeIn(0.25f);
	}

	[Command("followme")]
	public static void FollowPlayer(string actorId)
	{
		Actor actor = ActorRegistry.Get(actorId).actorObject;
		actor.GetData().Get<FactionStatus>().AccompanyTarget =
			ActorRegistry.Get(PlayerController.PlayerActorId).actorObject.ActorId;
	}

	[Command("formattedtime")]
	public static string GetFormattedTime()
	{
		return TimeKeeper.FormattedTime;
	}

	[Command("getfaction")]
	public static string GetFaction(string actorId)
	{
		Actor actor = ActorRegistry.Get(actorId).actorObject;
		if (actor.GetData().Get<FactionStatus>() == null) return "Actor has no FactionStatus component.";
		string id = actor.GetData().Get<FactionStatus>().FactionId;
		if (id == null)
		{
			Console.Print(actor.GetData().ActorName + " is not in a faction.");
			return null;
		}

		string name = FactionManager.Get(id).GroupName;
		Debug.Log(actor.GetData().ActorName + " is a member of " + name + "");
		return id;
	}

	[Command("getfaction")]
	public static string GetPlayerFaction()
	{
		FactionStatus faction = PlayerController.GetPlayerActor().GetData().Get<FactionStatus>();
		if (faction == null) return "Player has no FactionStatus component.";
		string id = faction.FactionId;
		if (id == null)
		{
			Console.Print("Player is not in a faction.");
			return null;
		}

		string name = FactionManager.Get(id).GroupName;
		Debug.Log("Player is a member of " + name + "");
		return id;
	}

	[Command("give")]
	public static void Give(string itemId)
	{
		Actor player = ActorRegistry.Get(PlayerController.PlayerActorId).actorObject;
		ActorInventory inventory = player.GetData().Get<ActorInventory>();
		if (inventory == null)
		{
			Console.Print("Player has no ActorInventory component.");
			return;
		}
		inventory.AttemptAddItem(new ItemStack(itemId, 1));
	}

	[Command("give")]
	public static void Give(int count, string itemId)
	{
		for (int i = 0; i < count; i++)
		{
			Give(itemId);
		}
	}

	[Command("give")]
	public static void Give(string actorId, int count, string itemId)
	{
		ActorInventory inventory = ActorRegistry.Get(actorId).data.Get<ActorInventory>();
		if (inventory == null)
		{
			Console.Print("Actor has no ActorInventory component.");
			return;
		}

		for (int i = 0; i < count; i++)
		{
			Actor actor = ActorRegistry.Get(actorId).actorObject;
			ItemData itemData = ContentLibrary.Instance.Items.Get(itemId);
			inventory.AttemptAddItem(new ItemStack(itemData));
		}
	}

	[Command("inmyfaction")]
	public static bool InMyFaction(string actorId)
	{
		Actor actor = ActorRegistry.Get(actorId).actorObject;
		string myFaction = ActorRegistry.Get(PlayerController.PlayerActorId).data.Get<FactionStatus>().FactionId;
		string otherFaction = actor.GetData().Get<FactionStatus>().FactionId;
		if (myFaction == null || otherFaction == null) return false;
		return myFaction == otherFaction;
	}

	[Command("notify")]
	public static void Notify(string msg)
	{
		NotificationManager.Notify(msg);
	}

	[Command("possess")]
	public static void Possess(string actor)
	{
		if (ActorRegistry.Get(actor) == null)
		{
			Console.Print("Actor not found.");
			return;
		}

		PlayerController.SetPlayerActor(actor);
	}


	[Command("recruit")]
	public static void Recruit(string actorId)
	{
		Actor actor = ActorRegistry.Get(actorId).actorObject;
		FactionStatus faction = actor.GetData().Get<FactionStatus>();
		if (faction == null)
		{
			Console.Print("Actor has no FactionStatus component.");
			return;
		}
		FactionStatus playerFaction = PlayerController.GetPlayerActor().GetData().Get<FactionStatus>();
		if (playerFaction == null)
		{
			Console.Print("Player has no FactionStatus component.");
			return;
		}

		// Create a new faction if the player doesn't already have one
		if (playerFaction.FactionId == null)
		{
			playerFaction.FactionId = FactionManager.CreateFaction(
				ActorRegistry.Get(PlayerController.PlayerActorId).actorObject.ActorId);
		}

		faction.FactionId = playerFaction.FactionId;
	}

	[Command("save")]
	public static void Save()
	{
		GameSaver.SaveGame(SaveInfo.SaveFileId);
	}

	[Command("setbalance")]
	public static void SetBalance(int amount)
	{
		ActorWallet wallet = PlayerController.GetPlayerActor().GetData().Get<ActorWallet>();
		if (wallet == null)
		{
			Console.Print("Player has no ActorWallet component.");
			return;
		}
		wallet.Balance = amount;
	}

	[Command("settime")]
	public static void SetTime(float time)
	{
		TimeKeeper.SetTimeOfDay(time);
	}

	[Command("spawn")]
	public static void SpawnDrifter(string templateId)
	{
		if (!ContentLibrary.Instance.ActorTemplates.Contains(templateId))
		{
			Console.Print("No actor template found with ID " + templateId);
			return;
		}

		AdvancedRandomizedActorTemplate template = ContentLibrary.Instance.ActorTemplates.Get(templateId);
		ActorData data = template.CreateActor(s => !ActorRegistry.IdIsRegistered(s), out _);
		ActorRegistry.Register(data);
		Actor player = ActorRegistry.Get(PlayerController.PlayerActorId).actorObject;
		ActorSpawner.Spawn(data.ActorId, player.Location.Vector2, player.Location.scene);
	}

	[Command("time")]
	public static ulong GetTime()
	{
		return TimeKeeper.CurrentTick;
	}

	[Command("timeofday")]
	public static float GetTimeAsFraction()
	{
		return TimeKeeper.TimeOfDay;
	}

	[Command("timescale")]
	public static float RealTimeScale
	{
		get { return Time.timeScale; }
		set { Time.timeScale = value; }
	}

	[Command("worldsize")]
	public static Vector2 WorldSize => SaveInfo.RegionSize;

	[Command("worldstate")]
	public static string PrintWorldState()
	{
		WorldState.WorldStateManager state = GameObject.FindObjectOfType<WorldState.WorldStateManager>();
		if (state == null)
		{
			return "World state object not found.";
		}

		return state.GetJson();
	}
}
