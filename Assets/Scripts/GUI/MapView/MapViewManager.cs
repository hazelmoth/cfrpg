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
    [SerializeField] private Sprite homeMarker = null;

    private void Start()
    {
        SetVisible(false);
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
        }
        
        // Center the camera over the map
        mapViewCamera.transform.position =
            (Vector3) (Vector2) map.dimensions / 2f + mapViewCamera.transform.position.z * Vector3.forward;

        mapViewCamera.orthographicSize = Camera.main.orthographicSize;
    }
}
