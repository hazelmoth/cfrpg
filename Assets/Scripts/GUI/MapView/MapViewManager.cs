using System.Collections.Generic;
using ContinentMaps;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapViewManager : MonoBehaviour
{
    [SerializeField] private Camera mapViewCamera = null;
    [SerializeField] private MapCameraController mapCameraInput = null;
    [SerializeField] private Tilemap tilemap = null;
    [SerializeField] private TileBase landTile = null;
    [SerializeField] private TileBase waterTile = null;
    [SerializeField] private Sprite playerMarker = null;
    [SerializeField] private Sprite homeMarker = null;

    private List<GameObject> icons;

    private void Start()
    {
        SetVisible(false);
        icons = new List<GameObject>();
    }
    
    public void SetVisible(bool visible)
    {
        mapViewCamera.enabled = visible;
        tilemap.gameObject.SetActive(visible);
        mapCameraInput.Enabled = visible;
    }

    public bool CurrentlyVisible => mapViewCamera.enabled;
    
    public void RenderMap(ContinentMap map)
    {
        DestroyIcons();
        
        for (var x = 0; x < map.regionInfo.GetLength(0); x++)
        for (var y = 0; y < map.regionInfo.GetLength(1); y++)
        {
            RegionInfo region = map.regionInfo[x, y];
            if (region.topography == RegionTopography.Water)
            {
                tilemap.SetTile(new Vector3Int(x, y, 0), waterTile);
            }
            else
            {
                tilemap.SetTile(new Vector3Int(x, y, 0), landTile);
            }

            if (region.playerHome)
            {
                PlaceIcon(homeMarker, new Vector2Int(x, y));
            }

            if (x == RegionMapManager.CurrentRegionCoords.x && y == RegionMapManager.CurrentRegionCoords.y)
            {
                PlaceIcon(playerMarker, new Vector2Int(x, y));
            }
        }
        
        // Center the camera over the map
        mapViewCamera.transform.position =
            (Vector3) (Vector2) map.dimensions / 2f + mapViewCamera.transform.position.z * Vector3.forward;

        mapViewCamera.orthographicSize = Camera.main.orthographicSize;
    }


    private void PlaceIcon(Sprite sprite, Vector2Int pos)
    {
        GameObject iconObject = new GameObject("icon");
        iconObject.transform.SetParent(tilemap.gameObject.transform);
        iconObject.transform.position = (Vector2)pos + new Vector2(0.5f, 0.5f);
        iconObject.layer = 6;
        SpriteRenderer spriteRenderer = iconObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = 5;
        spriteRenderer.sprite = sprite;
        icons.Add(iconObject);
    }
    
    private void DestroyIcons()
    {
        icons.ForEach(Destroy);
        icons = new List<GameObject>();
    }
}