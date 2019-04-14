using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipmentManager : MonoBehaviour {

	Item currentEquippedItem;

	// Use this for initialization
	void Start () {
		HotbarManager.OnHotbarSlotSelected += OnItemEquipped;
		TileMouseInputManager.OnTileClicked += OnItemUse;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnItemEquipped (int index) {
		currentEquippedItem = Player.instance.Inventory.GetHotbarArray() [index];
		EquippableItem equippedEquippable = currentEquippedItem as EquippableItem;
		if (equippedEquippable != null) {
			TileMouseInputManager.SetMaxDistance (equippedEquippable.TileSelectorRange);
			TileMouseInputManager.SetCheckingForInput (equippedEquippable.UseTileSelector);
		} else {
			TileMouseInputManager.SetCheckingForInput (false);
		}
	}

	void OnItemUse (Vector3Int tilePos) {
		EquippableItem equippedEquippable = currentEquippedItem as EquippableItem;
		if (equippedEquippable != null) {
			equippedEquippable.Activate (tilePos);
		}
	}
}
