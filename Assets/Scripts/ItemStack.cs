using Newtonsoft.Json;
using System.Collections.Generic;

// Represents a single stack of one or more identical items
[System.Serializable]
public class ItemStack
{
    public string id;
    public int quantity;

    [JsonConstructor]
    public ItemStack() { }

    public ItemStack (string id, int quantity)
    {
        this.id = id;
        this.quantity = quantity;
    }
    public ItemStack (ItemData data)
    {
        id = data.ItemId;
        quantity = 1;
    }
    public ItemData GetData ()
    {
        return ContentLibrary.Instance.Items.Get(id);
    }
    public string GetName ()
    {
        return ContentLibrary.Instance.Items.Get(id).GetItemName(GetModifiers());
    }

    /*
     * Returns true iff this stack can't hold any more items.
     */
    public bool IsFull()
    {
        return GetData().MaxStackSize == quantity;
    }

    // Returns a new item stack with one fewer item, or null if this item stack
    // has only one item.
    public ItemStack Decrement()
    {
        if (quantity == 1) return null;
        return new ItemStack(id, quantity - 1);
    }

    public IDictionary<string, string> GetModifiers()
    {
        return ItemIdParser.ParseModifiers(id);
    }
}
