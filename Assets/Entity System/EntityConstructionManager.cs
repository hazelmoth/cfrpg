using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityConstructionManager : MonoBehaviour
{
	private static bool isPlacingEntity = false;
	private static EntityData entityBeingPlaced = null;
	// Whether we've found a reference to the player object yet
	private static bool hasInitedForPlayerObject = false;

	private void Start () {
		SceneObjectManager.OnAnySceneLoaded += InitializeForPlayerObject;
		InitializeForPlayerObject ();
	}

	private void InitializeForPlayerObject () {
		if (ActorRegistry.Get(PlayerController.PlayerActorId) != null && !hasInitedForPlayerObject) 
		{
			ActorRegistry.Get(PlayerController.PlayerActorId).data.Inventory.OnInventoryChanged += CheckEntityPlacementIsStillLegal;

			hasInitedForPlayerObject = true;
			// Remove the event call once we've found the player
			SceneObjectManager.OnAnySceneLoaded -= InitializeForPlayerObject;
		}
	}

    // Update is called once per frame
    private void Update()
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
				TileMarkerController.HideTileMarkers();
			}
        }
    }

    private void OnPlaceEntityInput ()
    {
		string scene = ActorRegistry.Get(PlayerController.PlayerActorId).actorObject.CurrentScene;
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
					ActorRegistry.Get(PlayerController.PlayerActorId).data.Inventory.RemoveOneInstanceOf (ContentLibrary.Instance.Items.Get(ingredient.itemId));
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
		EntityData entity = ContentLibrary.Instance.Entities.Get (entityId);

		if (!entity.isConstructable)
			return false;

		List<EntityData.CraftingIngredient> ingredients = entity.initialCraftingIngredients;
		List<ItemData> ingredientItems = new List<ItemData> ();

		// Build a list of ingredient items to check with the inventory
		foreach (EntityData.CraftingIngredient ingredient in ingredients) {
			for (int i = 0; i < ingredient.quantity; i++) {
				ingredientItems.Add (ContentLibrary.Instance.Items.Get (ingredient.itemId));
			}
		}
		if (ActorRegistry.Get(PlayerController.PlayerActorId).data.Inventory.ContainsAllItems (ingredientItems)) {
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
        entityBeingPlaced = ContentLibrary.Instance.Entities.Get(entityId);
        isPlacingEntity = true;

    }
    public static void CancelEntityPlacement ()
    {
        entityBeingPlaced = null;
        isPlacingEntity = false;
    }
}
