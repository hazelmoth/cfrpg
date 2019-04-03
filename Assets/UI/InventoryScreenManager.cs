using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// This class pretty much just handles displaying the proper items in the proper slots
// and responding to things being dragged around, to pass that information on to the
// PlayerInventory class. It also manages the ducat display for some reason.
public class InventoryScreenManager : MonoBehaviour {

	[SerializeField] GameObject inventoryBackgroundPanel;
	[SerializeField] GameObject inventoryGrid;
	[SerializeField] GameObject apparelGrid;
	[SerializeField] GameObject hotbarGrid;
	[SerializeField] GameObject containerGrid;
	[SerializeField] TextMeshProUGUI containerWindowTitle;
	[SerializeField] GameObject hotbarHud;
	[SerializeField] GameObject inventoryDragParent;
	[SerializeField] TextMeshProUGUI ducatAmountText;

	GameObject[] inventorySlots;
	GameObject[] hotbarSlots;
	GameObject[] hotbarHudSlots;
	GameObject hatSlot;
	GameObject shirtSlot;
	GameObject pantsSlot;
	GameObject[] containerSlots;

	public delegate void InventoryDragEvent (int startSlotIndex, InventorySlotType startSlotType, int destSlotIndex, InventorySlotType destSlotType);
	public delegate void InventoryDragOutOfWindowEvent (int slotIndex, InventorySlotType slotType);
	public static event InventoryDragEvent OnInventoryDrag;
	public static event InventoryDragOutOfWindowEvent OnInventoryDragOutOfWindow;

	void Start () {
		inventorySlots = new GameObject[18];
		hotbarSlots = new GameObject[6];
		hotbarHudSlots = new GameObject[6];
		containerSlots = new GameObject[containerGrid.transform.childCount];

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

		PlayerInventory.OnInventoryChangedLikeThis += UpdateInventoryPanels;
		PlayerInventory.OnCurrentContainerChanged += UpdateContainerPanel;
		PlayerDucats.BalanceChanged += UpdateDucatDisplay;
	}
		

	public GameObject GetBackgroundPanel() {
		return inventoryBackgroundPanel;
	}
	void UpdateDucatDisplay (int ducats) {
		ducatAmountText.text = ducats.ToString ();
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

		if (OnInventoryDrag != null)
			OnInventoryDrag (start, startType, end, endType);
		// Make sure the animations follow properly if we move the item in the current hotbar slot
	}

	public void ManageInventoryDragOutOfWindow (GameObject draggedSlot) 
	{
		Debug.Log ("Managing drop of " + draggedSlot.name);
		InventorySlotType slotType;
		int slotIndex = FindIndexOfInventorySlot (draggedSlot, out slotType);

		if (OnInventoryDragOutOfWindow != null) {
			OnInventoryDragOutOfWindow (slotIndex, slotType);
		}
		// Make sure the animations follow properly if we move the item in the current hotbar slot
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
}
