using System.Collections.Generic;
using ContentLibraries;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GUI
{
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

		[SerializeField] private TextMeshProUGUI BuildingsCategoryText;
		[SerializeField] private TextMeshProUGUI DefenseCategoryText;
		[SerializeField] private TextMeshProUGUI WorkstationsCategoryText;
		[SerializeField] private TextMeshProUGUI StorageCategoryText;
		[SerializeField] private TextMeshProUGUI FurnitureCategoryText;
		[SerializeField] private TextMeshProUGUI DecorationCategoryText;

		private static string currentSelectedEntityId = null;
		// Whether we've found a reference to the player object yet
		private static bool hasInitedForPlayerObject = false;

		private const string DefaultInfoPanelTitleText = "Select an object to construct.";
		private const string DefaultConstructButtonText = "Construct";
		private const string FadedConstructButtonText = "Missing ingredients";
		private static readonly Color NormalCategoryButtonTextColor = new Color(255 / 255f, 252 / 255f, 237 / 255f);
		private static readonly Color SelectedCategoryButtonTextColor = new Color(250 / 255f, 236 / 255f, 173 / 255f);

		// Start is called before the first frame update
		private void Start()
		{
			instance = this;

			// Look for the player when a scene is loaded
			SceneObjectManager.OnAnySceneLoaded += InitializeForPlayerObject;

			InitializeForPlayerObject();
			PopulateEntityMenu();
			ClearInfoPanel();
		}

		private void InitializeForPlayerObject()
		{
			if (ActorRegistry.Get(PlayerController.PlayerActorId) != null && !hasInitedForPlayerObject)
			{
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

		public static void PopulateEntityMenu()
		{
			PopulateEntityMenu(EntityCategory.Buildings);
		}

		private static void PopulateEntityMenu(EntityCategory category)
		{
			List<EntityData> entities = new List<EntityData>();
			foreach (string id in ContentLibrary.Instance.Entities.GetAllIds())
			{
				EntityCategory entCategory = ContentLibrary.Instance.Entities.Get(id).category;
				bool isConstructable = ContentLibrary.Instance.Entities.Get(id).isConstructable;
					
				// If in god mode, show Natural entities under Decorations
				if ((isConstructable || GameConfig.GodMode) && (entCategory == category 
				                                                || (GameConfig.GodMode 
				                                                    && category == EntityCategory.Decoration 
					                                                && entCategory == EntityCategory.Natural)))
				{
					entities.Add(ContentLibrary.Instance.Entities.Get(id));
				}
			}
			PopulateEntityMenu(entities);
		}

		public static void PopulateEntityMenu(List<EntityData> entities)
		{
			// Destroy any list items that are already there
			foreach (Transform child in instance.entityMenuContent.transform)
			{
				Destroy(child.gameObject);
			}
			foreach (EntityData entity in entities)
			{
				GameObject newMenuItem = Instantiate(instance.entityMenuItemPrefab);
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

		private void SetInfoPanel(string entityId)
		{
			if (entityId == null)
			{
				ClearInfoPanel();
				return;
			}
			EntityData entity = ContentLibrary.Instance.Entities.Get(entityId);

			if (entity == null)
			{
				ClearInfoPanel();
				return;
			}
			selectedEntityTitleText.text = entity.entityName;
			ingredientsListTitleText.SetActive(true);
			selectedEntityImage.color = Color.white;
			selectedEntityImage.sprite = entity.entityPrefab.GetComponentInChildren<SpriteRenderer>().sprite;

			string recipeText = "";
			foreach (EntityData.CraftingIngredient ingredient in ContentLibrary.Instance.Entities.Get(entityId).constructionIngredients)
			{
				recipeText += ingredient.quantity + " " + ContentLibrary.Instance.Items.Get(ingredient.itemId).DefaultName + "\n";
			}
			selectedEntityRecipeText.text = recipeText;

			if (EntityConstructionManager.ResourcesAvailableToConstruct(entityId) || GameConfig.GodMode)
			{
				constructButtonText.fontMaterial = constructButtonNormalFontMaterial;
				constructButtonText.text = DefaultConstructButtonText;
			}
			else
			{
				constructButtonText.fontMaterial = constructButtonFadedFontMaterial;
				constructButtonText.text = FadedConstructButtonText;
			}

		}

		private void UpdateInfoPanel()
		{
			if (currentSelectedEntityId == null)
			{
				ClearInfoPanel();
				return;
			}
			SetInfoPanel(currentSelectedEntityId);
		}

		private void ClearInfoPanel()
		{
			selectedEntityImage.sprite = null;
			selectedEntityImage.color = Color.clear;
			selectedEntityRecipeText.text = null;
			selectedEntityTitleText.text = DefaultInfoPanelTitleText;
			ingredientsListTitleText.SetActive(false);

			constructButtonText.fontMaterial = constructButtonFadedFontMaterial;
			constructButtonText.text = DefaultConstructButtonText;
		}

		private void ClearSelectedCategory()
		{
			BuildingsCategoryText.color = NormalCategoryButtonTextColor;
			DefenseCategoryText.color = NormalCategoryButtonTextColor;
			WorkstationsCategoryText.color = NormalCategoryButtonTextColor;
			StorageCategoryText.color = NormalCategoryButtonTextColor;
			FurnitureCategoryText.color = NormalCategoryButtonTextColor;
			DecorationCategoryText.color = NormalCategoryButtonTextColor;
		}

		public void AttemptToActivateConstruction()
		{
			if (currentSelectedEntityId == null)
			{
				return;
			}
			// Only register selection if construction is successfully initiated
			if (EntityConstructionManager.AttemptToInitiateConstruction(currentSelectedEntityId))
			{
				OnConstructButton?.Invoke();
			}
			UpdateInfoPanel();
		}
		public static void SelectMenuItem(EntityMenuItem item)
		{
			string id = item.GetEntityId();
			currentSelectedEntityId = item.GetEntityId();
			instance.SetInfoPanel(id);
		}

		public void BuildingsCategoryButton()
		{
			PopulateEntityMenu(EntityCategory.Buildings);
			ClearSelectedCategory();
			BuildingsCategoryText.color = SelectedCategoryButtonTextColor;
		}
		public void DefenseCategoryButton()
		{
			PopulateEntityMenu(EntityCategory.Defense);
			ClearSelectedCategory();
			DefenseCategoryText.color = SelectedCategoryButtonTextColor;
		}
		public void WorkstationsCategoryButton()
		{
			PopulateEntityMenu(EntityCategory.Workstations);
			ClearSelectedCategory();
			WorkstationsCategoryText.color = SelectedCategoryButtonTextColor;
		}
		public void StorageCategoryButton()
		{
			PopulateEntityMenu(EntityCategory.Storage);
			ClearSelectedCategory();
			StorageCategoryText.color = SelectedCategoryButtonTextColor;
		}
		public void FurnitureCategoryButton()
		{
			PopulateEntityMenu(EntityCategory.Furniture);
			ClearSelectedCategory();
			FurnitureCategoryText.color = SelectedCategoryButtonTextColor;
		}
		public void DecorationCategoryButton()
		{
			PopulateEntityMenu(EntityCategory.Decoration);
			ClearSelectedCategory();
			DecorationCategoryText.color = SelectedCategoryButtonTextColor;
		}
	}
}