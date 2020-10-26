using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySlotWhitelisted : InventorySlot
{
    private List<string> whitelist;

    // currently uses full IDs, including modifiers.
    public InventorySlotWhitelisted (List<string> itemWhitelist)
    {
        whitelist = itemWhitelist;
    }

    public override bool CanHoldItem(string itemId)
    {
        return whitelist.Contains(itemId);
    }
}
