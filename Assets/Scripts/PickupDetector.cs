using UnityEngine;

// detects when the player is standing next to something he can pick up
public class PickupDetector : MonoBehaviour
{
	private IPickuppable currentDetectedItem = null;

	public IPickuppable GetCurrentDetectedItem () 
	{
		return currentDetectedItem;
	}

	private void OnTriggerEnter2D (Collider2D other) 
	{
		IPickuppable itemObject = other.GetComponent<IPickuppable> ();

		if (
			itemObject != null && 
			itemObject.CurrentlyPickuppable &&
			(currentDetectedItem == null || 
			!ReferenceEquals(itemObject, currentDetectedItem))) 
		{
			currentDetectedItem = itemObject;
		}
	}

	private void OnTriggerStay2D (Collider2D other) 
	{
		IPickuppable itemObject = other.GetComponent<IPickuppable> ();
		if (currentDetectedItem == null && itemObject != null && itemObject.CurrentlyPickuppable) 
		{
			currentDetectedItem = itemObject;
		}
	}

	private void OnTriggerExit2D (Collider2D other) 
	{
		IPickuppable itemObject = other.GetComponent<IPickuppable> ();
		if (itemObject != null && ReferenceEquals(currentDetectedItem, itemObject)) {
			currentDetectedItem = null;
		}
	}
}
