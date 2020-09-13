using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActorComponents
{
    public class Trader
    {
        [JsonProperty]
        private string actorId;

        [JsonConstructor]
        public Trader() { }

        public Trader(string actorId)
        {
            this.actorId = actorId;
        }

        public struct ItemForSale
        {
            public ItemForSale(Item item, int price)
            {
                this.item = item; this.unitPrice = price;
            }
            public Item item;
            public int unitPrice;
        }

        public List<ItemForSale> GetItemsForSale ()
        {
            ActorData actor = ActorRegistry.Get(actorId).data;
            List<ItemForSale> items = new List<ItemForSale>();
            foreach (Item item in actor.Inventory.GetAllItems())
            {
                items.Add(new ItemForSale(item, item.GetData().BaseValue));
            }
            return items;
        }
    }
}