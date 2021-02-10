using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using ContinentMaps;
using UnityEngine;

public static class GameSaver
{
    public static void SaveGame (string saveId)
    {
        if (saveId == null)
        {
            Debug.Log("No name for this save found. Looking for one from a recently created world.");
            saveId = GeneratedWorldSettings.worldName;
            if (saveId == null)
            {
                Debug.LogError("No current save name found!");
                saveId = "MissingName";
            }
        }
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

        List<SavedEntity> entities = new List<SavedEntity>();
		foreach (string scene in RegionMapManager.GetObjectMaps().Keys)
		{
			foreach (Vector2 location in RegionMapManager.GetObjectMaps()[scene].Keys)
			{
				if (RegionMapManager.GetObjectMaps()[scene][location.ToVector2Int()] != null)
				{
					EntityObject entity = RegionMapManager.GetEntityObjectAtPoint(location.ToVector2Int(), scene).GetComponent<EntityObject>();
					// Only add this entity to the save if the location we're checking is the root location of the entity--
					// so we're not adding the same entity to the save an additional time for each tile it covers
					if (TilemapInterface.WorldPosToScenePos(entity.transform.position, entity.Scene).ToVector2Int() == location.ToVector2Int())
					{
						SavedEntity entitySave = entity.GetSaveData();
						entities.Add(entitySave);
					}
				}
			}
		}

		List<SavedActor> Actors = new List<SavedActor> ();
		foreach (string actorId in ActorRegistry.GetAllIds())
		{
			ActorData actor = ActorRegistry.Get(actorId).data;

			SavedActor actorSave = new SavedActor(actor);
			Actors.Add(actorSave);
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

		List<SerializableScenePortal> scenePortals = new List<SerializableScenePortal>();
		scenePortals = ScenePortalLibrary.GetAllPortalDatas();
		
		SerializableContinentMap worldMap = ContinentManager.GetSaveData();
		
		Vector2IntSerializable currentRegionCoords = RegionMapManager.CurrentRegionCoords.ToSerializable();

		ulong time = TimeKeeper.CurrentTick;

		History.EventLog eventLog = GameObject.FindObjectOfType<History>().GetEventLog();
		

		WorldSave save = new WorldSave(
			worldName,
			time,
			PlayerController.PlayerActorId,
			eventLog,
			SaveInfo.RegionSize.ToSerializable(),
			worldMap,
			currentRegionCoords,
			entities,
			Actors,
			items,
			scenePortals,
			false);
		
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
