using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace ContentLibraries
{
    public class GenericContentLibrary<T> where T : IContentItem
    {
        private readonly string assetPath;
        private Dictionary<string, T> content;
        private LibraryAsset<T> loadedAsset;

        public GenericContentLibrary(string assetPath)
        {
            this.assetPath = assetPath;
        }

        public void LoadLibrary()
        {
            loadedAsset = (LibraryAsset<T>) Resources.Load(assetPath, typeof(ScriptableObject));

            if (loadedAsset == null)
                throw new FileNotFoundException($"{typeof(T).Name} library asset not found!");
            if (loadedAsset.content == null) 
                throw new NullReferenceException("Library doesn't appear to be built!");

            content = loadedAsset.content.ToDictionary(entry =>
            {
                if (entry != null) return entry.Id;

                Debug.LogError($"Content library of type {typeof(T).FullName} has a null entry!");
                return "";
            });
        }

        public bool Contains(string id)
        {
            return id != null && content.ContainsKey(id);
        }

        /// Returns the content item with the given ID. Assumes that such an item exists.
        public T Get(string id)
        {
            if (id != null && content.ContainsKey(id)) return content[id];
            Debug.LogError($"ID \"{id}\" not found in {typeof(T).Name} content library.");
            return default;
        }

        /// Attempts to get the content item with the given ID.
        public bool TryGet(string id, out T item)
        {
            if (id != null && content.ContainsKey(id))
            {
                item = content[id];
                return true;
            }

            item = default;
            return false;
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
