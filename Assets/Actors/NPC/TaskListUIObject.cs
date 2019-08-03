﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TaskListUIObject : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI text;
	[SerializeField] Image icon;
	[SerializeField] Image backgroundImage;

	public string taskId;

	Color normalColor = new Color(0.671f, 0.427f, 0.254f, 1f);
	Color selectedColor = new Color(0.396f, 0.259f, 0.163f, 1f);

	public void OnClick ()
	{
		TaskAssignmentUIManager.OnTaskSelected(this);
	}
	public void SetText(string text)
	{
		this.text.text = text;
	}
	public void SetImage (Sprite sprite)
	{
		icon.sprite = sprite;
	}
	public void SetHighlighted (bool doSetSelected)
	{
		if (doSetSelected)
			backgroundImage.color = selectedColor;
		else
			backgroundImage.color = normalColor;
	}
}
