using System.Collections.Generic;
using UnityEngine;

public class CraftingSystem : MonoBehaviour
{
	public static bool AttemptCraftItem(Actor actor, ItemData itemData)
    {
        ActorInventory inv = actor.GetData().Inventory;
        List<string> neededIngredients = new List<string>();
        foreach (var ingredient in itemData.Ingredients)
        {
	        for (int i = 0; i < ingredient.count; i++)
	        {
		        neededIngredients.Add(ingredient.itemId);
	        }
        }

        if (!inv.ContainsAllItems(neededIngredients))
        {
	        return false;
        }

        foreach (var ingredient in itemData.Ingredients)
        {
	        inv.Remove(ingredient.itemId, ingredient.count);
        }

        if (!inv.AttemptAddItem(new ItemStack(itemData)))
        {
            // If there's no space in actor inventory, drop at the actor's feet instead
            DroppedItemSpawner.SpawnItem(new ItemStack(itemData.ItemId, 1), TilemapInterface.WorldPosToScenePos(actor.transform.position, actor.CurrentScene), actor.CurrentScene, true);
        }

        return true;
    }
}
