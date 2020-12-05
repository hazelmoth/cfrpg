using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SettlementSystem
{
    public class House : MonoBehaviour, ISaveable
    {
        private const string OwnerSaveTag = "owner";
        private SettlementManager manager;

        public string Owner { get; set; }
        string ISaveable.ComponentId => "settlementsystem.house";

        private void Start()
        {
            manager = FindObjectOfType<SettlementManager>();
            manager.RegisterHouse(this);
        }

        private void OnDestroy()
        {
            manager.UnregisterHouse(this);
        }

        IDictionary<string, string> ISaveable.GetTags()
        {
            Dictionary<string, string> tags = new Dictionary<string, string>();
            tags.Add(OwnerSaveTag, Owner);
            return tags;
        }

        void ISaveable.SetTags(IDictionary<string, string> tags)
        {
            if (tags.TryGetValue(OwnerSaveTag, out string value))
            {
                Owner = value;
            }
        }
    }
}