using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using UnityEngine;

public static class GameSaver
{
    public static WorldSave GenerateSave ()
	{
		List<SavedEntity> entities = new List<SavedEntity>();
		foreach (string scene in WorldMapManager.GetObjectMaps().Keys)
		{
			foreach (Vector2 location in WorldMapManager.GetObjectMaps()[scene].Keys)
			{
				EntityObject entity = WorldMapManager.GetEntityObjectAtPoint(location.ToVector2Int(), scene).GetComponent<EntityObject>();
				// Only add this entity to the save if the location we're checking is the root location of the entity--
				// so we're not adding the same entity to the save multiple times for each tile it covers
				if (TilemapInterface.WorldPosToScenePos(entity.transform.position, entity.Scene) == location)
				{
					SavedEntity entitySave = entity.GetStateData();
					entities.Add(entitySave);
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

		WorldSave save = new WorldSave(worldMap, entities, npcs);
		return save;
	}
	public static void WriteSave (WorldSave save, string worldName)
	{
		const bool useJson = true;

		string savePath = Application.persistentDataPath + "/" + worldName + "/world.cfrpg";

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

			//TEST
			StreamReader reader = new StreamReader(savePath);
			string readJson = reader.ReadToEnd();
			reader.Close();
			WorldSave pheonix = JsonUtility.FromJson<WorldSave>(readJson);
			Debug.Log(pheonix.worldMap.scenes[0]);
			Debug.Log(pheonix.entities.Count);
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
