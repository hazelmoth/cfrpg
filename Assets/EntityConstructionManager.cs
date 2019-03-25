using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityConstructionManager : MonoBehaviour
{
    static bool isPlacingEntity = false;
    static EntityData entityBeingPlaced = null;
    // Start is called before the first frame update
    void Start()
    {
        
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
            // Stop placing if placement was successful
            CancelEntityPlacement();
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
