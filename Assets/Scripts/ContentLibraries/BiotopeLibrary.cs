using System.Collections.Generic;
using UnityEngine;

namespace ContentLibraries
{
	public class BiotopeLibrary
	{
		private const string LIBRARY_ASSET_PATH = "BiotopeLibrary"; 
		private BiotopeLibraryAsset loadedAsset;

		public List<Biotope> Biotopes { get; private set; }

		public void LoadLibrary()
		{
			loadedAsset = (BiotopeLibraryAsset)(Resources.Load(LIBRARY_ASSET_PATH, typeof(ScriptableObject)));

			if (loadedAsset == null)
			{
				Debug.LogError("Library asset not found!");
			}
			else if (loadedAsset.biotopes == null)
			{
				Debug.LogError("Library doesn't appear to be built!");
			}

			Biotopes = loadedAsset.biotopes;
		}

	
	}
}
