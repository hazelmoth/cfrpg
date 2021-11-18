﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using ContentLibraries;
using Items;
using Newtonsoft.Json;

/// Represents a single stack of one or more identical items.
/// This class is immutable.
[Serializable]
public class ItemStack
{
    [JsonConstructor]
    public ItemStack(string id, int quantity)
    {
        Id = id;
        Quantity = quantity;
    }

    /// Constructs a new ItemStack containing the given item with a quantity of 1.
    public ItemStack(ItemData data)
    {
        Id = data.ItemId;
        Quantity = 1;
    }

    /// Returns this stack's item ID, including any modifiers appended to it.
    public string Id { get; }

    /// Returns the quantity of items in this stack.
    public int Quantity { get; }

    public ItemData GetData()
    {
        return ContentLibrary.Instance.Items.Get(Id);
    }

    /// Returns the name of the item in this stack.
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
        return Quantity == 1
            ? null
            : new ItemStack(Id, Quantity - 1);
    }

    /// Returns a copy of this item stack with the provided value added to its quantity,
    /// or null if the resulting quantity is not positive.
    [Pure]
    public ItemStack AddQuantity(int added)
    {
        return Quantity + added <= 0
            ? null
            : new ItemStack(Id, Quantity + added);
    }

    /// Returns the item modifiers currently appended to this item's ID, if any.
    public IDictionary<string, string> GetModifiers()
    {
        return ItemIdParser.ParseModifiers(Id);
    }
}
