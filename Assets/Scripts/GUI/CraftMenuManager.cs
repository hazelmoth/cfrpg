using ContentLibraries;
using Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GUI
{
	public class CraftMenuManager : MonoBehaviour
	{
		[SerializeField] private GameObject listItemPrefab = null;
		[SerializeField] private GameObject itemListContent = null;
		[SerializeField] private Image selectedItemImage = null;
		[SerializeField] private TextMeshProUGUI selectedItemName = null;
		[SerializeField] private TextMeshProUGUI selectedItemDescription = null;
		[SerializeField] private TextMeshProUGUI selectedItemStats = null;
		[SerializeField] private TextMeshProUGUI selectedItemIngredients = null;
		[SerializeField] private Button craftButton = null;

		private readonly ItemData.Category[] categoryButtons = {
		ItemData.Category.Weapons,
		ItemData.Category.Tools,
		ItemData.Category.Clothing,
		ItemData.Category.Food,
		ItemData.Category.Drugs,
		ItemData.Category.Misc
	};


		private ItemData selectedItem;

		// Start is called before the first frame update
		void Start()
		{
			OnCategoryButton(0);
			ClearItemInfo();
		}

		public void OnCraftButton()
		{
			if (selectedItem == null)
			{
				return;
			}
			bool success = CraftingSystem.AttemptCraftItem(ActorRegistry.Get(PlayerController.PlayerActorId).actorObject, selectedItem);
		}

		public void OnCategoryButton(int buttonIndex)
		{
			PopulateItemList(categoryButtons[buttonIndex]);
		}

		private void PopulateItemList(ItemData.Category category)
		{
			foreach (Transform child in itemListContent.transform)
			{
				Destroy(child.gameObject);
			}

			foreach (ItemData item in ContentLibrary.Instance.Items.GetByCategory(category))
			{
				if (!item.IsCraftable || item.CraftingEnvironment != CraftingEnvironment.Handcrafted)
				{
					continue;
				}
				GameObject listItem = Instantiate(listItemPrefab);
				listItem.transform.SetParent(itemListContent.transform, false);

				CraftMenuItemListUiObject listItemScript = listItem.GetComponent<CraftMenuItemListUiObject>();
				listItemScript.image.sprite = item.DefaultIcon;
				listItemScript.text.text = item.DefaultName;
				listItemScript.clickEvent = OnItemListClickEvent;
				listItemScript.itemId = item.ItemId;
			}
		}

		private void OnItemListClickEvent(CraftMenuItemListUiObject receiver)
		{
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

			string ingredients = "";
			foreach (var ingredient in item.Ingredients)
			{
				ingredients += ContentLibrary.Instance.Items.Get(ingredient.itemId).DefaultName + " x" + ingredient.count + "\n";
			}
			selectedItemIngredients.text = ingredients;

			selectedItem = item;
		}

		private void ClearItemInfo()
		{
			selectedItemName.text = "";
			selectedItemImage.color = Color.clear;
			selectedItemDescription.text = "Select an item to craft";
			selectedItemStats.text = "";
			selectedItemIngredients.text = "";
		}
	}
}
