using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// An entity that can be picked up as an item.
public class PickuppableEntity : MonoBehaviour, IPickuppable
{
    [SerializeField] private string itemId;
    [SerializeField] private int quantity = 1;
    
    public bool CurrentlyPickuppable => true;

    public ItemStack ItemPickup => new ItemStack(itemId, quantity);

    void IPickuppable.OnPickup()
    {
        TileLocation loc = GetComponent<EntityObject>().Location;
        RegionMapManager.RemoveEntityAtPoint(loc.Vector2.ToVector2Int(), loc.scene);
    }
}
