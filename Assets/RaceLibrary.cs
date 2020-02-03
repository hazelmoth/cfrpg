using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceLibrary
{
	const string LIBRARY_ASSET_PATH = "RaceLibrary";

	private RaceLibraryAsset loadedLibraryAsset;
	private IDictionary<string, ActorRace> library;

	public void LoadLibrary()
	{
		loadedLibraryAsset = (RaceLibraryAsset)(Resources.Load(LIBRARY_ASSET_PATH, typeof(ScriptableObject)));

		if (loadedLibraryAsset == null)
		{
			Debug.LogError("Library asset not found!");
		}
		else if (loadedLibraryAsset.ids == null || loadedLibraryAsset.races == null)
		{
			Debug.LogError("Library doesn't appear to be built!");
		}

		MakeDictionary();
	}

	void MakeDictionary()
	{
		library = new Dictionary<string, ActorRace>();
		for (int i = 0; i < loadedLibraryAsset.ids.Count; i++)
		{
			library.Add(loadedLibraryAsset.ids[i], loadedLibraryAsset.races[i]);
		}
	}
	public List<string> GetEntityIdList()
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
	public ActorRace GetEntityFromID(string id)
	{
		if (!library.ContainsKey(id))
		{
			return null;
		}
		return library[id];
	}
}

