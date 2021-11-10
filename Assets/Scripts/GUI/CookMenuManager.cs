using System.Linq;
using ContentLibraries;
using Crafting;
using Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GUI
{
    public class CookMenuManager : MonoBehaviour
    {
        [SerializeField] private GameObject listItemPrefab = null;
        [SerializeField] private TextMeshProUGUI tabTitle = null;
        [SerializeField] private GameObject itemListContent = null;
        [SerializeField] private Image selectedItemImage = null;
        [SerializeField] private TextMeshProUGUI selectedItemName = null;
        [SerializeField] private TextMeshProUGUI selectedItemDescription = null;
        [SerializeField] private TextMeshProUGUI selectedItemStats = null;
        [SerializeField] private TextMeshProUGUI selectedItemIngredients = null;
        [SerializeField] private Button cookButton = null;

        private CraftingEnvironment activeEnvironment;
        private ItemData selectedItem;

        // Start is called before the first frame updaten
        private void Start()
        {
            PlayerInteractionManager.OnPlayerInteract += OnPlayerInteract;
            PopulateItemList(ItemData.Category.Food);
            ClearItemInfo();
        }

        public void OnCraftButton()
        {
            if (selectedItem == null) return;
            // Note: this assumes that there is at most one recipe for any combination of
            // item and CraftingEnvironment!
            CraftingRecipe recipe = ContentLibrary.Instance.CraftingRecipes.Get(selectedItem.ItemId)
                .Recipes.FirstOrDefault(r => r.CraftingEnvironment == activeEnvironment);

            bool success = recipe != null
                && CraftingSystem.AttemptCraftItem(
                    ActorRegistry.Get(PlayerController.PlayerActorId).actorObject,
                    selectedItem,
                    recipe);

            PopulateItemInfo(selectedItem.ItemId);
        }

        private void OnPlayerInteract(IInteractable interactable)
        {
            if (interactable is ICraftingStation craftingStation) SetWorkstation(craftingStation);
        }

        private void SetWorkstation(ICraftingStation craftingStation)
        {
            activeEnvironment = craftingStation.Environment;
            tabTitle.text = craftingStation.WorkstationName;
            PopulateItemList();
            ClearItemInfo();
            selectedItem = null;
        }

        private void PopulateItemList(ItemData.Category category)
        {
            ClearItemList();

            foreach (RecipeList recipeList in ContentLibrary.Instance.CraftingRecipes.GetAll())
            {
                // Check if this item has a recipe for this environment
                if (recipeList.Recipes.All(r => r.CraftingEnvironment != activeEnvironment)) continue;

                ItemData item = ContentLibrary.Instance.Items.Get(recipeList.ItemId);
                if (item.ItemCategory != category) continue;

                AddItemToList(item);
            }
        }

        private void PopulateItemList()
        {
            ClearItemList();

            foreach (RecipeList recipeList in ContentLibrary.Instance.CraftingRecipes.GetAll())
            {
                // Check if this item has a recipe for this environment
                if (recipeList.Recipes.All(r => r.CraftingEnvironment != activeEnvironment)) continue;

                ItemData item = ContentLibrary.Instance.Items.Get(recipeList.ItemId);
                AddItemToList(item);
            }
        }

        private void ClearItemList()
        {
            foreach (Transform child in itemListContent.transform) Destroy(child.gameObject);
        }

        private void AddItemToList(ItemData item)
        {
            GameObject listItem = Instantiate(listItemPrefab);
            listItem.transform.SetParent(itemListContent.transform, false);

            CraftMenuItemListUiObject listItemScript = listItem.GetComponent<CraftMenuItemListUiObject>();
            listItemScript.image.sprite = item.GetIcon(item.ItemId);
            listItemScript.text.text = item.DefaultName;
            listItemScript.clickEvent = OnItemListClickEvent;
            listItemScript.itemId = item.ItemId;
        }

        private void OnItemListClickEvent(CraftMenuItemListUiObject receiver)
        {
            selectedItem = ContentLibrary.Instance.Items.Get(receiver.itemId);
            PopulateItemInfo(receiver.itemId);
        }

        private void PopulateItemInfo(string itemId)
        {
            ItemData item = ContentLibrary.Instance.Items.Get(itemId);
            selectedItemImage.color = Color.white;
            selectedItemImage.sprite = item.GetIcon(itemId);
            selectedItemName.text = item.DefaultName;
            selectedItemDescription.text = item.Description;
            selectedItemStats.text = "";

            CraftingRecipe recipe = CraftingSystem.FindRecipe(itemId, activeEnvironment);
            string ingredients = "";
            foreach (CraftingIngredient ingredient in recipe.Ingredients)
            {
                ItemData data = ContentLibrary.Instance.Items.Get(ingredient.itemBaseId);
                ingredients += data.GetItemName(ingredient.GetTagsDict()) + " (";

                // TODO we need to properly check tag subsets for an accurate count of current ingredients
                // ingredients += ActorRegistry.Get(PlayerController.PlayerActorId).data.Inventory.GetCountOf(data.ItemId);
                // ingredients += "/";

                ingredients += ingredient.count + ")\n";
            }

            selectedItemIngredients.text = ingredients;
        }

        private void ClearItemInfo()
        {
            selectedItemName.text = "";
            selectedItemImage.color = Color.clear;
            selectedItemDescription.text = "Select a recipe to prepare";
            selectedItemStats.text = "";
            selectedItemIngredients.text = "";
        }
    }
}
