using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// reads save files into WorldSave objects
public static class SaveReader
{
	private static string GetSaveFolderPath()
	{
		return Application.persistentDataPath + "/saves";
	}

	private static void CreateSavesFolderIfAbsent()
	{
		string savePath = GetSaveFolderPath();
		if (!Directory.Exists(savePath))
		{
			Debug.Log("Save folder not found. Creating at \"" + savePath + "\".");
			Directory.CreateDirectory(savePath);
		}
	}
    public static List<WorldSave> GetAllSaves ()
    {
        List<WorldSave> retVal = new List<WorldSave>();

		string savePath = GetSaveFolderPath();
		CreateSavesFolderIfAbsent();

        var saveFolder = new DirectoryInfo(savePath);

        DirectoryInfo[] saveFolders = saveFolder.GetDirectories();
        foreach(DirectoryInfo directory in saveFolders)
        {
            string saveFilePath = directory.FullName + "/world.cfrpg";

            StreamReader reader = new StreamReader(saveFilePath);
            string readJson = reader.ReadToEnd();
            reader.Close();
            WorldSave loadedSave = JsonUtility.FromJson<WorldSave>(readJson);
			loadedSave.saveFileId = directory.Name;
            retVal.Add(loadedSave);
        }

        return retVal;
    }
	public static List<SavedPlayerChar> GetPlayerChars(string worldSaveId)
	{
		string savesPath = GetSaveFolderPath();
		CreateSavesFolderIfAbsent();

		List<SavedPlayerChar> retVal = new List<SavedPlayerChar>();

		string savePath = savesPath + "/" + worldSaveId;
		if (!Directory.Exists(savePath))
		{
			Debug.LogError("Save folder not found at " + savePath);
			return retVal;
		}
		string playerSavesPath = savePath + "/players";
		if (!Directory.Exists(playerSavesPath))
		{
			Directory.CreateDirectory(playerSavesPath);
			return retVal;
		}

		var playerSavesDirectory = new DirectoryInfo(playerSavesPath);
		foreach(FileInfo playerSaveFile in playerSavesDirectory.GetFiles())
		{
			string saveFilePath = playerSaveFile.FullName;

			StreamReader reader = new StreamReader(saveFilePath);
			string readJson = reader.ReadToEnd();
			reader.Close();
			SavedPlayerChar loadedSave = JsonUtility.FromJson<SavedPlayerChar>(readJson);
			retVal.Add(loadedSave);
		}
		return retVal;
	}
	public static SavedPlayerChar GetPlayerChar(string worldSaveId, string playerSaveId)
	{
		CreateSavesFolderIfAbsent();

		string savesPath = GetSaveFolderPath();
		string playerSavePath = savesPath + "/" + worldSaveId + "/players/" + playerSaveId + ".player";
		if (!File.Exists(playerSavePath))
		{
			Debug.LogError("Player save \"" + playerSaveId + "\" not found.");
			return null;
		}
		StreamReader reader = new StreamReader(playerSavePath);
		string readJson = reader.ReadToEnd();
		reader.Close();
		SavedPlayerChar loadedSave = JsonUtility.FromJson<SavedPlayerChar>(readJson);
		return loadedSave;
	}
    public static WorldSave GetSave (string fileId)
    {
        string savePath = Application.persistentDataPath + "/saves";
		CreateSavesFolderIfAbsent();

        string folderPath = savePath + "/" + fileId;
        if (!Directory.Exists(folderPath))
        {
            Debug.LogError("Save folder not found at " + folderPath);
            return null;
        }
        string saveFilePath = folderPath + "/world.cfrpg";

        StreamReader reader = new StreamReader(saveFilePath);
        string jsonText = reader.ReadToEnd();
        reader.Close();
        WorldSave loadedSave = JsonUtility.FromJson<WorldSave>(jsonText);
        return loadedSave;
    }
}
