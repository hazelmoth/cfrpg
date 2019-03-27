using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EntityMenuItem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Image iconImage;
    string entityId;
    string itemText;
    Sprite icon;
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
