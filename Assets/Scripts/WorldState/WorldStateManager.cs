using System;
using Newtonsoft.Json;
using UnityEngine;

namespace WorldState
{
    /// A MonoBehaviour that provides access to the current world state dictionary.
    /// Returns default values for unset keys.
    public class WorldStateManager : MonoBehaviour
    {
        private MultiStringDict dictionary;
        private bool initialized = false;

        public void Init(MultiStringDict dictionary)
        {
            dictionary ??= new MultiStringDict();
            this.dictionary = dictionary;
            initialized = true;
        }

        public MultiStringDict GetDictionary()
        {
            return dictionary;
        }

        public void SetString(string key, string value)
        {
            if (!initialized) Debug.LogError("WorldState not initialized");
            dictionary.SetString(key, value);
        }

        public string GetString(string key)
        {
            if (!initialized) Debug.LogError("WorldState not initialized");
            return dictionary.ContainsString(key) ? dictionary.GetString(key) : null;
        }

        public void SetInt(string key, int value)
        {
            if (!initialized) Debug.LogError("WorldState not initialized");
            dictionary.SetInt(key, value);
        }

        public int GetInt(string key)
        {
            if (!initialized) Debug.LogError("WorldState not initialized");
            return dictionary.ContainsInt(key) ? dictionary.GetInt(key) : 0;
        }

        public void SetFloat(string key, float value)
        {
            if (!initialized) Debug.LogError("WorldState not initialized");
            dictionary.SetFloat(key, value);
        }

        public float GetFloat(string key)
        {
            if (!initialized) Debug.LogError("WorldState not initialized");
            return dictionary.ContainsFloat(key) ? dictionary.GetFloat(key) : 0;
        }

        public void SetBool(string key, bool value)
        {
            if (!initialized) Debug.LogError("WorldState not initialized");
            dictionary.SetBool(key, value);
        }

        public bool GetBool(string key)
        {
            if (!initialized) Debug.LogError("WorldState not initialized");
            return dictionary.ContainsBool(key) && dictionary.GetBool(key);
        }

        /// Use JSON.NET to serialize the world state to a string.
        public string GetJson()
        {
            if (!initialized) Debug.LogError("WorldState not initialized");

            string json = JsonConvert.SerializeObject(dictionary);
            return json;
        }
    }
}
