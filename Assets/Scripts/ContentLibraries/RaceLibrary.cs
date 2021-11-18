using System.Collections.Generic;
using UnityEngine;

namespace ContentLibraries
{
	public class RaceLibrary
	{
		private const string LIBRARY_ASSET_PATH = "RaceLibrary";

		private RaceLibraryAsset loadedLibraryAsset;
		private IDictionary<string, ActorRace> library;

		public void LoadLibrary()
		{
			loadedLibraryAsset = (RaceLibraryAsset)(Resources.Load(LIBRARY_ASSET_PATH, typeof(ScriptableObject)));

			if (loadedLibraryAsset == null)
			{
				Debug.LogError("Library asset not found!");
			}
			else if (loadedLibraryAsset.races == null)
			{
				Debug.LogError("Library doesn't appear to be built!");
			}

			MakeDictionary();
		}

		private void MakeDictionary()
		{
			library = new Dictionary<string, ActorRace>();
			for (int i = 0; i < loadedLibraryAsset.races.Count; i++)
			{
				library.Add(loadedLibraryAsset.races[i].Id, loadedLibraryAsset.races[i]);
			}
		}
		
		public List<string> GetIdList()
		{
			if (library == null)
			{
				LoadLibrary();
			}
			List<string> keys = new List<string>();
			foreach (string key in library.Keys)
			{
				keys.Add(key);
			}
			return keys;
		}

		public ActorRace Get(string id)
		{
			if (!library.ContainsKey(id))
			{
				return null;
			}
			return library[id];
		}

		public bool Contains(string id)
        {
            return library.ContainsKey(id);
        }
	}
}
