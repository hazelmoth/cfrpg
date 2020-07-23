using System.Collections.Generic;
using UnityEngine;

public class CharacterGenTemplateLibrary
{
	private const string LIBRARY_ASSET_PATH = "CharacterGenTemplateLibrary";

	private CharacterGenTemplateLibraryAsset loadedLibraryAsset;
	private IDictionary<string, CharacterGenTemplate> library;

	public void LoadLibrary()
	{
		loadedLibraryAsset = (CharacterGenTemplateLibraryAsset)(Resources.Load(LIBRARY_ASSET_PATH, typeof(ScriptableObject)));

		if (loadedLibraryAsset == null)
		{
			Debug.LogError("Library asset not found!");
		}
		else if (loadedLibraryAsset.content == null)
		{
			Debug.LogError("Library doesn't appear to be built!");
		}

		MakeDictionary();
	}

	private void MakeDictionary()
	{
		library = new Dictionary<string, CharacterGenTemplate>();
		for (int i = 0; i < loadedLibraryAsset.content.Count; i++)
		{
			library.Add(((CharacterGenTemplate)loadedLibraryAsset.content[i]).templateId, (CharacterGenTemplate)loadedLibraryAsset.content[i]);
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
	public CharacterGenTemplate Get(string id)
	{
		if (!library.ContainsKey(id))
		{
			return null;
		}
		return library[id];
	}
}

