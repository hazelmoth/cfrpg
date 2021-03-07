using System.Collections.Generic;
using UnityEngine;

namespace ContentLibraries
{
	public class EntityLibrary
	{
		private EntityLibraryObject libraryObject;
		private Dictionary<string, EntityData> library;
		private const string ENTITY_LIBRARY_PATH = "EntityLibrary";


		// Must be called to use this class
		public void LoadLibrary () {
			EntityLibraryObject loadedLibraryAsset = (EntityLibraryObject)(Resources.Load (ENTITY_LIBRARY_PATH, typeof(ScriptableObject)));

			if (loadedLibraryAsset == null)
			{
				Debug.LogError("Entity library not found!");
			}
			else if (loadedLibraryAsset.libraryIds == null || loadedLibraryAsset.libraryEntities == null)
			{
				Debug.LogError("Entity library doesn't appear to be built!");
			}

			libraryObject = loadedLibraryAsset;;
			MakeDictionary ();
		}

		private void MakeDictionary () {
			library = new Dictionary<string, EntityData>();
			for (int i = 0; i < libraryObject.libraryIds.Count; i++) {
				library.Add (libraryObject.libraryIds [i], libraryObject.libraryEntities [i]);
			}
		}
		public List<string> GetEntityIdList () {
			if (library == null) {
				LoadLibrary ();
			}
			List<string> keys = new List<string> ();
			foreach (string key in library.Keys)
				keys.Add (key);
			return keys;
		}
		public EntityData Get (string id) {
			if (id == null || !library.ContainsKey(id)) {
				Debug.LogWarning("Entity ID \"" + id + "\" not found!");
				return null;
			}
			return library [id];
		}
	}
}
