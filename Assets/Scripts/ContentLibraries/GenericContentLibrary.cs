using System;
using System.Collections.Generic;
using System.IO;
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
                throw new FileNotFoundException("Library asset not found!");
            }
            else if (loadedAsset.content == null)
            {
                throw new NullReferenceException("Library doesn't appear to be built!");
            }

            content = loadedAsset.content.ToDictionary(entry =>
            {
                if (entry != null) return entry.Id;
                
                Debug.LogError($"Content library of type {typeof(T).FullName} has a null entry!");
                return "";
            });
        }
        
        public T Get(string id)
        {
            if (id != null && content.ContainsKey(id)) return content[id];
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