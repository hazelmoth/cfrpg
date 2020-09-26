using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace GUI
{
	public class InventoryIconInteractable : InventoryIcon, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
	{

		private RectTransform rectTransform;
		private Vector3 startPosition;
		private bool isDragging = false;
		private GameObject lastTouchedObject;
		private GameObject originalParent;


		static private GameObject draggingParent; // Parent object to child icons to as they are being dragged
		static private InventoryScreenManager invScreen;
		static private GameObject inventoryBackgroundPanel;

		private void Start()
		{
			renderer = GetComponent<Image>();
			rectTransform = GetComponent<RectTransform>();
			originalParent = gameObject.transform.parent.gameObject;

			draggingParent = GameObject.Find("Drag Parent Object");
			invScreen = FindObjectOfType<InventoryScreenManager>();
			inventoryBackgroundPanel = invScreen.GetBackgroundPanel();
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			if (!isDragging)
			{
				GameObject activeSlot = eventData.pointerCurrentRaycast.gameObject;

				if (activeSlot.GetComponent<InventoryIconInteractable>())
				{
					activeSlot = activeSlot.transform.parent.gameObject;
				}

				if (activeSlot.tag == "InventorySlot" ||
					activeSlot.tag == "HotbarSlot" ||
					activeSlot.tag == "HatSlot" ||
					activeSlot.tag == "ShirtSlot" ||
					activeSlot.tag == "PantsSlot" ||
					activeSlot.tag == "ContainerSlot")
				{
					invScreen.ManageSlotSelection(activeSlot);
				}
			}
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			startPosition = rectTransform.position;
			isDragging = true;
			setRaycastTarget(false);

			gameObject.transform.SetParent(draggingParent.transform);
		}

		public void OnDrag(PointerEventData eventData)
		{
			rectTransform.position = Input.mousePosition;

			lastTouchedObject = eventData.pointerCurrentRaycast.gameObject;
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			setRaycastTarget(true);
			ManageDrag(lastTouchedObject);
		}

		private void setRaycastTarget(bool recieveRaycasts)
		{
			renderer.raycastTarget = recieveRaycasts;
		}

		private void OnApplicationFocus(bool isFocused)
		{
			if (!isFocused && isDragging)
			{
				rectTransform.position = startPosition;
				isDragging = false;
				gameObject.transform.SetParent(originalParent.transform);
			}
		}

		private void ManageDrag(GameObject dragDestination)
		{
			if (dragDestination == null)
			{
				ResetIconPosition();
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
				invScreen.ManageInventoryDrag(originalParent, dragDestination);
				ResetIconPosition();
			}
			else
			{
				if (dragDestination == inventoryBackgroundPanel)
				{
					invScreen.ManageInventoryDragOutOfWindow(originalParent);
				}
				ResetIconPosition();
			}
		}

		private void ResetIconPosition()
		{
			isDragging = false;
			setRaycastTarget(true);
			rectTransform.position = startPosition;
			gameObject.transform.SetParent(originalParent.transform);
		}



	}
}