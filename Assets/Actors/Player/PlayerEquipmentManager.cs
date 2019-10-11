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
		PointableItem equippedEquippable = currentEquippedItem as PointableItem;
		if (equippedEquippable != null) {
			TileMouseInputManager.SetMaxDistance (equippedEquippable.TileSelectorRange);
			TileMouseInputManager.SetCheckingForInput (equippedEquippable.UseTileSelector);
		} else {
			TileMouseInputManager.SetCheckingForInput (false);
		}
	}

	void OnItemUse (Vector3Int tilePos) {
		PointableItem equippedEquippable = currentEquippedItem as PointableItem;
		if (equippedEquippable != null) {
			equippedEquippable.Activate (tilePos);
		}
		SwingableItem equippedSwingable = currentEquippedItem as SwingableItem;
		if (equippedSwingable != null)
		{
			//equippedSwingable.Swing(GetComponent<Actor>());
		}
	}
}
