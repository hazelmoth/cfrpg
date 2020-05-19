using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingSystem : MonoBehaviour
{
	public static bool AttemptCraftItem(Actor actor, ItemData item)
    {
        ActorInventory inv = actor.GetData().Inventory;
        List<ItemData> neededIngredients = new List<ItemData>();
        foreach (var ingredient in item.Ingredients)
        {
	        for (int i = 0; i < ingredient.count; i++)
	        {
		        neededIngredients.Add(ContentLibrary.Instance.Items.Get(ingredient.itemId));
	        }
        }

        if (!inv.ContainsAllItems(neededIngredients))
        {
	        return false;
        }

        foreach (var ingredient in item.Ingredients)
        {
	        inv.Remove(ingredient.itemId, ingredient.count);
        }

        if (!inv.AttemptAddItemToInv(item))
        {
            // If there's no space in actor inventory, drop at the actor's feet instead
            DroppedItemSpawner.SpawnItem(item.ItemId, TilemapInterface.WorldPosToScenePos(actor.transform.position, actor.CurrentScene), actor.CurrentScene, true);
        }

        return true;
    }
}
