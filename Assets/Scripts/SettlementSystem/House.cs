using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SettlementSystem
{
    public class House : MonoBehaviour, ISaveable
    {
        public const string OwnerSaveTag = "owner";
        
        private SettlementManager manager;

        public string Owner { get; set; }
        string ISaveable.ComponentId => "SettlementSystem.House";

        private void Start()
        {
            manager = FindObjectOfType<SettlementManager>();
            manager.RegisterHouse(this);
        }

        private void OnDestroy()
        {
            if (manager != null) manager.UnregisterHouse(this);
        }

        IDictionary<string, string> ISaveable.GetTags()
        {
            Dictionary<string, string> tags = new Dictionary<string, string> {{OwnerSaveTag, Owner}};
            return tags;
        }

        void ISaveable.SetTags(IDictionary<string, string> tags)
        {
            if (!tags.TryGetValue(OwnerSaveTag, out string value)) return;
            Owner = value;
            Debug.Log("House is owned by " + Owner);
        }
    }
}