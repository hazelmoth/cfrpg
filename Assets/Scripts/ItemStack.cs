using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using ContentLibraries;
using Newtonsoft.Json;

/// Represents a single stack of one or more identical items.
[Serializable]
public class ItemStack
{
    [JsonConstructor]
    public ItemStack(string id, int quantity)
    {
        Id = id;
        Quantity = quantity;
    }

    public ItemStack(ItemData data)
    {
        Id = data.ItemId;
        Quantity = 1;
    }

    public string Id { get; }
    public int Quantity { get; }

    public ItemData GetData()
    {
        return ContentLibrary.Instance.Items.Get(Id);
    }

    public string GetName()
    {
        return ContentLibrary.Instance.Items.Get(Id).GetItemName(GetModifiers());
    }

    /// Returns true iff this stack can't hold any more items.
    public bool IsFull()
    {
        return GetData().MaxStackSize == Quantity;
    }

    /// Returns a copy of this stack with one more item.
    [Pure]
    public ItemStack Incremented()
    {
        return new ItemStack(Id, Quantity + 1);
    }

    /// Returns a copy of this stack with one fewer item, or null if this item stack has
    /// only one item.
    [Pure]
    public ItemStack Decremented()
    {
        if (Quantity == 1) return null;
        return new ItemStack(Id, Quantity - 1);
    }

    /// Returns a copy of this item stack with the provided value added to its quantity,
    /// or null if the resulting quantity is not positive.
    [Pure]
    public ItemStack AddQuantity(int added)
    {
        return Quantity + added <= 0 ? null : new ItemStack(Id, Quantity + added);
    }

    /// Returns the item modifiers currently appended to this item's ID, if any.
    public IDictionary<string, string> GetModifiers()
    {
        return ItemIdParser.ParseModifiers(Id);
    }
}