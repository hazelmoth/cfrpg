using System;
using System.Collections.Generic;
using ContentLibraries;
using JetBrains.Annotations;
using UnityEngine;

namespace Crafting
{
    /// A set of all recipes for a particular item.
    [Serializable]
    public class RecipeList : IContentItem
    {
        [SerializeField] private string itemId;
        [SerializeField] [CanBeNull] private List<CraftingRecipe> recipes;

        public string ItemId => itemId;
        public List<CraftingRecipe> Recipes => recipes;

        string IContentItem.Id => itemId;
    }
}
