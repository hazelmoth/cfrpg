﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class JobListItem : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI text;
	[SerializeField] Image backgroundImage;

	public string jobId;

	Color normalColor = new Color(0.671f, 0.427f, 0.254f, 1f);
	Color selectedColor = new Color(0.396f, 0.259f, 0.163f, 1f);

	public void OnClick ()
	{
		SurvivorMenuManager.OnJobSelected(this);
	}
	public void SetText(string text)
	{
		this.text.text = text;
	}
	public void SetHighlighted (bool doSetSelected)
	{
		if (doSetSelected)
			backgroundImage.color = selectedColor;
		else
			backgroundImage.color = normalColor;
	}
}
