using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// An InventorySlot is capable of holding a single ItemStack.
public class InventorySlot
{
    public ItemStack Contents { get; set; }
    public string SlotTag { get; set; }
    public virtual bool CanHoldItem(string itemId)
    {
        return true;
    }
}
