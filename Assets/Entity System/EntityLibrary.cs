using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EntityLibrary : MonoBehaviour
{
	static EntityLibraryObject libraryObject;
	static Dictionary<string, EntityData> library;
	const string EntityLibraryPath = "EntityLibrary";


	// Must be called to use this class
	public static void LoadLibrary () {
		// TODO directly reference the library asset instead of using Resources
		EntityLibraryObject loadedLibraryAsset = (EntityLibraryObject)(Resources.Load (EntityLibraryPath, typeof(ScriptableObject)));
		if (loadedLibraryAsset == null)
			Debug.LogError ("Entity library not found!");
		else if (loadedLibraryAsset.libraryIds == null || loadedLibraryAsset.libraryEntities == null)
			Debug.LogError ("Entity library doesn't appear to be built!");

		libraryObject = loadedLibraryAsset;;
		MakeDictionary ();
	}
		
	static void MakeDictionary () {
		library = new Dictionary<string, EntityData>();
		for (int i = 0; i < libraryObject.libraryIds.Count; i++) {
			library.Add (libraryObject.libraryIds [i], libraryObject.libraryEntities [i]);
		}
	}
	public static List<string> GetEntityIdList () {
		if (library == null) {
			LoadLibrary ();
		}
		List<string> keys = new List<string> ();
		foreach (string key in library.Keys)
			keys.Add (key);
		return keys;
	}
	public static EntityData GetEntityFromID (string id) {
		if (!library.ContainsKey(id)) {
			return null;
		}
		return library [id];
	}
}
