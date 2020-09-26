using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class WorldListItem : MonoBehaviour, IPointerClickHandler
{
	[SerializeField] private TextMeshProUGUI nameText;
	[SerializeField] private TextMeshProUGUI subtitleText;

	public WorldSave save { get; set; }

	public void SetText (string nameText, string subtitleText)
	{
		this.nameText.text = nameText;
		this.subtitleText.text = subtitleText;
	}
	public void SetHighlighted (bool doHighlight)
	{
		Image background = GetComponent<Image>();
		if (doHighlight) {
			background.CrossFadeAlpha(1, 0.2f, true);
		}
		else
		{
			background.CrossFadeAlpha(0, 0.2f, true);
		}
	}
    public void OnPointerClick(PointerEventData eventData)
    {
        LoadWorldMenuManager.instance.OnListItemSelected(this);
    }

}
