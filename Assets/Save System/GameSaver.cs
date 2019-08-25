using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using UnityEngine;

public static class GameSaver
{
    public static void SaveGame ()
    {
        string saveId = GameDataMaster.SaveFileId;
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


        WriteSave(GenerateSave(), saveId);
    }
    public static WorldSave GenerateSave ()
	{
        string worldName = GameDataMaster.WorldName;
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
		foreach (string scene in WorldMapManager.GetObjectMaps().Keys)
		{
			foreach (Vector2 location in WorldMapManager.GetObjectMaps()[scene].Keys)
			{
				if (WorldMapManager.GetObjectMaps()[scene][location.ToVector2Int()] != null)
				{
					EntityObject entity = WorldMapManager.GetEntityObjectAtPoint(location.ToVector2Int(), scene).GetComponent<EntityObject>();
					// Only add this entity to the save if the location we're checking is the root location of the entity--
					// so we're not adding the same entity to the save an additional time for each tile it covers
					if (TilemapInterface.WorldPosToScenePos(entity.transform.position, entity.Scene) == location)
					{
						SavedEntity entitySave = entity.GetStateData();
						entities.Add(entitySave);
					}
				}
			}
		}
		List<SavedNpc> npcs = new List<SavedNpc> ();
		foreach (NPC npc in NPCObjectRegistry.GetAllNpcs())
		{
			SavedNpc npcSave = new SavedNpc(npc);
			npcs.Add(npcSave);
		}

		SerializableWorldMap worldMap = new SerializableWorldMap(WorldMapManager.GetWorldMap());

		WorldSave save = new WorldSave(worldName, worldMap, entities, npcs);
		return save;
	}

	static void WriteSave (WorldSave save, string saveId)
	{
		const bool useJson = true;

		string savePath = Application.persistentDataPath + "/saves/" + saveId + "/world.cfrpg";

		// Create the directory if nonexistent
		if (!Directory.Exists(Path.GetDirectoryName(savePath)))
		{
			Directory.CreateDirectory(Path.GetDirectoryName(savePath));
		}

		if (useJson)
		{
			string json = JsonUtility.ToJson(save);
			StreamWriter writer = new StreamWriter(savePath, false);
			writer.WriteLine(json);
			writer.Close();
		}
		else
		{
			FileStream file = File.Create(savePath);
			BinaryFormatter formatter = new BinaryFormatter();
			try
			{
				formatter.Serialize(file, save);

			}
			catch (SerializationException e)
			{
				Debug.LogWarning("Failed to serialize. Reason: " + e.Message);
				throw;
			}
			finally
			{
				file.Close();
			}
		}
	}
}
