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

        FileInfo[] saveFiles = saveFolder.GetFiles();
        foreach(FileInfo saveFile in saveFiles)
        {
	        StreamReader reader = new StreamReader(saveFile.OpenRead());
            string readJson = reader.ReadToEnd();
            reader.Close();
            WorldSave loadedSave = JsonUtility.FromJson<WorldSave>(readJson);
            loadedSave.saveFileId = Path.GetFileNameWithoutExtension(saveFile.FullName);
			retVal.Add(loadedSave);
        }

        return retVal;
    }

    // TODO update to use new format
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
