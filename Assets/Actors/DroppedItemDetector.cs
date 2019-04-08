using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// detects when the player is standing next to something he can pick up
public class DroppedItemDetector : MonoBehaviour
{
	DroppedItem currentDetectedItem = null;

	public DroppedItem GetCurrentDetectedItem () {
		return currentDetectedItem;
	}

	void OnTriggerEnter2D (Collider2D other) {
		DroppedItem itemObject = other.GetComponent<DroppedItem> ();
		if (
			itemObject != null && 
			(currentDetectedItem == null || 
			itemObject.GetInstanceID() != currentDetectedItem.GetInstanceID())
		) {
			currentDetectedItem = itemObject;
		}
	}
	void OnTriggerStay2D (Collider2D other) {
		DroppedItem itemObject = other.GetComponent<DroppedItem> ();
		if (currentDetectedItem == null && itemObject != null) {
			currentDetectedItem = itemObject;
		}
	}
	void OnTriggerExit2D (Collider2D other) {
		DroppedItem itemObject = other.GetComponent<DroppedItem> ();
		if (itemObject != null) {
			currentDetectedItem = null;
		}
	}
}
