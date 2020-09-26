﻿using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActorComponents
{

    public class Trader
    {
        private const float PurchasePriceMultiplier = 1.1f;
        
        [JsonProperty]
        private string actorId;

        [JsonConstructor]
        public Trader() { }

        public Trader(string actorId)
        {
            this.actorId = actorId;
        }

        // Maps Item IDs to the number the trader has available.
        public Dictionary<string, int> GetItemsForSale ()
        {
            ActorData actor = ActorRegistry.Get(actorId).data;
            Dictionary<string, int> items = new Dictionary<string, int>();

            foreach (Item item in actor.Inventory.GetAllItems())
            {
                if (items.ContainsKey(item.id))
                {
                    items[item.id] += item.quantity;
                }
                else
                {
                    items[item.id] = item.quantity;
                }
            }
            return items;
        }

        public int GetPurchasePrice (string item)
        {
            return (int)(ContentLibrary.Instance.Items.Get(item).BaseValue * PurchasePriceMultiplier);
        }
    }
}