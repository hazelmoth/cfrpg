using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

// Builds a ScriptableObject in the editor to store entity data
namespace ContentLibraries
{
	public static class EntityLibraryBuilder
	{
		private const string EntitiesFolderPath = "Content/Entities";
		private const string EntityLibraryPath = "Resources/EntityLibrary.asset";
		private const string DataObjectName = "data.asset";

		[MenuItem("Assets/Build Entity Library")]
		public static void BuildEntityLibrary () {
			List<EntityData> entities = ReadEntities ();
			List<EntityData> libraryEntities = new List<EntityData> ();

			foreach (EntityData entity in entities) {
				libraryEntities.Add (entity);
			}
			
			// Create a new library prefab
			EntityLibraryObject libraryObject = ScriptableObject.CreateInstance<EntityLibraryObject> (); 
			AssetDatabase.CreateAsset(libraryObject, "Assets/" + EntityLibraryPath);

			// Relocate the created prefab in the assets folder
			EntityLibraryObject loadedLibraryAsset =
				(EntityLibraryObject)(AssetDatabase.LoadAssetAtPath (
					"Assets/" + EntityLibraryPath, typeof(ScriptableObject)));

			// Make some persistent changes
			Undo.RecordObject (loadedLibraryAsset, "Build entity library prefab");
			loadedLibraryAsset.content = libraryEntities;
			PrefabUtility.RecordPrefabInstancePropertyModifications (loadedLibraryAsset);
			EditorUtility.SetDirty (loadedLibraryAsset);


			// Double check that that worked
			if (loadedLibraryAsset == null || loadedLibraryAsset.content == null) {
				Debug.LogWarning ("Entity library build failed!");
			} else {
				Debug.Log ("Entity library built.");
			}
		}

		private static List<EntityData> ReadEntities () {
			List<EntityData> entities = new List<EntityData> ();
			DirectoryInfo entitiesFolder = new DirectoryInfo(Path.Combine(Application.dataPath, EntitiesFolderPath));
			DirectoryInfo[] entityDirs = entitiesFolder.GetDirectories ();

			foreach (DirectoryInfo dir in entityDirs) {
				string entName = dir.Name;
				string dataObjectPath = "Assets/" + EntitiesFolderPath + "/" + dir.Name + "/" + DataObjectName;
				EntityDataAsset dataObject = (EntityDataAsset)AssetDatabase.LoadMainAssetAtPath(dataObjectPath);

				if (dataObject == null)
				{
					Debug.LogWarning("Data object not found for entity \"" + entName + "\"!");
					continue;
				}

				// Auto-add EntityObject components.
				GameObject prefab = dataObject.data.EntityPrefab;
				EntityObject prefabEntityTag = prefab.GetComponent<EntityObject>();
				if (prefabEntityTag == null) prefabEntityTag = prefab.AddComponent<EntityObject>();
				prefabEntityTag.EntityId = dataObject.data.Id;
				EditorUtility.SetDirty(prefab);

				entities.Add(dataObject.data);
			}
			return entities;
		}
	}
}
