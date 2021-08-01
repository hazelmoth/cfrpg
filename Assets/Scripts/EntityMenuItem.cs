using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GUI;

public class EntityMenuItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image iconImage;
    private string entityId;
    private string itemText;
    private Sprite icon;
    public void SetEntity (EntityData entity)
    {
        entityId = entity.Id;
        itemText = entity.EntityName;

        SpriteRenderer spriter = entity.EntityPrefab.GetComponentInChildren<SpriteRenderer>();
        if (spriter != null)
        {
            icon = spriter.sprite;
        }

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
