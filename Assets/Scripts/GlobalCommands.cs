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

	[Command("DateTime")]
	public static string GetDateTime()
	{
		return TimeKeeper.CurrentDateTime.ToString();
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
		bool success = player.GetData().Inventory.AttemptAddItem(new Item(itemData));
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
			bool success = actor.GetData().Inventory.AttemptAddItem(new Item(itemData));
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

	[Command("WorldSize")]
	public static Vector2 WorldSize
	{
		get
		{
			return GameDataMaster.WorldSize;
		}
	}
}
