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

    public IDictionary<string, string> GetModifiers()
    {
        return ItemIdParser.ParseModifiers(id);
    }
}
