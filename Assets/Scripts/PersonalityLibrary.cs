using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PersonalityLibrary
{
	private const string LIBRARY_ASSET_PATH = "PersonalityLibrary"; 
	private PersonalityLibraryAsset loadedAsset;

	private IDictionary<string, PersonalityData> contentDict;

	public void LoadLibrary()
	{
		loadedAsset = (PersonalityLibraryAsset)(Resources.Load(LIBRARY_ASSET_PATH, typeof(ScriptableObject)));
		contentDict = new Dictionary<string, PersonalityData>();

		if (loadedAsset == null)
		{
			Debug.LogError("Library asset not found!");
		}
		else if (loadedAsset.contents == null)
		{
			Debug.LogError("Library doesn't appear to be built!");
		}

		foreach (PersonalityData data in loadedAsset.contents)
		{
			contentDict.Add(data.PersonalityId, data);
		}
	}

	public List<string> GetAll()
	{
		return contentDict.Keys.ToList();
	}
	public PersonalityData GetById(string id)
	{
		if (!contentDict.TryGetValue(id, out PersonalityData result))
		{
			Debug.LogError("Personality ID \"" + id + "\" not found in personality library");
		}

		return result;
	}
}
