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
	[SerializeField] Image constructButtonIcon;
	[SerializeField] TextMeshProUGUI constructButtonText;
	[SerializeField] Material constructButtonNormalFontMaterial = null;
	[SerializeField] Material constructButtonFadedFontMaterial = null;

	static string currentSelectedEntityId = null;

	const string DefaultInfoPanelTitleText = "Select an object to construct.";
	const string DefaultConstructButtonText = "Construct";
	const string FadedConstructButtonText = "Missing ingredients";
	static Color FadedConstructArrowColor = new Color (0.81f, 0.81f, 0.81f, 0.63f);
	static Color NormalConstructArrowColor = Color.white;

    // Start is called before the first frame update
    void Start()
    {
		instance = this;
		// In case some resources get removed and we can no longer construct an item
		PlayerInventory.OnInventoryChanged += UpdateInfoPanel;

        //TEST
		PopulateEntityMenu();
		ClearInfoPanel ();
    }
		
	void PopulateEntityMenu () {
		List<EntityData> entities = new List<EntityData>();
		foreach (string id in EntityLibrary.GetEntityIdList()) {
			if (EntityLibrary.GetEntityFromID (id).isConstructable) {
				entities.Add (EntityLibrary.GetEntityFromID (id));
			}
		}
		PopulateEntityMenu(entities);

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
		ingredientsListTitleText.SetActive(true);
		selectedEntityImage.color = Color.white;
		selectedEntityImage.sprite = entity.entityPrefab.GetComponentInChildren<SpriteRenderer> ().sprite;

		string recipeText = "";
		foreach (EntityData.CraftingIngredient ingredient in EntityLibrary.GetEntityFromID(entityId).ingredients) {
			recipeText += ingredient.quantity + " " + ItemManager.GetItemById (ingredient.itemId).itemName + "\n";
		}
		selectedEntityRecipeText.text = recipeText;

		if (EntityConstructionManager.ResourcesAvailableToConstruct(entityId)) {
			constructButtonIcon.color = NormalConstructArrowColor;
			constructButtonText.fontMaterial = constructButtonNormalFontMaterial;
			constructButtonText.text = DefaultConstructButtonText;
		} else {
			constructButtonIcon.color = FadedConstructArrowColor;
			constructButtonText.fontMaterial = constructButtonFadedFontMaterial;
			constructButtonText.text = FadedConstructButtonText;
		}

	}
	void UpdateInfoPanel () {
		if (currentSelectedEntityId == null) {
			ClearInfoPanel ();
			return;
		}
		SetInfoPanel (currentSelectedEntityId);
	}
	void ClearInfoPanel () {
		selectedEntityImage.sprite = null;
		selectedEntityImage.color = Color.clear;
		selectedEntityRecipeText.text = null;
		selectedEntityTitleText.text = DefaultInfoPanelTitleText;
		ingredientsListTitleText.SetActive (false);

		constructButtonIcon.color = FadedConstructArrowColor;
		constructButtonText.fontMaterial = constructButtonFadedFontMaterial;
		constructButtonText.text = DefaultConstructButtonText;
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
		UpdateInfoPanel ();
	}
	public static void SelectMenuItem (EntityMenuItem item) {
		string id = item.GetEntityId ();
		currentSelectedEntityId = item.GetEntityId();
		instance.SetInfoPanel (id);
	}
}
