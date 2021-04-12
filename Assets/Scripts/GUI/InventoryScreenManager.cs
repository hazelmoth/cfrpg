using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace GUI
{

	// This class pretty much just handles displaying the proper items in the proper slots
	// and responding to things being dragged around, to pass that information on to the
	// PlayerInventory class. It also manages the ducat display and item info panel.
	// ...And keeps track of what inventory slot is selected for the info panel.
	public class InventoryScreenManager : MonoBehaviour
	{
		[SerializeField] private ContainerPanelRenderer containerRenderer = null;
		[SerializeField] private GameObject inventoryBackgroundPanel = null;
		[SerializeField] private GameObject inventoryGrid = null;
		[SerializeField] private GameObject apparelGrid = null;
		[SerializeField] private GameObject hotbarGrid = null;
		[SerializeField] private GameObject containerGrid = null;
		[SerializeField] private TextMeshProUGUI containerWindowTitle = null;
		[SerializeField] private GameObject hotbarHud = null;
		[SerializeField] private GameObject inventoryDragParent = null;
		[SerializeField] private TextMeshProUGUI ducatAmountText = null;
		[SerializeField] private GameObject selectedItemInfoPanel = null;
		[SerializeField] private TextMeshProUGUI selectedItemName = null;
		[SerializeField] private Image selectedItemIcon = null;
		[SerializeField] private GameObject selectedItemEatButton = null;

		private GameObject[] inventorySlots;
		private GameObject[] hotbarSlots;
		private GameObject[] hotbarHudSlots;
		private GameObject hatSlot;
		private GameObject shirtSlot;
		private GameObject pantsSlot;

		private GameObject currentSelectedSlot;
		private GameObject lastHighlightedSlot;
		private ItemStack currentSelectedItem;
		private bool hasInitializedForPlayer = false;

		private static Color invIconSelectedColor = new Color(201f / 255f, 146f / 255f, 99f / 255f);
		private static Color invIconNormalColor;

		[UsedImplicitly]
		private void Start()
		{
			inventorySlots = new GameObject[18];
			hotbarSlots = new GameObject[6];
			hotbarHudSlots = new GameObject[6];

			SetSelectedSlot(null);

			for (int i = 0; i < 18; i++)
			{
				inventorySlots[i] = inventoryGrid.transform.GetChild(i).gameObject;
			}
			for (int i = 0; i < 6; i++)
			{
				hotbarSlots[i] = hotbarGrid.transform.GetChild(i).gameObject;
			}
			for (int i = 0; i < 6; i++)
			{
				hotbarHudSlots[i] = hotbarHud.transform.GetChild(i).gameObject;
			}
			hatSlot = apparelGrid.transform.GetChild(0).gameObject;
			shirtSlot = apparelGrid.transform.GetChild(1).gameObject;
			pantsSlot = apparelGrid.transform.GetChild(2).gameObject;

			invIconNormalColor = hatSlot.GetComponent<Image>().color;

			UIManager.OnOpenInventoryScreen += ClearSelectedItem;
			UIManager.OnExitInventoryScreen += ClearSelectedItem;
			PlayerController.OnPlayerIdSet += InitializeForPlayerObject;
		}

		private void Update()
		{
			if (!PauseManager.Paused)
				UpdateDucatDisplay(ActorRegistry.Get(PlayerController.PlayerActorId).data.Wallet.Balance);
		}

		// Needs to be called after player is spawned
		private void InitializeForPlayerObject()
		{
			if (hasInitializedForPlayer)
				Debug.LogWarning("Inventory already initialized!");
			if (ActorRegistry.Get(PlayerController.PlayerActorId).actorObject != null && !hasInitializedForPlayer)
			{
				ActorRegistry.Get(PlayerController.PlayerActorId).data.Inventory.OnInventoryChangedLikeThis += UpdateInventoryPanels;
				ActorRegistry.Get(PlayerController.PlayerActorId).data.Inventory.OnCurrentContainerChanged += UpdateContainerPanel;
				UpdateInventoryPanels(ActorRegistry.Get(PlayerController.PlayerActorId).data.Inventory.GetContents());
				hasInitializedForPlayer = true;
			}
		}

		public GameObject GetBackgroundPanel()
		{
			return inventoryBackgroundPanel;
		}

		private void UpdateDucatDisplay(int ducats)
		{
			ducatAmountText.text = ducats.ToString();
		}

		private void SetInfoPanel(ItemStack item)
		{
			if (item == null)
			{
				ClearInfoPanel();
				return;
			}
			if (item.GetData().IsEdible)
				selectedItemEatButton.SetActive(true);
			else
				selectedItemEatButton.SetActive(false);

			selectedItemIcon.gameObject.SetActive(true);
			selectedItemIcon.sprite = item.GetData().Icon;
			selectedItemName.text = item.GetName();
		}

		private void ClearInfoPanel()
		{
			selectedItemIcon.gameObject.SetActive(false);
			selectedItemEatButton.SetActive(false);
			selectedItemIcon.sprite = null;
			selectedItemName.text = null;
		}

		private void ClearSelectedItem()
		{
			SetSelectedSlot(null);
			ClearInfoPanel();
		}
		// Make sure that whatever item is in the currently selected slot is being properly displayed
		private void UpdateSelectedSlot()
		{
			if (currentSelectedSlot == null)
			{
				ClearInfoPanel();
				return;
			}

			int slotIndex = FindIndexOfInventorySlot(currentSelectedSlot, out InventorySlotType slotType);
			ItemStack itemInSlot = ActorRegistry.Get(PlayerController.PlayerActorId).data.Inventory.GetItemInSlot(slotIndex, slotType);
			currentSelectedItem = itemInSlot;
			SetInfoPanel(itemInSlot);
		}

		// Updates the inventory screen to display the given lists of items
		private void UpdateInventoryPanels(ActorInventory.InvContents inv)
		{
			UpdateInventoryPanels(inv.mainInvArray, inv.hotbarArray, new ItemStack[] { inv.equippedHat, inv.equippedShirt, inv.equippedPants });
		}

		// Displays the given arrays of items in the inventory screen and hotbar
		private void UpdateInventoryPanels(ItemStack[] inventory, ItemStack[] hotbar, ItemStack[] apparel)
		{
			for (int i = 0; i < inventorySlots.Length; i++)
			{
				InventoryIcon icon = null;
				ItemStack item = inventory[i];
				if (inventorySlots[i].transform.childCount >= 1)
				{
					icon = inventorySlots[i].transform.GetChild(0).GetComponent<InventoryIcon>();
				}
				else
				{
					icon = inventoryDragParent.GetComponentInChildren<InventoryIcon>();
				}

				if (!icon)
					throw new UnityException("Inventory slot panel missing InventoryIcon component on child");

				SetSlotAppearance(icon, item);
			}

			for (int i = 0; i < hotbarSlots.Length; i++)
			{
				InventoryIcon icon = null;
				InventoryIcon hudIcon = null;
				ItemStack item = hotbar[i];

				if (hotbarSlots[i].transform.childCount >= 1)
				{
					icon = hotbarSlots[i].transform.GetChild(0).GetComponent<InventoryIcon>();
				}
				else
				{
					icon = inventoryDragParent.GetComponentInChildren<InventoryIcon>();
				}
				hudIcon = hotbarHudSlots[i].transform.GetChild(0).GetComponent<InventoryIcon>();

				if (!icon || !hudIcon)
					throw new UnityException("Hotbar slot panel missing component on child");

				SetSlotAppearance(hudIcon, item);
				SetSlotAppearance(icon, item);
				UpdateSelectedSlot();
			}

			InventoryIcon hatIcon = null;
			InventoryIcon shirtIcon = null;
			InventoryIcon pantsIcon = null;

			if (hatSlot.transform.childCount >= 1)
			{
				hatIcon = hatSlot.transform.GetChild(0).GetComponent<InventoryIcon>();
			}
			else
			{
				hatIcon = inventoryDragParent.GetComponentInChildren<InventoryIcon>();
			}
			if (shirtSlot.transform.childCount >= 1)
			{
				shirtIcon = shirtSlot.transform.GetChild(0).GetComponent<InventoryIcon>();
			}
			else
			{
				shirtIcon = inventoryDragParent.GetComponentInChildren<InventoryIcon>();
			}
			if (pantsSlot.transform.childCount >= 1)
			{
				pantsIcon = pantsSlot.transform.GetChild(0).GetComponent<InventoryIcon>();
			}
			else
			{
				pantsIcon = inventoryDragParent.GetComponentInChildren<InventoryIcon>();
			}

			SetSlotAppearance(hatIcon, apparel[0]);
			SetSlotAppearance(shirtIcon, apparel[1]);
			SetSlotAppearance(pantsIcon, apparel[2]);
		}

		// Updates the container panel to display the items in the current container,
		// following the layout provided by the container. Requires that the given
		// container is not null.
		private void UpdateContainerPanel(IContainer container)
		{
			Debug.Assert (container != null);
			
			if (container is ICustomLayoutContainer custom)
			{
				containerRenderer.RenderCustomLayout(custom);
			}
			else
			{
				containerRenderer.RenderNormalContainer(container);
			}
		}


		public bool AttemptInventoryMove(GameObject draggedSlot, GameObject destinationSlot)
		{
			InventorySlotType startType;
			InventorySlotType endType;
			int start = FindIndexOfInventorySlot(draggedSlot, out startType);
			int end = FindIndexOfInventorySlot(destinationSlot, out endType);

			ItemStack draggedItem = ActorRegistry.Get(PlayerController.PlayerActorId).data.Inventory.GetItemInSlot(start, startType);

			// Trigger the actual item move in the inventory
			ActorRegistry.Get(PlayerController.PlayerActorId).data.Inventory.AttemptMove(start, startType, end, endType);

			ItemStack itemInDest = ActorRegistry.Get(PlayerController.PlayerActorId).data.Inventory.GetItemInSlot(end, endType);
			ItemStack itemInStart = ActorRegistry.Get(PlayerController.PlayerActorId).data.Inventory.GetItemInSlot(start, startType);

			// Only change the selected inv slot if the drag was successful
			if (draggedItem != null && itemInDest != null && (itemInStart == null || ReferenceEquals(itemInDest, draggedItem)))
			{
				SetSelectedSlot(destinationSlot);
				UpdateSelectedSlot();
				return true;
			}
			return false;
		}

		public void ManageInventoryDragOutOfWindow(GameObject draggedSlot)
		{
			Debug.Log("Managing drop of " + draggedSlot.name);
			InventorySlotType slotType;
			int slotIndex = FindIndexOfInventorySlot(draggedSlot, out slotType);

			TriggerItemDrop(slotIndex, slotType);
			UpdateSelectedSlot();
		}

		// Display the item info in the info panel for the item in the given slot
		public void ManageSlotSelection(GameObject slot)
		{
			if (slot == null)
			{
				return;
			}

			InventorySlotType slotType;
			int slotIndex = FindIndexOfInventorySlot(slot, out slotType);

			currentSelectedItem = ActorRegistry.Get(PlayerController.PlayerActorId).data.Inventory.GetItemInSlot(slotIndex, slotType);
			SetSelectedSlot(slot);
		}

		private void TriggerItemDrop(int slot, InventorySlotType type)
		{
			Actor player = ActorRegistry.Get(PlayerController.PlayerActorId).actorObject;
			string playerScene = player.CurrentScene;

			player.GetData().Inventory.DropInventoryItem(
				slot,
				type,
				TilemapInterface.WorldPosToScenePos(player.transform.position, playerScene),
				playerScene);
		}

		// Sets the given slot as the currently selected one and highlights it
		private void SetSelectedSlot(GameObject slot)
		{
			if (lastHighlightedSlot != null)
				SetSlotHighlighted(lastHighlightedSlot, false);
			if (slot != null)
				SetSlotHighlighted(slot, true);
			currentSelectedSlot = slot;
			lastHighlightedSlot = slot;
			UpdateSelectedSlot();
		}

		// Displays the given item in the given slot. Displays the slot as empty if a null Item is passed.
		private void SetSlotAppearance(InventoryIcon slotIcon, ItemStack item)
		{
			if (item == null)
			{
				slotIcon.SetVisible(false);
				slotIcon.SetQuantityText("");
			}
			else
			{
				slotIcon.SetVisible(true);
				slotIcon.SetQuantityText(item.quantity.ToString());
				slotIcon.GetComponent<Image>().sprite = item.GetData().Icon;

				// Don't display quantity for single-item stacks
				if (item.quantity == 1)
				{
					slotIcon.SetQuantityText("");
				}
			}
		}

		private void SetSlotHighlighted(GameObject slot, bool slotIsHighlighted)
		{
			if (slot == null)
				return;
			Image image = slot.GetComponent<Image>();
			if (image == null)
			{
				Debug.LogWarning("Someone tried to change the appearance of a slot object without an Image component!");
				return;
			}
			if (slotIsHighlighted)
			{
				image.color = invIconSelectedColor;
			}
			else
			{
				image.color = invIconNormalColor;
			}
		}

		private int FindIndexOfInventorySlot(GameObject slot, out InventorySlotType type)
		{
			if (slot.CompareTag("InventorySlot"))
			{
				type = InventorySlotType.Inventory;
				for (int i = 0; i < inventorySlots.Length; i++)
				{
					if (inventorySlots[i].GetInstanceID() == slot.GetInstanceID())
					{
						return i;
					}
				}
			}
			if (slot.CompareTag("HotbarSlot"))
			{
				type = InventorySlotType.Hotbar;
				for (int i = 0; i < hotbarSlots.Length; i++)
				{
					if (hotbarSlots[i].GetInstanceID() == slot.GetInstanceID())
					{
						return i;
					}
				}
			}
			if (slot.CompareTag("ContainerSlot"))
			{
				type = InventorySlotType.ContainerInv;
				for (int i = 0; i < containerRenderer.Slots.Length; i++)
				{
					if (containerRenderer.Slots[i] == null) continue;
					if (containerRenderer.Slots[i].GetInstanceID() == slot.GetInstanceID())
					{
						return i;
					}
				}
			}
			if (slot.CompareTag("HatSlot"))
			{
				type = InventorySlotType.Hat;
				return 0;
			}
			if (slot.CompareTag("ShirtSlot"))
			{
				type = InventorySlotType.Shirt;
				return 1;
			}
			if (slot.CompareTag("PantsSlot"))
			{
				type = InventorySlotType.Pants;
				return 2;
			}
			type = 0;
			return 0;
		}

		[UsedImplicitly] // Button call
		public void OnEatButton()
		{
			if (currentSelectedItem != null)
			{
				bool wasEaten = ActorEatingSystem.AttemptEat(ActorRegistry.Get(PlayerController.PlayerActorId).actorObject, currentSelectedItem);

				if (!wasEaten)
					return;
				// Clear the inventory slot that was eaten from
				InventorySlotType eatenItemSlotType;
				int eatenItemSlot = FindIndexOfInventorySlot(currentSelectedSlot, out eatenItemSlotType);
				ActorRegistry.Get(PlayerController.PlayerActorId).data.Inventory.ClearSlot(eatenItemSlot, eatenItemSlotType);
			}
		}
	}
}