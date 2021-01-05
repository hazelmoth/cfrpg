using Popcron.Console;
using System.Reflection;
using UnityEngine;

public static class DebugCommands
{
	[Command("ClockTimeScale")]
	public static float ClockTimeScale
	{
		get { return TimeKeeper.timeScale; }
		set { TimeKeeper.timeScale = value; }
	}

	[Command("DateTime")]
	public static string GetDateTime()
	{
		return TimeKeeper.CurrentDateTime.ToString();
	}

	[Command("DebugActor")]
	public static void DebugActor(string actorId)
	{
		string output = ("\nDebugging actor with ID: \"" + actorId + "\"\n\n");
		ActorRegistry.ActorInfo info = ActorRegistry.Get(actorId);
		if (info == null)
		{
			output += ("Actor not found.\n");
			return;
		}

		output += ("Name: \"" + info.data.ActorName + "\"\n");
		output += ("Race: \"" + info.data.Race + "\"\n");
		output += ("Dead: " + info.data.PhysicalCondition.IsDead.ToString() + "\n");
		output += ("Sleeping: " + info.data.PhysicalCondition.Sleeping.ToString() + "\n");
		output += ("Faction ID: \"" + info.data.FactionStatus.FactionId + "\"\n");
		if (info.data.FactionStatus.FactionId != null)
		{
			output += ("Faction name: " + (FactionManager.Get(info.data.FactionStatus.FactionId) != null ? FactionManager.Get(info.data.FactionStatus.FactionId).GroupName : "Faction not found.") + "\n");
			output += ("In player faction: " + (info.data.FactionStatus.FactionId == ActorRegistry.Get(PlayerController.PlayerActorId).data.FactionStatus.FactionId) + "\n");
		}
		output += ("Profession: \"" + info.data.Profession + "\"\n");
		output += ("Spawned: " + (info.actorObject != null) + "\n");

		if (info.actorObject != null)
		{
			output += ("Scene: \"" + info.actorObject.CurrentScene + "\"\n");
			output += "Top-level Behaviour: " + info.actorObject.GetComponent<ActorBehaviourExecutor>().CurrentBehaviourName + "\n";
		}
		IAiBehaviour currentBehaviour = info.actorObject.GetComponent<ActorBehaviourExecutor>().CurrentBehaviour;
		if (currentBehaviour != null && currentBehaviour.GetType() == typeof(SettlerBehaviour))
		{
			output += "Settler sub-Behaviour: " + ((SettlerBehaviour)currentBehaviour).CurrentSubBehaviourName + "\n";
		}
		Debug.Log(output);
	}

	[Command("DebugAllActors")]
	public static void DebugAllActors()
	{
		foreach (string id in ActorRegistry.GetAllIds())
		{
			DebugActor(id);
		}
	}

	[Command("DebugCurrentItem")]
	public static string DebugCurrentItem()
	{
		ActorData playerData = ActorRegistry.Get(PlayerController.PlayerActorId).data;
		ItemStack item = playerData.Inventory.GetEquippedItem();
		if (item != null) {
			return item.id;
		}
		return "No item equipped.";
	}

	[Command("FadeOutScreen")]
	public static void FadeOutScreen()
	{
		ScreenFadeAnimator.FadeOut(0.25f);
	}

	[Command("FadeInScreen")]
	public static void FadeInScreen()
	{
		ScreenFadeAnimator.FadeIn(0.25f);
	}

	[Command("FollowPlayer")]
	public static void FollowPlayer(string actorId)
	{
		Actor actor = ActorRegistry.Get(actorId).actorObject;
		actor.GetData().FactionStatus.AccompanyTarget = ActorRegistry.Get(PlayerController.PlayerActorId).actorObject.ActorId;
	}

	[Command("FormattedTime")]
	public static string GetFormattedTime ()
	{
		return TimeKeeper.FormattedTime;
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
		Debug.Log(actor.GetData().ActorName + " is a member of \"" + name + "\"");
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
		Debug.Log("Player is a member of \"" + name + "\"");
		return id;
	}

	[Command("Give")]
	public static void Give(string itemId)
	{
		Actor player = ActorRegistry.Get(PlayerController.PlayerActorId).actorObject;
		bool success = player.GetData().Inventory.AttemptAddItem(new ItemStack(itemId, 1));
	}

	[Command("Give")]
	public static void Give(int count, string itemId)
	{
		for (int i = 0; i < count; i++)
		{
			Give(itemId);
		}
	}

	[Command("Give")]
	public static void Give(string actorId, int count, string itemId)
	{
		for (int i = 0; i < count; i++)
		{
			Actor actor = ActorRegistry.Get(actorId).actorObject;
			ItemData itemData = ContentLibrary.Instance.Items.Get(itemId);
			bool success = actor.GetData().Inventory.AttemptAddItem(new ItemStack(itemData));
		}
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

	[Command("Notify")]
	public static void Notify(string msg)
	{
		NotificationManager.Notify(msg);
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

	[Command("Save")]
	public static void Save()
	{
		GameSaver.SaveGame(GameDataMaster.SaveFileId);
	}

	[Command("SetBalance")]
	public static void SetBalance(int amount)
	{
		ActorRegistry.Get(PlayerController.PlayerActorId).data.Wallet.SetBalance(amount);
	}

	[Command("SetTime")]
	public static void SetTime(float time)
	{
		TimeKeeper.SetTimeOfDay(time);
	}

	[Command("Time")]
	public static ulong GetTime()
	{
		return TimeKeeper.CurrentTick;
	}

	[Command("TimeAsFraction")]
	public static float GetTimeAsFraction()
	{
		return TimeKeeper.TimeAsFraction;
	}

	[Command("WorldSize")]
	public static Vector2 WorldSize
	{
		get
		{
			return GameDataMaster.WorldSize;
		}
	}
}
