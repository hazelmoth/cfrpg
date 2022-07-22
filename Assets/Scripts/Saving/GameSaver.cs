using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using ContinentMaps;
using SettlementSystem;
using UnityEngine;
using WorldState;

public static class GameSaver
{
    public static void SaveGame (string saveId)
    {
	    WriteSave(GenerateWorldSave(), saveId);
    }

    // Returns a new WorldSave containing all the data in the world as it exists at present.
    private static WorldSave GenerateWorldSave ()
	{
        string worldName = SaveInfo.WorldName;
        if (worldName == null)
        {
            Debug.Log("No name for this save found. Looking for one from a recently created world.");
            worldName = GeneratedWorldSettings.worldName;
            if (worldName == null)
            {
                Debug.LogError("No current save name found!");
                worldName = "MissingName";
            }
        }
        // Make sure the current region is saved as part of the continent
        ContinentManager.SaveRegion(RegionMapManager.ExportRegionMap(), ContinentManager.CurrentRegionId);

        List<ActorData> actors = new List<ActorData> ();
		foreach (string actorId in ActorRegistry.GetAllIds())
		{
			ActorData actor = ActorRegistry.Get(actorId).data;
			// this is a live reference; might want to do a deep copy
			actors.Add(actor);
		}

		DroppedItemRegistry itemRegistry = GameObject.FindObjectOfType<DroppedItemRegistry>();
		List<SavedDroppedItem> items = new();
		if (itemRegistry != null)
		{
			foreach (DroppedItem item in itemRegistry.GetItems())
			{
				Vector2Serializable location = TilemapInterface
					.WorldPosToScenePos(item.transform.position.ToVector2(), item.Scene)
					.ToSerializable();
				SavedDroppedItem saved = new(location, item.GetScene(), item.Item);
				items.Add(saved);
			}
		} 
		else
		{
			Debug.LogError("Missing dropped item registry in scene!");
		}
		
		ulong time = TimeKeeper.CurrentTick;
		History.EventLog eventLog = Object.FindObjectOfType<History>().GetEventLog();
		SerializableWorldMap worldMap = ContinentManager.GetSaveData();
		string currentRegionId = ContinentManager.CurrentRegionId;

		Dictionary<string, SettlementManager.SettlementInfo> settlements =
			Object.FindObjectOfType<SettlementManager>()?.GetSettlements();
		settlements ??= new Dictionary<string, SettlementManager.SettlementInfo>();

		WorldStateManager worldStateManager = Object.FindObjectOfType<WorldStateManager>();
		MultiStringDict worldState =
			worldStateManager != null ? worldStateManager.GetDictionary() : new MultiStringDict();

		// TODO: wouldn't it be neat if the save file was just a generic list of saveable things?
		// Each game system that needs saving could just implement some interface with a
		// save and load method, and some ID. We could just search for MonoBehaviours that
		// implement that interface, and pass them the object matching their ID when loading.

		WorldSave save = new(
			worldName: worldName,
			time: time,
			playerActorId: PlayerController.PlayerActorId,
			eventLog: eventLog,
			regionSize: SaveInfo.RegionSize.ToSerializable(),
			worldMap: worldMap,
			currentRegionId: currentRegionId,
			actors: actors,
			settlements: settlements,
			worldState: worldState,
			newlyCreated: false);
		
		return save;
	}


	private static void WriteSave (WorldSave save, string saveId)
	{
		string savePath = Application.persistentDataPath + "/saves/" + saveId + ".cfrpg";

		// Create the directory if nonexistent
		if (!Directory.Exists(Path.GetDirectoryName(savePath)))
		{
			Directory.CreateDirectory(Path.GetDirectoryName(savePath));
		}

		

		StreamWriter writer = new(savePath, false);
		JsonTextWriter jwriter = new(writer);
		jwriter.Formatting = Formatting.Indented;

		JsonSerializer serializer = new()
		{
			TypeNameHandling = TypeNameHandling.Auto
		};
		serializer.Serialize(jwriter, save);

		writer.Close();
	}
}
