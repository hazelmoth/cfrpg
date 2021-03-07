using System.Collections.Generic;
using UnityEngine;

namespace ContentLibraries
{
	public class RegionFeatureLibrary
	{
		private const string LIBRARY_ASSET_PATH = "RegionFeatureLibrary";

		private RegionFeatureLibraryAsset loadedLibraryAsset;
		private IDictionary<string, RegionFeature> library;
    
		public void LoadLibrary()
		{
			loadedLibraryAsset = (RegionFeatureLibraryAsset)(Resources.Load(LIBRARY_ASSET_PATH, typeof(ScriptableObject)));
    
			if (loadedLibraryAsset == null)
			{
				Debug.LogError("Library asset not found!");
			}
			else if (loadedLibraryAsset.features == null)
			{
				Debug.LogError("Library doesn't appear to be built!");
			}
    
			MakeDictionary();
		}
    
		private void MakeDictionary()
		{
			library = new Dictionary<string, RegionFeature>();
			for (int i = 0; i < loadedLibraryAsset.features.Count; i++)
			{
				library.Add(loadedLibraryAsset.features[i].Id, loadedLibraryAsset.features[i]);
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
		public RegionFeature Get(string id)
		{
			if (!library.ContainsKey(id))
			{
				return null;
			}
			return library[id];
		}
	}
}
