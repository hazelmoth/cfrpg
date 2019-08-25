using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// reads save files into WorldSave objects
public static class SaveReader
{
    public static List<WorldSave> GetAllSaves ()
    {
        List<WorldSave> retVal = new List<WorldSave>();

        string savePath = Application.persistentDataPath + "/saves";
        if (!Directory.Exists(savePath))
        {
            Debug.Log("Save folder not found. Creating at \"" + savePath + "\".");
            Directory.CreateDirectory(savePath);
        }
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
    public static WorldSave GetSave (string fileId)
    {
        string savePath = Application.persistentDataPath + "/saves";
        if (!Directory.Exists(savePath))
        {
            Debug.Log("Save folder not found. Creating at \"" + savePath + "\".");
            Directory.CreateDirectory(savePath);
        }
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
