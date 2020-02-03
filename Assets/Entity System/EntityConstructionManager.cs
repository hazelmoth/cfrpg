using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityConstructionManager : MonoBehaviour
{
    static bool isPlacingEntity = false;
    static EntityData entityBeingPlaced = null;
	// Whether we've found a reference to the player object yet
	static bool hasInitedForPlayerObject = false;

	void Start () {
		SceneObjectManager.OnAnySceneLoaded += InitializeForPlayerObject;
		InitializeForPlayerObject ();
	}
	void InitializeForPlayerObject () {
		if (Player.instance != null && !hasInitedForPlayerObject) 
		{
			Player.instance.Inventory.OnInventoryChanged += CheckEntityPlacementIsStillLegal;

			hasInitedForPlayerObject = true;
			// Remove the event call once we've found the player
			SceneObjectManager.OnAnySceneLoaded -= InitializeForPlayerObject;
		}
	}

    // Update is called once per frame
    void Update()
    {
        if (isPlacingEntity)
        {
            List<Vector2Int> markerLocations = new List<Vector2Int>();
            foreach (Vector2Int location in entityBeingPlaced.baseShape)
            {
                Vector2Int newVector = new Vector2Int(
                    TileMouseInputManager.GetTilePositionUnderCursor().x, 
                    TileMouseInputManager.GetTilePositionUnderCursor().y
                );
                newVector += location;
                markerLocations.Add(newVector);
            }
            TileMarkerController.SetTileMarkers(markerLocations);

            if (Input.GetMouseButton(0))
            {
                OnPlaceEntityInput();
            }
        }
        else
        {
            TileMarkerController.HideTileMarkers();
        }
    }
    void OnPlaceEntityInput ()
    {
		string scene = Player.instance.CurrentScene;
        Vector2Int location = new Vector2Int (
            TileMouseInputManager.GetTilePositionUnderCursor().x,
            TileMouseInputManager.GetTilePositionUnderCursor().y
        );
		Vector2 scenePos = TilemapInterface.WorldPosToScenePos(location, scene);
        location = new Vector2Int((int)scenePos.x, (int)scenePos.y);

		if (WorldMapManager.AttemptPlaceEntityAtPoint(entityBeingPlaced, location, scene))
        {
			// Placement was successful

			// Remove expended resources from inventory
			foreach (EntityData.CraftingIngredient ingredient in entityBeingPlaced.initialCraftingIngredients) {
				for (int i = 0; i < ingredient.quantity; i++) {
					Player.instance.Inventory.RemoveOneInstanceOf (ItemLibrary.GetItemById(ingredient.itemId));
				}
			}
            // Stop placing
			entityBeingPlaced = null;
			isPlacingEntity = false;
        }
    }
	public static bool AttemptToInitiateConstruction (string entityId) {
		
		if (ResourcesAvailableToConstruct (entityId)) {
			InitiateEntityPlacement (entityId);
			return true;
		} else
			return false;
	}
		
	// Checks if the player has the necessary resources and the entity is constructable
	public static bool ResourcesAvailableToConstruct (string entityId) {
		EntityData entity = ContentLibrary.Instance.Entities.GetEntityFromID (entityId);

		if (!entity.isConstructable)
			return false;

		List<EntityData.CraftingIngredient> ingredients = entity.initialCraftingIngredients;
		List<Item> ingredientItems = new List<Item> ();

		// Build a list of ingredient items to check with the inventory
		foreach (EntityData.CraftingIngredient ingredient in ingredients) {
			for (int i = 0; i < ingredient.quantity; i++) {
				ingredientItems.Add (ItemLibrary.GetItemById (ingredient.itemId));
			}
		}
		if (Player.instance.Inventory.ContainsAllItems (ingredientItems)) {
			return true;
		} else
			return false;
	}
	public void CheckEntityPlacementIsStillLegal () {
		if (!isPlacingEntity)
			return;
		if (!ResourcesAvailableToConstruct(entityBeingPlaced.entityId)) {
			CancelEntityPlacement ();
		}
	}
    public static void InitiateEntityPlacement (string entityId)
    {
        entityBeingPlaced = ContentLibrary.Instance.Entities.GetEntityFromID(entityId);
        isPlacingEntity = true;

    }
    public static void CancelEntityPlacement ()
    {
        entityBeingPlaced = null;
        isPlacingEntity = false;
    }
}
