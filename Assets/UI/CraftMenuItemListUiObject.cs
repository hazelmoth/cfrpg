using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CraftMenuItemListUiObject : MonoBehaviour, IPointerClickHandler
{
	public TextMeshProUGUI text;
	public Image image;
	public string itemId;
	public Action<CraftMenuItemListUiObject> clickEvent;

	void IPointerClickHandler.OnPointerClick(PointerEventData eventData) 
	{
		clickEvent(this);
	}
}
