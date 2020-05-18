using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HotbarManager : MonoBehaviour {

	[SerializeField] private GameObject hotbarGrid;
	private GameObject[] hotbarSlots;

	// Use this for initialization
	private void Start () {
		hotbarSlots = new GameObject[hotbarGrid.transform.childCount];
		for (int i = 0; i < hotbarGrid.transform.childCount; i++) {
			hotbarSlots [i] = hotbarGrid.transform.GetChild (i).gameObject;
		}
		KeyInputHandler.OnHotbarSelect += SetActiveHotbarSlot;
	}
	
	public void SetActiveHotbarSlot (int slot) {
		for (int i = 0; i < hotbarSlots.Length; i++) {
			if (i == slot - 1) {
				SetSlotHighlighted (hotbarSlots [i], true);
				EquipItem(i);
				
			}
			else
				SetSlotHighlighted (hotbarSlots [i], false);
		}
	}

	private void SetSlotHighlighted (GameObject slot, bool highlight) {
		foreach (Transform child in slot.transform) {
			if (child.tag == "HotbarActiveIndicator") {
				child.GetComponent<Image> ().enabled = highlight;
			}
		}
	}

	private void EquipItem(int slot)
	{
		ActorRegistry.Get(PlayerController.PlayerActorId).data.Inventory.SetEquippedHotbarSlot(slot);
		ItemData item = ActorRegistry.Get(PlayerController.PlayerActorId).data.Inventory.GetItemInSlot(slot, InventorySlotType.Hotbar);

		ActorRegistry.Get(PlayerController.PlayerActorId)
			.gameObject
			.GetComponent<ActorEquipmentManager>()
			.EquipItem(item);
	}
}
