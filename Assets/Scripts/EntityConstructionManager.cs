using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ActorComponents;
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
            .CurrentRegion
            .info
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
                entityBeingPlaced.BaseShape.Select(location => mousePos + location).ToList();

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
        if (PlayerController.GetPlayerActor() == null || hasInitedForPlayerObject) return;
        ActorInventory inventory =
            PlayerController.GetPlayerActor()?.GetData()?.Get<ActorInventory>();

        if (inventory != null) inventory.OnInventoryChanged += CancelEntityPlacementIfIllegal;
        else Debug.LogError("EntityConstructionManager requires player actor to have an inventory");

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

        if (entityBeingPlaced.WorkToBuild > 0 && !GameConfig.GodMode)
        {
            // This entity takes nonzero work to build, so place a construction zone instead of the entity.
            placingConstructionZone = true;
            actualEntityToPlace = ContentLibrary.Instance.Entities.Get(ConstructionEntityID);
        }

        if (!RegionMapManager.AttemptPlaceEntityAtPoint(
            actualEntityToPlace,
            location,
            scene,
            entityBeingPlaced.BaseShape,
            out EntityObject placed))
            return;
        
        // Placement was successful.

        if (placingConstructionZone) placed.GetComponent<ConstructionSite>().Initialize(entityBeingPlaced.Id);

        if (!GameConfig.GodMode)
            // Remove expended resources from inventory.
            foreach (EntityData.CraftingIngredient ingredient in entityBeingPlaced.ConstructionIngredients)
                for (int i = 0; i < ingredient.quantity; i++)
                    PlayerController.GetPlayerActor().GetData().Get<ActorInventory>()
                        .RemoveOneInstanceOf(ingredient.itemId);

        // Stop placing.
        entityBeingPlaced = null;
        isPlacingEntity = false;
    }

    public static bool AttemptToInitiateConstruction(string entityId)
    {
        if (!PlayerCanConstruct(entityId) && !GameConfig.GodMode) return false;
        InitiateEntityPlacement(entityId);
        return true;
    }

    /// Checks if the player has the necessary resources and the entity is constructable
    public static bool PlayerCanConstruct(string entityId)
    {
        EntityData entity = ContentLibrary.Instance.Entities.Get(entityId);

        if (!entity.IsConstructable)
            return false;

        ImmutableList<EntityData.CraftingIngredient> ingredients = entity.ConstructionIngredients;
        List<string> ingredientItems = new List<string>();

        // Build a list of ingredient items to check with the inventory
        foreach (EntityData.CraftingIngredient ingredient in ingredients)
            for (int i = 0; i < ingredient.quantity; i++)
                ingredientItems.Add(ingredient.itemId);
        
        return PlayerController.GetPlayerActor()
            .GetData()
            .Get<ActorInventory>()
            .ContainsAllItems(ingredientItems);
    }

    private static void CancelEntityPlacementIfIllegal()
    {
        if (!isPlacingEntity)
            return;
        if (!PlayerCanConstruct(entityBeingPlaced.Id)) CancelEntityPlacement();
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
