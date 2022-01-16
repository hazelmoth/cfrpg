using System.Collections.Generic;
using System.Linq;
using ContentLibraries;
using Items;
using UnityEngine;

namespace Crafting
{
    /// Static methods for employing crafting recipes.
    public class CraftingSystem : MonoBehaviour
    {
        /// Attempts to craft the given product using the given recipe, drawing items from
        /// the given actor's inventory. If the actor has all the ingredients, removes
        /// them from the inventory, adds the product to the inventory, and returns true.
        /// Otherwise, returns false.
        public static bool AttemptCraftItem(Actor actor, ItemData product, CraftingRecipe recipe)
        {
            ActorInventory inv = actor.GetData().Inventory;

            // Dictionary mapping item id + item modifiers to required quantity.
            Dictionary<string, int> neededIngredients = new();

            foreach (CraftingIngredient ingredient in recipe.Ingredients)
                for (int i = 0; i < ingredient.count; i++)
                {
                    List<CraftingIngredient.RequiredItemTag> requiredTags =
                        ingredient.tagHandlingMode == CraftingIngredient.TagHandlingMode.RequireTags
                            ? ingredient.requiredTags.Value.ToList()
                            : new List<CraftingIngredient.RequiredItemTag>();

                    string ingredientIdWithMods = ItemIdParser.SetAllModifiers(
                        ingredient.itemBaseId,
                        requiredTags.ToDictionary(tag => tag.key, tag => tag.value));

                    if (neededIngredients.ContainsKey(ingredientIdWithMods))
                        neededIngredients[ingredientIdWithMods]++;
                    else
                        neededIngredients.Add(ingredientIdWithMods, 1);
                }

            if (!inv.ContainsAllItemsWithAtLeastProvidedModifiers(neededIngredients)) return false;

            foreach (CraftingIngredient ingredient in recipe.Ingredients)
                if (ingredient.tagHandlingMode == CraftingIngredient.TagHandlingMode.RequireTags)
                {
                    string ingredientIdWithMods = ItemIdParser.SetAllModifiers(
                        ingredient.itemBaseId,
                        ingredient.requiredTags.Value.ToDictionary(tag => tag.key, tag => tag.value));
                    inv.RemoveWithAtLeastProvidedModifiers(ingredientIdWithMods, ingredient.count);
                }
                else
                {
                    inv.Remove(ingredient.itemBaseId, ingredient.count);
                }

            for (int i = 0; i < recipe.ProductYield; i++)
                if (!inv.AttemptAddItem(new ItemStack(product)))
                    // If there's no space in actor inventory, drop at the actor's feet instead
                    DroppedItemSpawner.SpawnItem(
                        new ItemStack(product.ItemId, 1),
                        TilemapInterface.WorldPosToScenePos(actor.transform.position, actor.CurrentScene),
                        actor.CurrentScene,
                        true);

            return true;
        }

        /// Returns a recipe for the given item (including exact tags, if any) that uses
        /// the specified environment, or null if no such recipe exists.
        public static CraftingRecipe FindRecipe(string productId, CraftingEnvironment environment)
        {
            return ContentLibrary.Instance.CraftingRecipes.GetAll()
                .Where(recipeList => recipeList.ItemId == productId)
                .SelectMany(recipeList => recipeList.Recipes.Where(recipe => recipe.CraftingEnvironment == environment))
                .FirstOrDefault();
        }
    }
}
