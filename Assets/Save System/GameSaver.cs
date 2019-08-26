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


        WriteSave(GenerateWorldSave(), GeneratePlayerSave(), saveId);
    }
    public static WorldSave GenerateWorldSave ()
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
	public static SavedPlayerChar GeneratePlayerSave ()
	{
		// Start with player data loaded at game start, then update it with current game data
		PlayerCharData data = GameDataMaster.PlayerToLoad.data;
		data.hairId = Player.instance.GetHair();
		data.inventory = new SerializableActorInv(Player.instance.Inventory.GetContents());
		data.ducatBalance = PlayerDucats.DucatBalance;


		Vector2 location = TilemapInterface.WorldPosToScenePos(Player.instance.transform.position, Player.instance.CurrentScene);
		Direction direction = Player.instance.Direction;
		string scene = Player.instance.CurrentScene;

		return new SavedPlayerChar(data, location, direction, scene);
	}

	static void WriteSave (WorldSave save, SavedPlayerChar player, string saveId)
	{
		List<SavedPlayerChar> list = new List<SavedPlayerChar> { player };
		WriteSave(save, list, saveId);
	}

	static void WriteSave (WorldSave save, List<SavedPlayerChar> players, string saveId)
	{

		string savePath = Application.persistentDataPath + "/saves/" + saveId + "/world.cfrpg";

		// Create the directory if nonexistent
		if (!Directory.Exists(Path.GetDirectoryName(savePath)))
		{
			Directory.CreateDirectory(Path.GetDirectoryName(savePath));
		}

		string json = JsonUtility.ToJson(save);
		StreamWriter writer = new StreamWriter(savePath, false);
		writer.WriteLine(json);
		writer.Close();

		foreach (SavedPlayerChar player in players)
		{
			if (player.data == null)
			{
				Debug.LogWarning("No pre-loaded data found for this player.");
				player.data = new PlayerCharData();
			}
			string playerSavePath = Application.persistentDataPath + "/saves/" + saveId + "/players/" + player.data.saveId + ".player";

			// Create the directory if nonexistent
			if (!Directory.Exists(Path.GetDirectoryName(playerSavePath)))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(playerSavePath));
			}

			string playerJson = JsonUtility.ToJson(player);
			writer = new StreamWriter(playerSavePath, false);
			writer.WriteLine(playerJson);
			writer.Close();
		}
	}
}
