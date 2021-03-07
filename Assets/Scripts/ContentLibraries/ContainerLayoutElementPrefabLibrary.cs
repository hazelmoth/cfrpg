using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace ContentLibraries
{
    public class ContainerLayoutElementPrefabLibrary
    {
        private const string LIBRARY_ASSET_PATH = "ContainerLayoutElementPrefabLibrary";
        private Dictionary<string, GameObject> prefabs; 
    
        public void LoadLibrary ()
        {
            var loadedLibraryAsset = (ContainerLayoutElementPrefabLibraryAsset) (Resources.Load(LIBRARY_ASSET_PATH, typeof(ScriptableObject)));

            if (loadedLibraryAsset == null)
                throw new FileNotFoundException("Failed to load ContainerLayoutElementPrefabLibrary from resources");
        
            prefabs = loadedLibraryAsset.prefabs.ToDictionary(entry => entry.id, entry => entry.prefab);
        }
    
        public GameObject Get(string id)
        {
            return prefabs.ContainsKey(id) ? prefabs[id] : null;
        }
    }
}
