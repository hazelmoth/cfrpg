using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// This class pretty much just handles displaying the proper items in the proper slots
// and responding to things being dragged around, to pass that information on to the
// PlayerInventory class. It also manages the ducat display and item info panel.
// ...And keeps track of what inventory slot is selected for the info panel.
public class InventoryScreenManager : MonoBehaviour {

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
	private GameObject[] containerSlots;

	private GameObject currentSelectedSlot;
	private GameObject lastHighlightedSlot;
	private ItemData currentSelectedItem;
	private bool hasInitializedForPlayer = false;

	private static Color invIconSelectedColor = new Color(201f/255f, 146f/255f, 99f/255f);
	private static Color invIconNormalColor;

	[UsedImplicitly]
	private void Start () {
		inventorySlots = new GameObject[18];
		hotbarSlots = new GameObject[6];
		hotbarHudSlots = new GameObject[6];
		containerSlots = new GameObject[containerGrid.transform.childCount];

		SetSelectedSlot (null);

		for (int i = 0; i < 18; i++) {
			inventorySlots [i] = inventoryGrid.transform.GetChild (i).gameObject;
		}
		for (int i = 0; i < 6; i++) {
			hotbarSlots [i] = hotbarGrid.transform.GetChild (i).gameObject;
		}
		for (int i = 0; i < 6; i++) {
			hotbarHudSlots [i] = hotbarHud.transform.GetChild (i).gameObject;
		}
		for (int i = 0; i < containerGrid.transform.childCount; i++) {
			containerSlots [i] = containerGrid.transform.GetChild (i).gameObject;
		}
		hatSlot = apparelGrid.transform.GetChild (0).gameObject;
		shirtSlot = apparelGrid.transform.GetChild (1).gameObject;
		pantsSlot = apparelGrid.transform.GetChild (2).gameObject;

		invIconNormalColor = hatSlot.GetComponent<Image> ().color;

		PlayerDucats.BalanceChanged += UpdateDucatDisplay;
		UIManager.OnOpenInventoryScreen += ClearSelectedItem;
		UIManager.OnExitInventoryScreen += ClearSelectedItem;
		PlayerController.OnPlayerIdSet += InitializeForPlayerObject;
	}

	// Needs to be called after player is spawned
	private void InitializeForPlayerObject () {
		if (hasInitializedForPlayer)
			Debug.LogWarning("Inventory already initialized!");
		if (ActorRegistry.Get(PlayerController.PlayerActorId).actorObject != null && !hasInitializedForPlayer) {
			ActorRegistry.Get(PlayerController.PlayerActorId).data.Inventory.OnInventoryChangedLikeThis += UpdateInventoryPanels;
			ActorRegistry.Get(PlayerController.PlayerActorId).data.Inventory.OnCurrentContainerChanged += UpdateContainerPanel;
			UpdateInventoryPanels(ActorRegistry.Get(PlayerController.PlayerActorId).data.Inventory.GetContents());
			hasInitializedForPlayer = true;
		}
		
	}
	
	public GameObject GetBackgroundPanel() {
		return inventoryBackgroundPanel;
	}

	private void UpdateDucatDisplay (int ducats) {
		ducatAmountText.text = ducats.ToString ();
	}

	private void SetInfoPanel (ItemData item) {
		if (item == null) {
			ClearInfoPanel ();
			return;
		}
		if (item.IsEdible)
			selectedItemEatButton.SetActive (true);
		else
			selectedItemEatButton.SetActive (false);
		
		selectedItemIcon.gameObject.SetActive (true);
		selectedItemIcon.sprite = item.ItemIcon;
		selectedItemName.text = item.ItemName;
	}

	private void ClearInfoPanel () {
		selectedItemIcon.gameObject.SetActive (false);
		selectedItemEatButton.SetActive (false);
		selectedItemIcon.sprite = null;
		selectedItemName.text = null;
	}

	private void ClearSelectedItem() {
		SetSelectedSlot (null);
		ClearInfoPanel ();
	}
	// Make sure that whatever item is in the currently selected slot is being properly displayed
	private void UpdateSelectedSlot() {
		if (currentSelectedSlot == null) {
			ClearInfoPanel ();
			return;
		}

		int slotIndex = FindIndexOfInventorySlot(currentSelectedSlot, out InventorySlotType slotType);
		ItemData itemInSlot = ActorRegistry.Get(PlayerController.PlayerActorId).data.Inventory.GetItemInSlot (slotIndex, slotType);
		currentSelectedItem = itemInSlot;
		SetInfoPanel (itemInSlot);
	}

	// Updates the inventory screen to display the given lists of items
	private void UpdateInventoryPanels (ActorInventory.InvContents inv)
	{
		UpdateInventoryPanels(inv.mainInvArray, inv.hotbarArray, new ItemData[] { inv.equippedHat, inv.equippedShirt, inv.equippedPants });
	}

	private void UpdateInventoryPanels (ItemData[] inventory, ItemData[] hotbar, ItemData[] apparel)
	{
		for (int i = 0; i < inventorySlots.Length; i++) {
			Image iconImage = null;
			ItemData item = inventory [i];
			if (inventorySlots [i].transform.childCount >= 1) {
				iconImage = inventorySlots [i].transform.GetChild (0).GetComponent<Image> ();
			} else {
				iconImage = inventoryDragParent.GetComponentInChildren<Image> ();
			}

			if (!iconImage)
				throw new UnityException ("Inventory slot panel missing image component on child");
			if (item == null) {
				iconImage.GetComponent<InventoryIcon> ().SetVisible (false);
			} else {
				iconImage.GetComponent<InventoryIcon> ().SetVisible (true);
				iconImage.sprite = item.ItemIcon;
			}
		}
		for (int i = 0; i < hotbarSlots.Length; i++) {
			Image iconImage = null;
			Image hudIconImage = null;
			ItemData item = hotbar [i];
			if (hotbarSlots [i].transform.childCount >= 1) {
				iconImage = hotbarSlots [i].transform.GetChild (0).GetComponent<Image> ();
			} else {
				iconImage = inventoryDragParent.GetComponentInChildren<Image> ();
			}
			hudIconImage = hotbarHudSlots [i].transform.GetChild (0).GetComponent<Image> ();

			if (!iconImage || !hudIconImage)
				throw new UnityException ("Hotbar slot panel missing image component on child");
			if (item == null) {
				iconImage.GetComponent<InventoryIcon> ().SetVisible (false);
				hudIconImage.GetComponent<InventoryIcon> ().SetVisible (false);
			} else {
				iconImage.GetComponent<InventoryIcon> ().SetVisible (true);
				hudIconImage.GetComponent<InventoryIcon> ().SetVisible (true);
				iconImage.sprite = item.ItemIcon;
				hudIconImage.sprite = item.ItemIcon;
			}
			UpdateSelectedSlot();
		}
		Image hatImage = null;
		Image shirtImage = null;
		Image pantsImage = null;
		if (hatSlot.transform.childCount >= 1) {
			hatImage = hatSlot.transform.GetChild (0).GetComponent<Image> ();
		} else {
			hatImage = inventoryDragParent.GetComponentInChildren<Image> ();
		}
		if (shirtSlot.transform.childCount >= 1) {
			shirtImage = shirtSlot.transform.GetChild (0).GetComponent<Image> ();
		} else {
			shirtImage = inventoryDragParent.GetComponentInChildren<Image> ();
		}
		if (pantsSlot.transform.childCount >= 1) {
			pantsImage = pantsSlot.transform.GetChild (0).GetComponent<Image> ();
		} else {
			pantsImage = inventoryDragParent.GetComponentInChildren<Image> ();
		}

		if (apparel[0] == null) {
			hatImage.GetComponent<InventoryIcon> ().SetVisible (false);
		} else {
			hatImage.GetComponent<InventoryIcon> ().SetVisible (true);
			hatImage.sprite = apparel[0].ItemIcon;
		}		
		if (apparel[1] == null) {
			shirtImage.GetComponent<InventoryIcon> ().SetVisible (false);
		} else {
			shirtImage.GetComponent<InventoryIcon> ().SetVisible (true);
			shirtImage.sprite = apparel[1].ItemIcon;
		}		
		if (apparel[2] == null) {
			pantsImage.GetComponent<InventoryIcon> ().SetVisible (false);
		} else {
			pantsImage.GetComponent<InventoryIcon> ().SetVisible (true);
			pantsImage.sprite = apparel[2].ItemIcon;
		}
	}

	private void UpdateContainerPanel (InteractableContainer container) {
		if (container == null)
			return;
		for (int i = 0; i < container.NumSlots; i++) {
			Image iconImage = null;
			ItemData item = container.GetContainerInventory() [i];
			if (containerSlots [i].transform.childCount >= 1) {
				iconImage = containerSlots [i].transform.GetChild (0).GetComponent<Image> ();
			} else {
				iconImage = inventoryDragParent.GetComponentInChildren<Image> ();
			}

			if (!iconImage)
				throw new UnityException ("Inventory slot panel missing image component on child");
			if (item == null) {
				iconImage.GetComponent<InventoryIcon> ().SetVisible (false);
			} else {
				iconImage.GetComponent<InventoryIcon> ().SetVisible (true);
				iconImage.sprite = item.ItemIcon;
			}
		}
		SetNumActiveContainerSlots (container.NumSlots);
		SetContainerWindowTitle (container.ContainerName);
	}

	private void SetNumActiveContainerSlots (int slots) {
		for (int i = 0; i < containerSlots.Length; i++) {
			if (i >= slots) {
				containerSlots [i].SetActive (false);
			}
			else {
				containerSlots [i].SetActive (true);
			}
		}
	}

	private void SetContainerWindowTitle (string title) {
		containerWindowTitle.text = title;
	}
		
	public void ManageInventoryDrag (GameObject draggedSlot, GameObject destinationSlot)
	{
		InventorySlotType startType;
		InventorySlotType endType;
		int start = FindIndexOfInventorySlot (draggedSlot, out startType);
		int end = FindIndexOfInventorySlot (destinationSlot, out endType);

		ItemData draggedItem = ActorRegistry.Get(PlayerController.PlayerActorId).data.Inventory.GetItemInSlot(start, startType);

		// Trigger the actual item move in the inventory
		ActorRegistry.Get(PlayerController.PlayerActorId).data.Inventory.AttemptMoveInventoryItem(start, startType, end, endType);

		// Only change the selected inv slot if the drag was successful

		ItemData itemInDest = ActorRegistry.Get(PlayerController.PlayerActorId).data.Inventory.GetItemInSlot(end, endType);
		if (draggedItem != null && (itemInDest != null && itemInDest.GetInstanceID() == draggedItem.GetInstanceID())) {
			SetSelectedSlot (destinationSlot);
		}
		UpdateSelectedSlot ();
	}

	public void ManageInventoryDragOutOfWindow (GameObject draggedSlot) 
	{
		Debug.Log ("Managing drop of " + draggedSlot.name);
		InventorySlotType slotType;
		int slotIndex = FindIndexOfInventorySlot (draggedSlot, out slotType);

		TriggerItemDrop(slotIndex, slotType);
		UpdateSelectedSlot ();
	}

	// Display the item info in the info panel for the item in the given slot
	public void ManageSlotSelection (GameObject slot) {
		if (slot == null) {
			return;
		}

		InventorySlotType slotType;
		int slotIndex = FindIndexOfInventorySlot (slot, out slotType);

		currentSelectedItem = ActorRegistry.Get(PlayerController.PlayerActorId).data.Inventory.GetItemInSlot (slotIndex, slotType);
		SetSelectedSlot (slot);
	}

	private void TriggerItemDrop(int slot, InventorySlotType type)
	{
		string playerScene = ActorRegistry.Get(PlayerController.PlayerActorId).actorObject.CurrentScene;

		ActorRegistry.Get(PlayerController.PlayerActorId).data.Inventory.DropInventoryItem(
			slot,
			type,
			TilemapInterface.WorldPosToScenePos(transform.position, playerScene),
			playerScene);
	}

	private void SetSelectedSlot (GameObject slot) {
		if (lastHighlightedSlot != null)
			SetSlotAppearance (lastHighlightedSlot, false);
		if (slot != null)
			SetSlotAppearance (slot, true);
		currentSelectedSlot = slot;
		lastHighlightedSlot = slot;
		UpdateSelectedSlot ();
	}

	private void SetSlotAppearance (GameObject slot, bool slotIsHighlighted) {
		if (slot == null)
			return;
		Image image = slot.GetComponent<Image> ();
		if (image == null) {
			Debug.LogWarning ("Someone tried to change the appearance of a slot object without an Image component!");
			return;
		}
		if (slotIsHighlighted) {
			image.color = invIconSelectedColor;
		} else {
			image.color = invIconNormalColor;
		}
	}

	private int FindIndexOfInventorySlot (GameObject slot, out InventorySlotType type) {
		if (slot.tag == "InventorySlot") {
			type = InventorySlotType.Inventory;
			for (int i = 0; i < inventorySlots.Length; i++) {
				if (inventorySlots[i].GetInstanceID() == slot.GetInstanceID()) {
					return i;
				}
			}
		}
		if (slot.tag == "HotbarSlot") {
			type = InventorySlotType.Hotbar;
			for (int i = 0; i < hotbarSlots.Length; i++) {
				if (hotbarSlots[i].GetInstanceID() == slot.GetInstanceID()) {
					return i;
				}
			}
		}
		if (slot.tag == "ContainerSlot") {
			type = InventorySlotType.ContainerInv;
			for (int i = 0; i < containerSlots.Length; i++) {
				if (containerSlots[i].GetInstanceID() == slot.GetInstanceID()) {
					return i;
				}
			}
		}
		if (slot.tag == "HatSlot") {
			type = InventorySlotType.Hat;
			return 0;
		}
		if (slot.tag == "ShirtSlot") {
			type = InventorySlotType.Shirt;
			return 1;
		}
		if (slot.tag == "PantsSlot") {
			type = InventorySlotType.Pants;
			return 2;
		}
		Debug.LogError ("An object was passed into FindIndexOfInventorySlot that doesn't appear to be a slot!");
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
