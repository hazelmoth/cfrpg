using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EntityMenuItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image iconImage;
    private string entityId;
    private string itemText;
    private Sprite icon;
    public void SetEntity (EntityData entity)
    {
        entityId = entity.entityId;
        itemText = entity.entityName;
        icon = entity.entityPrefab.GetComponentInChildren<SpriteRenderer>().sprite;

        text.text = itemText;
        iconImage.sprite = icon;
    }
    public string GetEntityId ()
    {
        return entityId;
    }
	public void OnClick () {
		BuildMenuManager.SelectMenuItem (this);
	}
}
