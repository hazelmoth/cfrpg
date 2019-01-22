using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryIconInteractable : InventoryIcon, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler {

	private RectTransform rectTransform;
	private Vector3 startPosition;
	private bool isDragging = false;
	private GameObject lastTouchedObject;
	private GameObject originalParent;
	private GameObject draggingParent; // Parent object to child icons to as they are being dragged
	private InventoryScreenManager invScreen;
	private GameObject inventoryBackgroundPanel;

	void Start () {
		renderer = GetComponent<Image> ();
		rectTransform = GetComponent<RectTransform> ();
		originalParent = gameObject.transform.parent.gameObject;
		draggingParent = GameObject.Find ("Drag Parent Object");
		invScreen = GameObject.FindObjectOfType<InventoryScreenManager> ();
		inventoryBackgroundPanel = invScreen.GetBackgroundPanel ();
	}

	public void OnPointerDown (PointerEventData eventData)
	{
		if (!isDragging)
		{
			GameObject activeSlot = eventData.pointerCurrentRaycast.gameObject;

			if (activeSlot.GetComponent<InventoryIconInteractable>())
			{
				activeSlot = activeSlot.transform.parent.gameObject;
			}

			if (activeSlot.tag == "InventorySlot")
			{
				// This is where we would put code to display info about an item after it is
				// clicked on in the inventory screen, if we were doing that.
			}
		}
	}

	public void OnBeginDrag (PointerEventData eventData)
	{
		startPosition = rectTransform.position;
		isDragging = true;
		setRaycastTarget (false);

		gameObject.transform.SetParent (draggingParent.transform);
	}

	public void OnDrag (PointerEventData eventData)
	{
		rectTransform.position = Input.mousePosition;

		lastTouchedObject = eventData.pointerCurrentRaycast.gameObject;
	}

	public void OnEndDrag (PointerEventData eventData)
	{
		setRaycastTarget (true);
		ManageDrag (lastTouchedObject);
	}

	void setRaycastTarget(bool recieveRaycasts) {
		renderer.raycastTarget = recieveRaycasts;
	}
		
	void OnApplicationFocus(bool isFocused)
	{
		if (!isFocused && isDragging)
		{
			rectTransform.position = startPosition;
			isDragging = false;
			gameObject.transform.SetParent (originalParent.transform);
		}
	}

	void ManageDrag (GameObject dragDestination)
	{
		if (dragDestination == null) {
			ResetIconPosition ();
			return;
		}

		if (dragDestination.GetComponent<InventoryIconInteractable>())
		{
			dragDestination = dragDestination.transform.parent.gameObject;
		}

		if (dragDestination.tag == "InventorySlot" || 
			dragDestination.tag == "HotbarSlot" || 
			dragDestination.tag == "HatSlot" || 
			dragDestination.tag == "ShirtSlot" || 
			dragDestination.tag == "PantsSlot" ||
			dragDestination.tag == "ContainerSlot")
		{
			invScreen.ManageInventoryDrag (originalParent, dragDestination);
			ResetIconPosition ();
		}
		else 
		{
			if (dragDestination == inventoryBackgroundPanel) {
				Debug.Log ("Icon dragged outside of inventory area");
				invScreen.ManageInventoryDragOutOfWindow (originalParent);
			}
			ResetIconPosition ();
		}
	}

	void ResetIconPosition ()
	{
		isDragging = false;
		setRaycastTarget (true);
		rectTransform.position = startPosition;
		gameObject.transform.SetParent (originalParent.transform);
	}



}
