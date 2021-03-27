using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ContentLibraries
{
    public class GenericContentLibrary<T> where T : IContentItem
    {
        private string assetPath;
        private LibraryAsset<T> loadedAsset;
        private Dictionary<string, T> content;

        public GenericContentLibrary(string assetPath)
        {
            this.assetPath = assetPath;
        }

        public void LoadLibrary()
        {
            loadedAsset = (LibraryAsset<T>) (Resources.Load(assetPath, typeof(ScriptableObject)));

            if (loadedAsset == null)
            {
                Debug.LogError("Library asset not found!");
                return;
            }
            else if (loadedAsset.content == null)
            {
                Debug.LogError("Library doesn't appear to be built!");
                return;
            }

            content = loadedAsset.content.ToDictionary(entry => entry.Id);
        }
        
        public T Get(string id)
        {
            if (id != null && content.ContainsKey(id)) return content[id];
            Debug.LogError("Couldn't find content of type " + typeof(T).FullName + " for given ID: " + id);
            return default;
        }
        
        public ICollection<string> GetAllIds()
        {
            return content.Keys;
        }

        public ICollection<T> GetAll()
        {
            return content.Values;
        }
    }

    public interface IContentItem
    {
        public string Id { get; }
    }
}