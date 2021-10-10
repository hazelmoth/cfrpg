using System.Collections.Generic;
using ContentLibraries;
using ContinentMaps;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapViewManager : MonoBehaviour
{
    [SerializeField] private GameObject mapIconPrefab = null;
    [SerializeField] private Camera mapViewCamera = null;
    [SerializeField] private MapCameraController mapCameraInput = null;
    [SerializeField] private Tilemap tilemap = null;
    [SerializeField] private Tilemap dirtTilemap = null;
    [SerializeField] private Tilemap grassTilemap = null;
    [SerializeField] private TileBase landTile = null;
    [SerializeField] private TileBase waterTile = null;
    [SerializeField] private TileBase grassTile = null;
    [SerializeField] private TileBase deadGrassTile = null;
    [SerializeField] private Sprite heartlandsDetail = null;
    [SerializeField] private Sprite badlandsDetail = null;
    [SerializeField] private Sprite playerMarker = null;
    [SerializeField] private Sprite homeMarker = null;

    private List<GameObject> icons; // Stores currently epos.xispos.ting map icons, so
                                    // they can be destroyed later

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
    
    public void RenderMap(Dictionary<Vector2Int, RegionInfo> map)
    {
        DestroyIcons();
        
        foreach (Vector2Int pos in map.Keys)
        {
            RegionInfo region = map[pos];
            if (region.isWater)
            {
                tilemap.SetTile(new Vector3Int(pos.x, pos.y, 0), waterTile);
            }
            else
            {
                tilemap.SetTile(new Vector3Int(pos.x, pos.y, 0), landTile);
                if (region.biome != "desert") dirtTilemap.SetTile(new Vector3Int(pos.x, pos.y, 0), deadGrassTile);
                if (region.biome == "heartlands") grassTilemap.SetTile(new Vector3Int(pos.x, pos.y, 0), grassTile);
            }

            if (region.playerHome)
            {
                PlaceIcon(homeMarker, new Vector2Int(pos.x, pos.y));
            } 
            else if (region.feature != null && ContentLibrary.Instance.RegionFeatures.Get(region.feature).MapIcon)
            {
                PlaceIcon(ContentLibrary.Instance.RegionFeatures.Get(region.feature).MapIcon , new Vector2Int(pos.x, pos.y));
            }
            else if (region.feature == null && !region.isWater && region.biome == "heartlands")
            {
                PlaceIcon(heartlandsDetail, new Vector2Int(pos.x, pos.y));
            }
            else if (region.feature == null && !region.isWater && region.coasts.Count == 0 && region.biome == "badlands")
            {
                PlaceIcon(badlandsDetail, new Vector2Int(pos.x, pos.y));
            }

            if (region.Id == ContinentManager.CurrentRegionId)
            {
                PlaceIcon(playerMarker, new Vector2Int(pos.x, pos.y));
            }
        }
        
        // Center the camera over the map
        // mapViewCamera.transform.position =
        //    (Vector3) (Vector2) map.dimensions / 2f + mapViewCamera.transform.position.z * Vector3.forward;

        mapViewCamera.orthographicSize = Camera.main.orthographicSize / 2;
    }


    private void PlaceIcon(Sprite sprite, Vector2Int pos)
    {
        GameObject iconObject = Instantiate(mapIconPrefab, tilemap.gameObject.transform, true);
        iconObject.transform.position = pos + new Vector2(0.5f, 0.5f);
        iconObject.layer = 6;
        SpriteRenderer spriteRenderer = iconObject.GetComponent<SpriteRenderer>();
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
