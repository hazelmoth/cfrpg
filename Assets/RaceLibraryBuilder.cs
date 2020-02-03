using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public static class RaceLibraryBuilder
{
	private const string RACES_FOLDER_PATH = "Content/Races";
	private const string RACE_LIBRARY_PATH = "Resources/RaceLibrary.asset";
	private const string DATA_OBJECT_NAME = "data.asset";

	[MenuItem("Assets/Build Race Library")]
	public static void BuildLibrary()
	{
		List<ActorRace> races = ReadActorRaces();
		List<string> libraryIds = new List<string>();

		foreach (ActorRace actorRace in races)
		{
			libraryIds.Add(actorRace.Id);
		}
		// Create a new library prefab
		RaceLibraryAsset libraryObject = ScriptableObject.CreateInstance<RaceLibraryAsset>();
		AssetDatabase.CreateAsset(libraryObject, "Assets/" + RACE_LIBRARY_PATH);

		// Relocate the created prefab in the assets folder
		RaceLibraryAsset loadedLibraryAsset = (RaceLibraryAsset)(AssetDatabase.LoadAssetAtPath("Assets/" + RACE_LIBRARY_PATH, typeof(ScriptableObject)));
		// Make some persistent changes
		Undo.RecordObject(loadedLibraryAsset, "Build race library prefab");
		loadedLibraryAsset.ids = libraryIds;
		loadedLibraryAsset.races = races;

		PrefabUtility.RecordPrefabInstancePropertyModifications(loadedLibraryAsset);
		EditorUtility.SetDirty(loadedLibraryAsset);

		// Double check that that worked
		if (loadedLibraryAsset == null || loadedLibraryAsset.ids == null)
		{
			Debug.LogError("Entity library build failed!");
		}
		else
		{
			Debug.Log("Race library built.");
		}
	}

	static List<ActorRace> ReadActorRaces()
	{
		List<ActorRace> races = new List<ActorRace>();

		// 1. go through each folder
		// 2. parse the data file for actorRace properties and make it into an actorRacedata
		// 3. add a reference to the actual prefab for the actorRace to the actorRacedata
		// 4. do this for all of the entities and make a list

		var racesFolder = new DirectoryInfo(Path.Combine(Application.dataPath, RACES_FOLDER_PATH));

		foreach (FileInfo file in racesFolder.GetFiles("*.asset"))
		{
			string dataObjectPath = "Assets/" + RACES_FOLDER_PATH + "/" + file.Name;

			ActorRace dataObject = (ActorRace)AssetDatabase.LoadMainAssetAtPath(dataObjectPath);
			if (dataObject != null)
			{
				races.Add(dataObject);
			}
			else
			{
				Debug.LogWarning("Failed to cast ActorRace object from \"" + file.Name + "\"!");
			}
		}
		return races;
	}
}
