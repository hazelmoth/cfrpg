using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HotbarManager : MonoBehaviour {

	public delegate void HotbarEquipEvent (int slotIndex);
	public static event HotbarEquipEvent OnHotbarSlotSelected;
	[SerializeField] GameObject hotbarGrid;
	GameObject[] hotbarSlots;

	void OnDestroy()
	{
		OnHotbarSlotSelected = null;
	}
	// Use this for initialization
	void Start () {
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
				OnHotbarSlotSelected?.Invoke(i);
				ActorRegistry.Get(PlayerController.PlayerActorId).data.Inventory.SetEquippedHotbarSlot(i);
			}
			else
				SetSlotHighlighted (hotbarSlots [i], false);
		}
	}

	void SetSlotHighlighted (GameObject slot, bool highlight) {
		foreach (Transform child in slot.transform) {
			if (child.tag == "HotbarActiveIndicator") {
				child.GetComponent<Image> ().enabled = highlight;
			}
		}
	}
}
