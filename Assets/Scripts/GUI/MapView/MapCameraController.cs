using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapCameraController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerUpHandler, IPointerDownHandler
{

	[SerializeField] private Camera mapCamera;
	
	Vector3 mouseDownPosition;
	bool usedTwoFingersLastFrame = false;

	public bool Enabled { get; set; }

	Vector3 getInputPosWorldSpace () {
		Vector3 input = Input.mousePosition;
		Vector3 pos = mapCamera.ScreenToWorldPoint (new Vector3 (input.x, input.y, 10));
		return pos;
	}
	public void OnPointerDown(PointerEventData data) {
		mouseDownPosition = getInputPosWorldSpace ();
	}
	public void OnPointerUp(PointerEventData data) {
		RaycastHit hit;
		if (Physics.Raycast (data.pressPosition, Vector3.forward, out hit))
			Debug.Log (data.pressPosition);
	}
	public void OnBeginDrag(PointerEventData data) {
		if (Input.touchCount == 2 || !Enabled) {
			return;
		}
		usedTwoFingersLastFrame = false;
		mouseDownPosition = getInputPosWorldSpace ();
	}

	public void OnDrag(PointerEventData data) {
		if (!Enabled)
			return;
		if (Input.touchCount == 2) {
			usedTwoFingersLastFrame = true;
			return;
		}
		if (usedTwoFingersLastFrame) {
			mouseDownPosition = getInputPosWorldSpace ();
			usedTwoFingersLastFrame = false;
			return;
		}
		Vector3 inputPos = getInputPosWorldSpace ();
		var position = mapCamera.transform.position;
		position = new Vector3(position.x + (mouseDownPosition.x - inputPos.x), position.y + (mouseDownPosition.y - inputPos.y), position.z);
		mapCamera.transform.position = position;
	}

	public void OnEndDrag(PointerEventData data) {
		
	}
}
