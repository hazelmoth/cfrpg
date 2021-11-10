using System;
using System.Collections.Generic;
using Items;
using UnityEngine;

namespace Crafting
{
    /// A particular recipe to craft some item.
    [Serializable]
    public class CraftingRecipe
    {
        [SerializeField] private CraftingEnvironment craftingEnvironment;
        [SerializeField] private int productYield;
        [SerializeField] private List<CraftingIngredient> ingredients;

        public CraftingEnvironment CraftingEnvironment => craftingEnvironment;
        public int ProductYield => productYield;
        public List<CraftingIngredient> Ingredients => ingredients;
    }
}
