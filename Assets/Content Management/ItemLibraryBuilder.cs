using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public static class ItemLibraryBuilder
{
	private const string ITEMS_FOLDER_PATH = "Content/Items";
	private const string ITEM_LIBRARY_PATH = "Resources/ItemLibrary.asset";

	[MenuItem("Assets/Build Item Library")]
	public static void BuildLibrary()
	{
		List<Item> items = ReadItems();

		// Create a new library prefab
		ItemLibraryAsset libraryObject = ScriptableObject.CreateInstance<ItemLibraryAsset>();
		AssetDatabase.CreateAsset(libraryObject, "Assets/" + ITEM_LIBRARY_PATH);

		// Relocate the created prefab in the assets folder
		ItemLibraryAsset loadedLibraryAsset = (ItemLibraryAsset)(AssetDatabase.LoadAssetAtPath("Assets/" + ITEM_LIBRARY_PATH, typeof(ScriptableObject)));
		// Make some persistent changes
		Undo.RecordObject(loadedLibraryAsset, "Build race library prefab");
		loadedLibraryAsset.items = items;

		PrefabUtility.RecordPrefabInstancePropertyModifications(loadedLibraryAsset);
		EditorUtility.SetDirty(loadedLibraryAsset);

		// Double check that that worked
		if (loadedLibraryAsset == null || loadedLibraryAsset.items == null)
		{
			Debug.LogError("Entity library build failed!");
		}
		else
		{
			Debug.Log("Item library built.");
		}
	}

	static List<Item> ReadItems()
	{
		List<Item> items = new List<Item>();

		// 1. go through each folder
		// 2. parse the data file for Item properties and make it into an Itemdata
		// 3. add a reference to the actual prefab for the Item to the Itemdata
		// 4. do this for all of the entities and make a list

		var itemsFolder = new DirectoryInfo(Path.Combine(Application.dataPath, ITEMS_FOLDER_PATH));

		foreach (DirectoryInfo folder in itemsFolder.GetDirectories())
		{
			FileInfo locatedAsset = null;
			foreach (FileInfo asset in folder.GetFiles("*.asset"))
			{
				locatedAsset = asset;
				break;
			}
			if (locatedAsset == null)
			{
				Debug.LogWarning("Found a folder \"" + folder.Name + "\" without any asset file in item content directory.");
				continue;
			}

			string dataObjectPath = "Assets/" + ITEMS_FOLDER_PATH + "/" + folder.Name + "/" + locatedAsset.Name;

			Item dataObject = (Item)AssetDatabase.LoadMainAssetAtPath(dataObjectPath);
			if (dataObject != null)
			{
				items.Add(dataObject);
			}
			else
			{
				Debug.LogWarning("Failed to cast Item object from \"" + locatedAsset.Name + "\"!");
			}
		}
		return items;
	}
}
