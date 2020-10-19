using System.Collections.Generic;
using UnityEngine;

public class DroppedItemRegistry : MonoBehaviour
{
    private ISet<DroppedItem> items;

    public void RegisterItem(DroppedItem item)
    {
        if (items == null)
        {
            items = new HashSet<DroppedItem>();
        }

        if (items.Contains(item))
        {
            Debug.LogWarning("Given DroppedItem is already registered!");
        }

        items.Add(item);
    }

    public void RemoveItem(DroppedItem item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
        } else
        {
            Debug.LogWarning("Given DroppedItem isn't registered!");
        }
    }

    public ISet<DroppedItem> GetItems()
    {
        if (items == null)
        {
            items = new HashSet<DroppedItem>();
        }
        return items;
    }
}
