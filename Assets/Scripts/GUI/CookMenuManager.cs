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


		private ItemData selectedItem;

		// Start is called before the first frame updaten
		void Start()
		{
			PlayerInteractionManager.OnPlayerInteract += OnPlayerInteract;
			PopulateItemList(ItemData.Category.Food);
			ClearItemInfo();
		}

		public void OnCraftButton()
		{
			if (selectedItem == null)
			{
				return;
			}
			bool success = CraftingSystem.AttemptCraftItem(ActorRegistry.Get(PlayerController.PlayerActorId).actorObject, selectedItem);
			PopulateItemInfo(selectedItem.ItemId);
		}

		private void OnPlayerInteract(IInteractableObject interactable)
		{
			if (interactable is ICraftingStation craftingStation)
			{
				SetWorkstation(craftingStation);
			}
		}

		private void SetWorkstation(ICraftingStation craftingStation)
		{
			tabTitle.text = craftingStation.WorkstationName;
			PopulateItemList(craftingStation.Environment);
			ClearItemInfo();
			selectedItem = null;
		}

		private void PopulateItemList(ItemData.Category category)
		{
			ClearItemList();

			foreach (ItemData item in ContentLibrary.Instance.Items.GetByCategory(category))
			{
				if (!item.IsCraftable)
				{
					continue;
				}
				AddItemToList(item);
			}
		}
		private void PopulateItemList(CraftingEnvironment environment)
		{
			ClearItemList();
			foreach (ItemData item in ContentLibrary.Instance.Items.GetAll())
			{
				if (item.IsCraftable && item.CraftingEnvironment == environment)
				{
					AddItemToList(item);
				}
			}
		}

		private void ClearItemList()
		{
			foreach (Transform child in itemListContent.transform)
			{
				Destroy(child.gameObject);
			}
		}

		private void AddItemToList(ItemData item)
		{
			GameObject listItem = Instantiate(listItemPrefab);
			listItem.transform.SetParent(itemListContent.transform, false);

			CraftMenuItemListUiObject listItemScript = listItem.GetComponent<CraftMenuItemListUiObject>();
			listItemScript.image.sprite = item.ItemIcon;
			listItemScript.text.text = item.ItemName;
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
			selectedItemImage.sprite = item.ItemIcon;
			selectedItemName.text = item.ItemName;
			selectedItemDescription.text = item.Description;
			selectedItemStats.text = "";

			string ingredients = "";
			foreach (var ingredient in item.Ingredients)
			{
				ItemData data = ContentLibrary.Instance.Items.Get(ingredient.itemId);
				ingredients += data.ItemName + " (";
				ingredients += ActorRegistry.Get(PlayerController.PlayerActorId).data.Inventory.GetCountOf(data.ItemId);
				ingredients += "/" + ingredient.count + ")\n";

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