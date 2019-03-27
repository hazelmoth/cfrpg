using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildMenuManager : MonoBehaviour
{
	public delegate void MenuEvent();
	public static event MenuEvent OnConstructButton;

	static BuildMenuManager instance;

    [SerializeField] GameObject entityMenuContent = null;
    [SerializeField] GameObject entityMenuItemPrefab = null;
	[SerializeField] TextMeshProUGUI selectedEntityTitleText = null;
	[SerializeField] TextMeshProUGUI selectedEntityRecipeText = null;
	[SerializeField] GameObject ingredientsListTitleText = null;
	[SerializeField] Image selectedEntityImage = null;

	static string currentSelectedEntityId = null;

	const string DefaultInfoPanelTitleText = "Select an object to construct.";

    // Start is called before the first frame update
    void Start()
    {
		instance = this;
        //TEST
        List<EntityData> entities = new List<EntityData>();
        foreach (string id in EntityLibrary.GetEntityIdList())
            entities.Add(EntityLibrary.GetEntityFromID(id));
        PopulateEntityMenu(entities);
		ClearInfoPanel ();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void PopulateEntityMenu (List<EntityData> entities)
    {
        foreach (EntityData entity in entities)
        {
            GameObject newMenuItem = GameObject.Instantiate(entityMenuItemPrefab);
            newMenuItem.GetComponent<EntityMenuItem>().SetEntity(entity);
            newMenuItem.transform.SetParent(entityMenuContent.transform);
        }
    }
	void SetInfoPanel (string entityId) {
		if (entityId == null) {
			ClearInfoPanel ();
			return;
		}
		EntityData entity = EntityLibrary.GetEntityFromID (entityId);

		if (entity == null) {
			ClearInfoPanel ();
			return;
		}
		selectedEntityTitleText.text = entity.entityName;
		selectedEntityRecipeText.text = entity.entityId;
		ingredientsListTitleText.SetActive(true);
		selectedEntityImage.color = Color.white;
		selectedEntityImage.sprite = entity.entityPrefab.GetComponentInChildren<SpriteRenderer> ().sprite;
	}
	void ClearInfoPanel () {
		selectedEntityImage.sprite = null;
		selectedEntityImage.color = Color.clear;
		selectedEntityRecipeText.text = null;
		selectedEntityTitleText.text = DefaultInfoPanelTitleText;
		ingredientsListTitleText.SetActive (false);
	}
	public void AttemptToActivateConstruction () {
		if (currentSelectedEntityId == null) {
			return;
		}
		// Only register selection if construction is succesfully initiated
		if (EntityConstructionManager.AttemptToInitiateConstruction (currentSelectedEntityId)) {
			if (OnConstructButton != null)
				OnConstructButton ();
		}
	}
	public static void SelectMenuItem (EntityMenuItem item) {
		string id = item.GetEntityId ();
		currentSelectedEntityId = item.GetEntityId();
		instance.SetInfoPanel (id);
	}
}
