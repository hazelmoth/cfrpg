using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapInterface : MonoBehaviour
{
    private static Tilemap mainGroundTilemap = null;

    private static Tilemap MainGroundTilemap
    {
        get
        {
            if (mainGroundTilemap == null)
                mainGroundTilemap = TilemapLibrary.GetGroundTilemap(SceneObjectManager.WorldSceneId);
            return mainGroundTilemap;
        }
    }

    public static Vector2 GetCenterPositionOfTile(Vector2 tilePos)
    {
        return new Vector2((float) (tilePos.x + 0.5), (float) (tilePos.y + 0.5));
    }

    public static Vector2 WorldPosToScenePos(Vector2 worldPos, string sceneName)
    {
        Vector2 sceneRoot = SceneObjectManager.GetSceneObjectFromId(sceneName).transform.position;
        return worldPos - sceneRoot;
    }

    public static Vector2 ScenePosToWorldPos(Vector2 scenePos, string sceneName)
    {
        GameObject sceneObject = SceneObjectManager.GetSceneObjectFromId(sceneName);
        if (sceneObject == null)
        {
            Debug.LogError("Given scene object does not exist.");
            return scenePos;
        }

        Vector2 sceneRoot = sceneObject.transform.position;
        return scenePos + sceneRoot;
    }

    public static Vector2Int FloorToTilePos(Vector2 pos)
    {
        return new Vector2Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y));
    }

    public static TileBase GetTileAtPosition(float x, float y, string sceneName)
    {
        return TilemapLibrary.GetGroundTilemap(sceneName)
            .GetTile(new Vector3Int(Mathf.FloorToInt(x), Mathf.FloorToInt(y), 0));
    }

    public static void ChangeTile(int x, int y, TileBase tilePrefab, string sceneName, TilemapLayer layer)
    {
        if (layer == TilemapLayer.Ground)
            TilemapLibrary.GetGroundTilemap(sceneName).SetTile(new Vector3Int(x, y, 0), tilePrefab);
        else if (layer == TilemapLayer.GroundCover)
            TilemapLibrary.GetGroundCoverTilemap(sceneName).SetTile(new Vector3Int(x, y, 0), tilePrefab);
        else
            TilemapLibrary.GetCliffTilemap(sceneName).SetTile(new Vector3Int(x, y, 0), tilePrefab);
    }

    public static void ClearWorldTilemap()
    {
        MainGroundTilemap.ClearAllTiles();
    }

    public static void ClearTilemap(string sceneName)
    {
        TilemapLibrary.GetGroundTilemap(sceneName).ClearAllTiles();
    }

    public static void RefreshWorldTiles()
    {
        MainGroundTilemap.RefreshAllTiles();
    }

    public static void RefreshAllTilesInScene(string sceneName)
    {
        TilemapLibrary.GetGroundTilemap(sceneName).RefreshAllTiles();
    }


    public static TileBase GetTileAtWorldPosition(float x, float y, string sceneName)
    {
        Tilemap map = TilemapLibrary.GetGroundTilemap(sceneName);
        x -= map.transform.position.x;
        y -= map.transform.position.y;
        return map.GetTile(new Vector3Int(Mathf.FloorToInt(x), Mathf.FloorToInt(y), 0));
    }

    public static BoundsInt GetBoundsOfScene(string sceneName)
    {
        Tilemap map = TilemapLibrary.GetGroundTilemap(sceneName);
        map.CompressBounds();
        return map.cellBounds;
    }
}
