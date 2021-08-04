using AI;
using ContentLibraries;
using Popcron.Console;
using UnityEngine;

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
		output += ("Dead: " + info.data.PhysicalCondition.IsDead.ToString() + "\n");
		output += ("Sleeping: " + info.data.PhysicalCondition.Sleeping.ToString() + "\n");
		output += ("Faction ID: " + info.data.FactionStatus.FactionId + "\n");
		if (info.data.FactionStatus.FactionId != null)
		{
			output += ("Faction name: " + (FactionManager.Get(info.data.FactionStatus.FactionId) != null ? FactionManager.Get(info.data.FactionStatus.FactionId).GroupName : "Faction not found.") + "\n");
			output += ("In player faction: " + (info.data.FactionStatus.FactionId == ActorRegistry.Get(PlayerController.PlayerActorId).data.FactionStatus.FactionId) + "\n");
		}
		output += ("Profession: " + info.data.Profession + "\n");
		output += ("Spawned: " + (info.actorObject != null) + "\n");

		if (info.actorObject != null)
		{
			output += ("Scene: " + info.actorObject.CurrentScene + "\n");
			output += "Top-level Task: " + info.actorObject.GetComponent<ActorBehaviourExecutor>().CurrentBehaviourName + "\n";
			output += "Hostile targets:\n";
			foreach (Actor actor in info.actorObject.HostileTargets)
				output += $"\t{actor.ActorId}\n";
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
		ItemStack item = playerData.Inventory.EquippedItem;
		if (item != null) {
			return item.Id;
		}
		return "No item equipped.";
	}

	[Command("die")]
	public static void KillPlayer()
	{
		PlayerController.GetPlayerActor().GetData().PhysicalCondition.TakeHit(1000000);
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
		actor.GetData().FactionStatus.AccompanyTarget = ActorRegistry.Get(PlayerController.PlayerActorId).actorObject.ActorId;
	}

	[Command("formattedtime")]
	public static string GetFormattedTime ()
	{
		return TimeKeeper.FormattedTime;
	}

	[Command("getfaction")]
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
		Debug.Log(actor.GetData().ActorName + " is a member of " + name + "");
		return id;
	}

	[Command("getfaction")]
	public static string GetPlayerFaction()
	{
		string id = ActorRegistry.Get(PlayerController.PlayerActorId).data.FactionStatus.FactionId;
		if (id == null)
		{
			Console.print("Player is not in a faction.");
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
		bool success = player.GetData().Inventory.AttemptAddItem(new ItemStack(itemId, 1));
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
		for (int i = 0; i < count; i++)
		{
			Actor actor = ActorRegistry.Get(actorId).actorObject;
			ItemData itemData = ContentLibrary.Instance.Items.Get(itemId);
			bool success = actor.GetData().Inventory.AttemptAddItem(new ItemStack(itemData));
		}
	}

	[Command("inmyfaction")]
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
		// Create a new faction if the player doesn't already have one
		if (ActorRegistry.Get(PlayerController.PlayerActorId).data.FactionStatus.FactionId == null)
		{
			ActorRegistry.Get(PlayerController.PlayerActorId).data.FactionStatus.FactionId = FactionManager.CreateFaction(ActorRegistry.Get(PlayerController.PlayerActorId).actorObject.ActorId);
		}
		actor.GetData().FactionStatus.FactionId = ActorRegistry.Get(PlayerController.PlayerActorId).data.FactionStatus.FactionId;
	}

	[Command("save")]
	public static void Save()
	{
		GameSaver.SaveGame(SaveInfo.SaveFileId);
	}

	[Command("setbalance")]
	public static void SetBalance(int amount)
	{
		ActorRegistry.Get(PlayerController.PlayerActorId).data.Wallet.SetBalance(amount);
	}

	[Command("settime")]
	public static void SetTime(float time)
	{
		TimeKeeper.SetTimeOfDay(time);
	}

	[Command("spawndrifter")]
	public static void SpawnDrifter()
	{
		ActorData data = ActorGenerator.Generate();
		ActorRegistry.Register(data);
		Actor player = ActorRegistry.Get(PlayerController.PlayerActorId).actorObject;
		ActorSpawner.Spawn(data.actorId, player.Location.Vector2, player.Location.scene);
	}

	[Command("time")]
	public static ulong GetTime()
	{
		return TimeKeeper.CurrentTick;
	}

	[Command("timeofday")]
	public static float GetTimeAsFraction()
	{
		return TimeKeeper.TimeAsFraction;
	}
	
	[Command("timescale")]
	public static float RealTimeScale
	{
		get { return Time.timeScale; }
		set { Time.timeScale = value; }
	}

	[Command("worldsize")]
	public static Vector2 WorldSize => SaveInfo.RegionSize;
}
