using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace WorldState
{
    /// Dictionaries of strings to ints, floats, bools, and strings compounded together.
    [Serializable]
    [JsonObject(MemberSerialization.Fields)]
    public class MultiStringDict
    {
        private Dictionary<string, int> intDict;
        private Dictionary<string, float> floatDict;
        private Dictionary<string, bool> boolDict;
        private Dictionary<string, string> stringDict;

        public MultiStringDict()
        {
            intDict = new Dictionary<string, int>();
            floatDict = new Dictionary<string, float>();
            boolDict = new Dictionary<string, bool>();
            stringDict = new Dictionary<string, string>();
        }

        public void SetInt(string key, int value)
        {
            intDict[key] = value;
        }

        public void SetFloat(string key, float value)
        {
            floatDict[key] = value;
        }

        public void SetBool(string key, bool value)
        {
            boolDict[key] = value;
        }

        public void SetString(string key, string value)
        {
            stringDict[key] = value;
        }

        public int GetInt(string key)
        {
            return intDict[key];
        }

        public float GetFloat(string key)
        {
            return floatDict[key];
        }

        public bool GetBool(string key)
        {
            return boolDict[key];
        }

        public string GetString(string key)
        {
            return stringDict[key];
        }

        public bool ContainsInt(string key)
        {
            return intDict.ContainsKey(key);
        }

        public bool ContainsFloat(string key)
        {
            return floatDict.ContainsKey(key);
        }

        public bool ContainsBool(string key)
        {
            return boolDict.ContainsKey(key);
        }

        public bool ContainsString(string key)
        {
            return stringDict.ContainsKey(key);
        }

        public void RemoveInt(string key)
        {
            intDict.Remove(key);
        }

        public void RemoveFloat(string key)
        {
            floatDict.Remove(key);
        }

        public void RemoveBool(string key)
        {
            boolDict.Remove(key);
        }

        public void RemoveString(string key)
        {
            stringDict.Remove(key);
        }

        public void Clear()
        {
            intDict.Clear();
            floatDict.Clear();
            boolDict.Clear();
            stringDict.Clear();
        }
    }
}
