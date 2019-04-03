using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityConstructionManager : MonoBehaviour
{
    static bool isPlacingEntity = false;
    static EntityData entityBeingPlaced = null;

	void Start () {
		PlayerInventory.OnInventoryChanged += CheckEntityPlacementIsStillLegal;
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
        Vector2Int location = new Vector2Int (
            TileMouseInputManager.GetTilePositionUnderCursor().x,
            TileMouseInputManager.GetTilePositionUnderCursor().y
        );
        Vector2 scenePos = TilemapInterface.WorldPosToScenePos(location, "World");
        location = new Vector2Int((int)scenePos.x, (int)scenePos.y);
        if (WorldMapManager.AttemptPlaceEntityAtPoint(entityBeingPlaced, location, "World"))
        {
			// Placement was successful

			// Remove expended resources from inventory
			foreach (EntityData.CraftingIngredient ingredient in entityBeingPlaced.ingredients) {
				for (int i = 0; i < ingredient.quantity; i++) {
					PlayerInventory.RemoveOneInstanceOf (ItemManager.GetItemById(ingredient.itemId));
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
		EntityData entity = EntityLibrary.GetEntityFromID (entityId);

		if (!entity.isConstructable)
			return false;

		List<EntityData.CraftingIngredient> ingredients = entity.ingredients;
		List<Item> ingredientItems = new List<Item> ();

		// Build a list of ingredient items to check with the inventory
		foreach (EntityData.CraftingIngredient ingredient in ingredients) {
			for (int i = 0; i < ingredient.quantity; i++) {
				ingredientItems.Add (ItemManager.GetItemById (ingredient.itemId));
				Debug.Log (ingredient.itemId);
			}
		}
		if (PlayerInventory.ContainsAllItems (ingredientItems)) {
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
        entityBeingPlaced = EntityLibrary.GetEntityFromID(entityId);
        isPlacingEntity = true;

    }
    public static void CancelEntityPlacement ()
    {
        entityBeingPlaced = null;
        isPlacingEntity = false;
    }
}
