using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildMenuManager : MonoBehaviour
{
	public delegate void MenuEvent();
	public static event MenuEvent OnConstructButton;

	private static BuildMenuManager instance;

    [SerializeField] private GameObject entityMenuContent = null;
    [SerializeField] private GameObject entityMenuItemPrefab = null;
	[SerializeField] private TextMeshProUGUI selectedEntityTitleText = null;
	[SerializeField] private TextMeshProUGUI selectedEntityRecipeText = null;
	[SerializeField] private GameObject ingredientsListTitleText = null;
	[SerializeField] private Image selectedEntityImage = null;
	[SerializeField] private TextMeshProUGUI constructButtonText;
	[SerializeField] private Material constructButtonNormalFontMaterial = null;
	[SerializeField] private Material constructButtonFadedFontMaterial = null;

	private static string currentSelectedEntityId = null;
	// Whether we've found a reference to the player object yet
	private static bool hasInitedForPlayerObject = false;

	private const string DefaultInfoPanelTitleText = "Select an object to construct.";
	private const string DefaultConstructButtonText = "Construct";
	private const string FadedConstructButtonText = "Missing ingredients";

	// Start is called before the first frame update
    private void Start()
    {
		instance = this;

		// Look for the player when a scene is loaded
		SceneObjectManager.OnAnySceneLoaded += InitializeForPlayerObject;

		InitializeForPlayerObject ();
		PopulateEntityMenu();
		ClearInfoPanel ();
    }

    private void InitializeForPlayerObject () {
		if (ActorRegistry.Get(PlayerController.PlayerActorId) != null && !hasInitedForPlayerObject) {
			// In case some resources get removed and we can no longer construct an item
			ActorRegistry.Get(PlayerController.PlayerActorId).data.Inventory.OnInventoryChanged += UpdateInfoPanel;
			hasInitedForPlayerObject = true;
			// Remove the event call once we've found the player
			SceneObjectManager.OnAnySceneLoaded -= InitializeForPlayerObject;
		}
	}

    private void OnDestroy()
	{
		OnConstructButton = null;
	}
	
	public static void PopulateEntityMenu () {
		List<EntityData> entities = new List<EntityData>();
		foreach (string id in ContentLibrary.Instance.Entities.GetEntityIdList()) {
			if (ContentLibrary.Instance.Entities.GetEntityFromID (id).isConstructable) {
				entities.Add (ContentLibrary.Instance.Entities.GetEntityFromID (id));
			}
		}
		PopulateEntityMenu(entities);

	}
	public static void PopulateEntityMenu (List<EntityData> entities)
    {
		// Destroy any list items that are already there
		foreach (Transform child in instance.entityMenuContent.transform) {
			GameObject.Destroy (child.gameObject);
		}
        foreach (EntityData entity in entities)
        {
            GameObject newMenuItem = GameObject.Instantiate(instance.entityMenuItemPrefab);
            newMenuItem.GetComponent<EntityMenuItem>().SetEntity(entity);
            newMenuItem.transform.SetParent(instance.entityMenuContent.transform, false);
        }

        GridLayoutGroup grid = instance.entityMenuContent.GetComponent<GridLayoutGroup>();
        if (grid != null)
        {
	        instance.entityMenuContent.GetComponent<RectTransform>().sizeDelta = new Vector2(
		        instance.entityMenuContent.GetComponent<RectTransform>().sizeDelta.x,
		        grid.cellSize.y * entities.Count);
        }
    }

	private void SetInfoPanel (string entityId) {
		if (entityId == null) {
			ClearInfoPanel ();
			return;
		}
		EntityData entity = ContentLibrary.Instance.Entities.GetEntityFromID (entityId);

		if (entity == null) {
			ClearInfoPanel ();
			return;
		}
		selectedEntityTitleText.text = entity.entityName;
		ingredientsListTitleText.SetActive(true);
		selectedEntityImage.color = Color.white;
		selectedEntityImage.sprite = entity.entityPrefab.GetComponentInChildren<SpriteRenderer> ().sprite;

		string recipeText = "";
		foreach (EntityData.CraftingIngredient ingredient in ContentLibrary.Instance.Entities.GetEntityFromID(entityId).initialCraftingIngredients) {
			recipeText += ingredient.quantity + " " + ContentLibrary.Instance.Items.Get (ingredient.itemId).ItemName + "\n";
		}
		selectedEntityRecipeText.text = recipeText;

		if (EntityConstructionManager.ResourcesAvailableToConstruct(entityId)) {
			constructButtonText.fontMaterial = constructButtonNormalFontMaterial;
			constructButtonText.text = DefaultConstructButtonText;
		} else {
			constructButtonText.fontMaterial = constructButtonFadedFontMaterial;
			constructButtonText.text = FadedConstructButtonText;
		}

	}

	private void UpdateInfoPanel () {
		if (currentSelectedEntityId == null) {
			ClearInfoPanel ();
			return;
		}
		SetInfoPanel (currentSelectedEntityId);
	}

	private void ClearInfoPanel () {
		selectedEntityImage.sprite = null;
		selectedEntityImage.color = Color.clear;
		selectedEntityRecipeText.text = null;
		selectedEntityTitleText.text = DefaultInfoPanelTitleText;
		ingredientsListTitleText.SetActive (false);

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
