using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using ContinentMaps;
using UnityEngine;

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
        ContinentManager.SaveRegion(RegionMapManager.GetRegionMap(), ContinentManager.CurrentRegionId);

        List<SavedActor> actors = new List<SavedActor> ();
		foreach (string actorId in ActorRegistry.GetAllIds())
		{
			ActorData actor = ActorRegistry.Get(actorId).data;

			SavedActor actorSave = new SavedActor(actor);
			actors.Add(actorSave);
		}

		DroppedItemRegistry itemRegistry = GameObject.FindObjectOfType<DroppedItemRegistry>();
		List<SavedDroppedItem> items = new List<SavedDroppedItem>();
		if (itemRegistry != null)
		{
			foreach (DroppedItem item in itemRegistry.GetItems())
			{
				Vector2Serializable location = TilemapInterface.WorldPosToScenePos(item.transform.position.ToVector2(), item.Scene).ToSerializable();
				SavedDroppedItem saved = new SavedDroppedItem(location, item.GetScene(), item.Item);
				items.Add(saved);
			}
		} 
		else
		{
			Debug.LogError("Missing dropped item registry in scene!");
		}
		
		SerializableWorldMap worldMap = ContinentManager.GetSaveData();
		string currentRegionId = ContinentManager.CurrentRegionId;
		ulong time = TimeKeeper.CurrentTick;
		History.EventLog eventLog = GameObject.FindObjectOfType<History>().GetEventLog();
		
		WorldSave save = new WorldSave(
			worldName: worldName,
			time: time,
			playerActorId: PlayerController.PlayerActorId,
			eventLog: eventLog,
			regionSize: SaveInfo.RegionSize.ToSerializable(),
			worldMap: worldMap,
			currentRegionId: currentRegionId,
			actors: actors,
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

		

		StreamWriter writer = new StreamWriter(savePath, false);
		JsonTextWriter jwriter = new JsonTextWriter(writer);
		jwriter.Formatting = Formatting.Indented;

		JsonSerializer serializer = new JsonSerializer();
		serializer.TypeNameHandling = TypeNameHandling.Auto;
		serializer.Serialize(jwriter, save);

		writer.Close();
	}
}
