using System.Collections.Generic;
using System.Linq;
using ContentLibraries;
using ContinentMaps;
using UnityEngine;

public class EntityConstructionManager : MonoBehaviour
{
    private const string ConstructionEntityID = "construction";
    private static bool isPlacingEntity;
    private static EntityData entityBeingPlaced;
    // Whether we've found a reference to the player object yet
    private static bool hasInitedForPlayerObject;

    public static bool BuildingIsAllowed =>
        ContinentManager
            .LoadedMap
            .regionInfo[RegionMapManager.CurrentRegionCoords.x, RegionMapManager.CurrentRegionCoords.y]
            .playerHome;

    private void Start()
    {
        SceneObjectManager.OnAnySceneLoaded += InitializeForPlayerObject;
        InitializeForPlayerObject();
    }

    // Update is called once per frame
    private void Update()
    {
        if (isPlacingEntity)
        {
            Vector2Int mousePos = TileMouseInputManager.GetTilePositionUnderCursor().ToVector2().ToVector2Int();

            List<Vector2Int> markerLocations =
                entityBeingPlaced.baseShape.Select(location => mousePos + location).ToList();

            TileMarkerController.SetTileMarkers(markerLocations);

            if (Input.GetMouseButtonDown(0))
            {
                PlaceEntity();
                TileMarkerController.HideTileMarkers();
            }
        }
    }

    private static void InitializeForPlayerObject()
    {
        if (ActorRegistry.Get(PlayerController.PlayerActorId) == null || hasInitedForPlayerObject) return;
        
        ActorRegistry.Get(PlayerController.PlayerActorId).data.Inventory.OnInventoryChanged +=
            CancelEntityPlacementIfIllegal;

        hasInitedForPlayerObject = true;
        // Remove the event call once we've found the player
        SceneObjectManager.OnAnySceneLoaded -= InitializeForPlayerObject;
    }

    private void PlaceEntity()
    {
        string scene = ActorRegistry.Get(PlayerController.PlayerActorId).actorObject.CurrentScene;
        Vector2Int location = TileMouseInputManager.GetTilePositionUnderCursor().ToVector2().ToVector2Int();
        Vector2 scenePos = TilemapInterface.WorldPosToScenePos(location, scene);
        location = new Vector2Int((int) scenePos.x, (int) scenePos.y);

        EntityData actualEntityToPlace = entityBeingPlaced;
        bool placingConstructionZone = false;

        if (entityBeingPlaced.workToBuild > 0 && !GameConfig.GodMode)
        {
            // This entity takes nonzero work to build, so place a construction zone instead of the entity.
            placingConstructionZone = true;
            actualEntityToPlace = ContentLibrary.Instance.Entities.Get(ConstructionEntityID);
        }

        if (!RegionMapManager.AttemptPlaceEntityAtPoint(
            actualEntityToPlace,
            location,
            scene,
            entityBeingPlaced.baseShape,
            out EntityObject placed))
            return;
        
        // Placement was successful.

        if (placingConstructionZone) placed.GetComponent<ConstructionSite>().Initialize(entityBeingPlaced.entityId);

        if (!GameConfig.GodMode)
            // Remove expended resources from inventory.
            foreach (EntityData.CraftingIngredient ingredient in entityBeingPlaced.constructionIngredients)
                for (int i = 0; i < ingredient.quantity; i++)
                    ActorRegistry.Get(PlayerController.PlayerActorId).data.Inventory
                        .RemoveOneInstanceOf(ingredient.itemId);

        // Stop placing.
        entityBeingPlaced = null;
        isPlacingEntity = false;
    }

    public static bool AttemptToInitiateConstruction(string entityId)
    {
        if (!ResourcesAvailableToConstruct(entityId) && !GameConfig.GodMode) return false;
        InitiateEntityPlacement(entityId);
        return true;
    }

    // Checks if the player has the necessary resources and the entity is constructable
    public static bool ResourcesAvailableToConstruct(string entityId)
    {
        EntityData entity = ContentLibrary.Instance.Entities.Get(entityId);

        if (!entity.isConstructable)
            return false;

        List<EntityData.CraftingIngredient> ingredients = entity.constructionIngredients;
        List<string> ingredientItems = new List<string>();

        // Build a list of ingredient items to check with the inventory
        foreach (EntityData.CraftingIngredient ingredient in ingredients)
            for (int i = 0; i < ingredient.quantity; i++)
                ingredientItems.Add(ingredient.itemId);
        return ActorRegistry.Get(PlayerController.PlayerActorId).data.Inventory.ContainsAllItems(ingredientItems);
    }

    private static void CancelEntityPlacementIfIllegal()
    {
        if (!isPlacingEntity)
            return;
        if (!ResourcesAvailableToConstruct(entityBeingPlaced.entityId)) CancelEntityPlacement();
    }

    private static void InitiateEntityPlacement(string entityId)
    {
        entityBeingPlaced = ContentLibrary.Instance.Entities.Get(entityId);
        isPlacingEntity = true;
    }

    private static void CancelEntityPlacement()
    {
        entityBeingPlaced = null;
        isPlacingEntity = false;
    }
}
