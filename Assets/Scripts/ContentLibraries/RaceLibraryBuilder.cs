using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ContentLibraries
{
	public static class RaceLibraryBuilder
	{
		private const string RACES_FOLDER_PATH = "Content/Races";
		private const string RACE_LIBRARY_PATH = "Resources/RaceLibrary.asset";

		[MenuItem("Assets/Build Race Library")]
		public static void BuildLibrary()
		{
			List<ActorRace> races = ReadActorRaces();

			// Create a new library prefab
			RaceLibraryAsset libraryObject = ScriptableObject.CreateInstance<RaceLibraryAsset>();
			AssetDatabase.CreateAsset(libraryObject, "Assets/" + RACE_LIBRARY_PATH);

			// Relocate the created prefab in the assets folder
			RaceLibraryAsset loadedLibraryAsset = (RaceLibraryAsset)(AssetDatabase.LoadAssetAtPath("Assets/" + RACE_LIBRARY_PATH, typeof(ScriptableObject)));
			// Make some persistent changes
			Undo.RecordObject(loadedLibraryAsset, "Build race library prefab");
			loadedLibraryAsset.races = races;

			PrefabUtility.RecordPrefabInstancePropertyModifications(loadedLibraryAsset);
			EditorUtility.SetDirty(loadedLibraryAsset);

			// Double check that that worked
			if (loadedLibraryAsset == null || loadedLibraryAsset.races == null)
			{
				Debug.LogError("Entity library build failed!");
			}
			else
			{
				Debug.Log("Race library built.");
			}
		}

		private static List<ActorRace> ReadActorRaces()
		{
			List<ActorRace> races = new List<ActorRace>();

			// 1. go through each folder
			// 2. parse the data file for actorRace properties and make it into an actorRacedata
			// 3. add a reference to the actual prefab for the actorRace to the actorRacedata
			// 4. do this for all of the entities and make a list

			var racesFolder = new DirectoryInfo(Path.Combine(Application.dataPath, RACES_FOLDER_PATH));

			foreach (DirectoryInfo folder in racesFolder.GetDirectories())
			{
				FileInfo locatedAsset = null;
				foreach (FileInfo asset in folder.GetFiles("*.asset"))
				{
					locatedAsset = asset;
					break;
				}
				if (locatedAsset == null)
				{
					Debug.LogWarning("Found a folder \"" + folder.Name + "\" without any asset file in race content directory.");
					continue;
				}

				string dataObjectPath = "Assets/" + RACES_FOLDER_PATH + "/" + folder.Name + "/" + locatedAsset.Name;

				ActorRace dataObject = (ActorRace)AssetDatabase.LoadMainAssetAtPath(dataObjectPath);
				if (dataObject != null)
				{
					races.Add(dataObject);
				}
				else
				{
					Debug.LogWarning("Failed to cast ActorRace object from \"" + locatedAsset.Name + "\"!");
				}
			}
			return races;
		}
	}
}
