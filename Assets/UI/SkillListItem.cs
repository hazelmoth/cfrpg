using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillListItem : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI titleText = null;
	[SerializeField] private TextMeshProUGUI valueText = null;
	[SerializeField] private Slider slider = null;

	public void SetSkillName(string name)
	{
		titleText.text = name;
	}

	// Takes a skill level from 0 to 100
	public void SetSkillLevel(int value)
	{
		valueText.text = value.ToString();
		slider.value = value / 100f;
	}
}
