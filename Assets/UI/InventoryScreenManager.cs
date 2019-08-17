using UnityEngine;
using UnityEngine.UI;
using TMPro;

// This class pretty much just handles displaying the proper items in the proper slots
// and responding to things being dragged around, to pass that information on to the
// PlayerInventory class. It also manages the ducat display and item info panel.
// ...And keeps track of what inventory slot is selected for the info panel.
// This class should never directly access ActorInventory, but instead listen for events containing inventory data.
public class InventoryScreenManager : MonoBehaviour {

	[SerializeField] GameObject inventoryBackgroundPanel = null;
	[SerializeField] GameObject inventoryGrid = null;
	[SerializeField] GameObject apparelGrid = null;
	[SerializeField] GameObject hotbarGrid = null;
	[SerializeField] GameObject containerGrid = null;
	[SerializeField] TextMeshProUGUI containerWindowTitle = null;
	[SerializeField] GameObject hotbarHud = null;
	[SerializeField] GameObject inventoryDragParent = null;
	[SerializeField] TextMeshProUGUI ducatAmountText = null;
	[SerializeField] GameObject selectedItemInfoPanel = null;
	[SerializeField] TextMeshProUGUI selectedItemName = null;
	[SerializeField] Image selectedItemIcon = null;
	[SerializeField] GameObject selectedItemEatButton = null;

	GameObject[] inventorySlots;
	GameObject[] hotbarSlots;
	GameObject[] hotbarHudSlots;
	GameObject hatSlot;
	GameObject shirtSlot;
	GameObject pantsSlot;
	GameObject[] containerSlots;

	GameObject currentSelectedSlot;
	GameObject lastHighlightedSlot;
	Item currentSelectedItem;
	bool hasInitializedForPlayer = false;

	static Color invIconSelectedColor = new Color(201f/255f, 146f/255f, 99f/255f);
	static Color invIconNormalColor;

	public delegate void InventoryDragEvent (int startSlotIndex, InventorySlotType startSlotType, int destSlotIndex, InventorySlotType destSlotType);
	public delegate void InventoryDragOutOfWindowEvent (int slotIndex, InventorySlotType slotType);
	public static event InventoryDragEvent OnInventoryDrag;
	public static event InventoryDragOutOfWindowEvent OnInventoryDragOutOfWindow;

	void Start () {
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
		PlayerSpawner.OnPlayerSpawned += InitializeForPlayerObject;
	}

	void OnDestroy ()
	{
		OnInventoryDrag = null;
		OnInventoryDragOutOfWindow = null;
	}	

	// Needs to be called after player is spawned
	void InitializeForPlayerObject () {
		if (hasInitializedForPlayer)
			Debug.LogWarning("Inventory already initialized!");
		if (Player.instance != null && !hasInitializedForPlayer) {
			Player.instance.Inventory.OnInventoryChangedLikeThis += UpdateInventoryPanels;
			Player.instance.Inventory.OnCurrentContainerChanged += UpdateContainerPanel;
			UpdateInventoryPanels(Player.instance.Inventory.GetContents());
			hasInitializedForPlayer = true;
		}
		
	}
		
	public GameObject GetBackgroundPanel() {
		return inventoryBackgroundPanel;
	}
	void UpdateDucatDisplay (int ducats) {
		ducatAmountText.text = ducats.ToString ();
	}
	void SetInfoPanel (Item item) {
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
	void ClearInfoPanel () {
		selectedItemIcon.gameObject.SetActive (false);
		selectedItemEatButton.SetActive (false);
		selectedItemIcon.sprite = null;
		selectedItemName.text = null;
	}
	void ClearSelectedItem() {
		SetSelectedSlot (null);
		ClearInfoPanel ();
	}
	// Make sure that whatever item is in the currently selected slot is being properly displayed
	void UpdateSelectedSlot() {
		if (currentSelectedSlot == null) {
			ClearInfoPanel ();
			return;
		}

		int slotIndex = FindIndexOfInventorySlot(currentSelectedSlot, out InventorySlotType slotType);
		Item itemInSlot = Player.instance.Inventory.GetItemInSlot (slotIndex, slotType);
		currentSelectedItem = itemInSlot;
		SetInfoPanel (itemInSlot);
	}

	// Updates the inventory screen to display the given lists of items
	void UpdateInventoryPanels (ActorInventory.InvContents inv)
	{
		UpdateInventoryPanels(inv.mainInvArray, inv.hotbarArray, new Item[] { inv.equippedHat, inv.equippedShirt, inv.equippedPants });
	}
	void UpdateInventoryPanels (Item[] inventory, Item[] hotbar, Item[] apparel)
	{
		for (int i = 0; i < inventorySlots.Length; i++) {
			Image iconImage = null;
			Item item = inventory [i];
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
				iconImage.sprite = item.getIconSprite ();
			}
		}
		for (int i = 0; i < hotbarSlots.Length; i++) {
			Image iconImage = null;
			Image hudIconImage = null;
			Item item = hotbar [i];
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
				iconImage.sprite = item.getIconSprite ();
				hudIconImage.sprite = item.getIconSprite ();
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
			hatImage.sprite = apparel[0].getIconSprite ();
		}		
		if (apparel[1] == null) {
			shirtImage.GetComponent<InventoryIcon> ().SetVisible (false);
		} else {
			shirtImage.GetComponent<InventoryIcon> ().SetVisible (true);
			shirtImage.sprite = apparel[1].getIconSprite ();
		}		
		if (apparel[2] == null) {
			pantsImage.GetComponent<InventoryIcon> ().SetVisible (false);
		} else {
			pantsImage.GetComponent<InventoryIcon> ().SetVisible (true);
			pantsImage.sprite = apparel[2].getIconSprite ();
		}
	}
	void UpdateContainerPanel (InteractableContainer container) {
		if (container == null)
			return;
		for (int i = 0; i < container.NumSlots; i++) {
			Image iconImage = null;
			Item item = container.GetContainerInventory() [i];
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
				iconImage.sprite = item.getIconSprite ();
			}
		}
		SetNumActiveContainerSlots (container.NumSlots);
		SetContainerWindowTitle (container.ContainerName);
	}

	void SetNumActiveContainerSlots (int slots) {
		for (int i = 0; i < containerSlots.Length; i++) {
			if (i >= slots) {
				containerSlots [i].SetActive (false);
			}
			else {
				containerSlots [i].SetActive (true);
			}
		}
	}
	void SetContainerWindowTitle (string title) {
		containerWindowTitle.text = title;
	}
		
	public void ManageInventoryDrag (GameObject draggedSlot, GameObject destinationSlot)
	{
		InventorySlotType startType;
		InventorySlotType endType;
		int start = FindIndexOfInventorySlot (draggedSlot, out startType);
		int end = FindIndexOfInventorySlot (destinationSlot, out endType);

		Item draggedItem = Player.instance.Inventory.GetItemInSlot(start, startType);

		OnInventoryDrag?.Invoke(start, startType, end, endType);

		// Only change the selected inv slot if the drag was successful

		Item itemInDest = Player.instance.Inventory.GetItemInSlot(end, endType);
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

		OnInventoryDragOutOfWindow?.Invoke(slotIndex, slotType);
		UpdateSelectedSlot ();
	}

	// Whenever any item is clicked on or interacted with, display the item info in the info panel
	// (called by an inventory icon)
	public void ManageSlotSelection (GameObject slot) {
		if (slot == null) {
			return;
		}

		InventorySlotType slotType;
		int slotIndex = FindIndexOfInventorySlot (slot, out slotType);

		currentSelectedItem = Player.instance.Inventory.GetItemInSlot (slotIndex, slotType);
		SetSelectedSlot (slot);
	}

	void SetSelectedSlot (GameObject slot) {
		if (lastHighlightedSlot != null)
			SetSlotAppearance (lastHighlightedSlot, false);
		if (slot != null)
			SetSlotAppearance (slot, true);
		currentSelectedSlot = slot;
		lastHighlightedSlot = slot;
		UpdateSelectedSlot ();
	}
	void SetSlotAppearance (GameObject slot, bool slotIsHighlighted) {
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
	int FindIndexOfInventorySlot (GameObject slot, out InventorySlotType type) {
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
    public void OnEatButton()
    {
        if (currentSelectedItem != null)
        {
			bool wasEaten = ActorEatingSystem.AttemptEat(Player.instance, currentSelectedItem);

			if (!wasEaten)
				return;
			// Clear the inventory slot that was eaten from
            InventorySlotType eatenItemSlotType;
            int eatenItemSlot = FindIndexOfInventorySlot(currentSelectedSlot, out eatenItemSlotType);
			Player.instance.Inventory.ClearSlot(eatenItemSlot, eatenItemSlotType);
        }
    }
}
