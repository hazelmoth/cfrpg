using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using SimpleJSON;

// Builds a ScriptableObject in the editor to store entity data
public class EntityLibraryBuilder : MonoBehaviour
{
	const string EntitiesFolderPath = "Content/Entities";
	const string EntityLibraryPath = "Resources/EntityLibrary.asset";
	const string DataObjectName = "data.asset";

	[MenuItem("Assets/Build Entity Library")]
	public static void BuildEntityLibrary () {
		List<EntityData> entities = ReadEntities ();
		List<string> libraryIds = new List<string> ();
		List<EntityData> libraryEntities = new List<EntityData> ();

		foreach (EntityData entity in entities) {
			libraryIds.Add (entity.entityId);
			libraryEntities.Add (entity);
			Debug.Log (entity.entityId);
			Debug.Log (entity.entityPrefab.name);
		}
		// Create a new library prefab
		EntityLibraryObject libraryObject = ScriptableObject.CreateInstance<EntityLibraryObject> (); 
		AssetDatabase.CreateAsset(libraryObject, "Assets/" + EntityLibraryPath);

		// Relocate the created prefab in the assets folder
		EntityLibraryObject loadedLibraryAsset = (EntityLibraryObject)(AssetDatabase.LoadAssetAtPath ("Assets/" + EntityLibraryPath, typeof(ScriptableObject)));
		// Make some persistent changes
		Undo.RecordObject (loadedLibraryAsset, "Build entity library prefab");
		loadedLibraryAsset.libraryIds = libraryIds;
		loadedLibraryAsset.libraryEntities = libraryEntities;
		PrefabUtility.RecordPrefabInstancePropertyModifications (loadedLibraryAsset);
		EditorUtility.SetDirty (loadedLibraryAsset);


		// Double check that that worked
		if (loadedLibraryAsset == null || loadedLibraryAsset.libraryIds == null) {
			Debug.LogWarning ("Entity library build failed!");
		} else {
			Debug.Log ("Entity library built.");
		}
	}

	static List<EntityData> ReadEntities () {
		List<EntityData> entities = new List<EntityData> ();

		// 1. go through each folder in entities
		// 2. parse the data file for entity properties and make it into an entitydata
		// 3. add a reference to the actual prefab for the entity to the entitydata
		// 4. do this for all of the entities and make a list

		var entitiesFolder = new DirectoryInfo(Path.Combine(Application.dataPath, EntitiesFolderPath));

		DirectoryInfo[] entityDirs = entitiesFolder.GetDirectories ();
		foreach (DirectoryInfo dir in entityDirs) {
			string entName = dir.Name;
			string dataObjectPath = "Assets/" + EntitiesFolderPath + "/" + dir.Name + "/" + DataObjectName;
			EntityDataAsset dataObject = (EntityDataAsset)AssetDatabase.LoadMainAssetAtPath(dataObjectPath);
			Debug.Log (entName);
			Debug.Log (dataObjectPath);
			Debug.Log (dataObject);
			entities.Add (dataObject.data);
		}
		return entities;
	}


	// No longer used because entity data is stored in scriptableobjects now; maybe useful for mods
	static EntityData ParseEntityJSON (string jsonString) {
		EntityData entity = new EntityData();
		JSONNode json = JSON.Parse(jsonString);
		entity.entityId = json ["id"];
		entity.baseShape = new List<Vector2Int> ();
		for (int i = 0; i < JSONHelper.GetElementCount(json["baseShape"]); i++) {
			Vector2Int point = new Vector2Int ();
			point.x = json ["baseShape"] [i] ["x"];
			point.y = json ["baseShape"] [i] ["y"];
			entity.baseShape.Add (point);
		}
		return entity;
	}
}
