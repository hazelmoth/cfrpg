using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// Stores a dictionary of scene object names and their respective tilemaps
public static class TilemapLibrary
{
    private const int TilemapLayer = 10;
    private const int MapViewLayer = 6;
    private const string GroundTilemapTag = "GroundTilemap";
    private const string GroundCoverTilemapTag = "GroundCoverTilemap";
    private const string CliffsTilemapTag = "CliffsTilemap";

    private static IDictionary<string, Tilemap> groundMaps;
    private static IDictionary<string, Tilemap> groundCoverMaps;
    private static IDictionary<string, Tilemap> cliffMaps;

    /// Finds all the currently loaded tilemaps and stores them with the name of their scene
    /// (scenes need to be loaded to be added to the dictionary when this function is called)
    public static void BuildLibrary()
    {
        groundMaps = new Dictionary<string, Tilemap>();
        groundCoverMaps = new Dictionary<string, Tilemap>();
        cliffMaps = new Dictionary<string, Tilemap>();

        foreach (Tilemap tilemap in Object.FindObjectsOfType<Tilemap>())
        {
            // Only consider maps in tilemap layer
            if (tilemap.gameObject.layer != TilemapLayer) continue;

            if (!SceneObjectManager.SceneExists(SceneObjectManager.GetSceneIdForObject(tilemap.gameObject)))
            {
                Debug.LogWarning(
                    $"There's a tilemap in the scene that isn't under a registered scene object! Tag: {tilemap.tag}",
                    tilemap.gameObject);
                continue;
            }

            if (tilemap.CompareTag(GroundTilemapTag))
                groundMaps.Add(SceneObjectManager.GetSceneIdForObject(tilemap.gameObject), tilemap);
            else if (tilemap.CompareTag(GroundCoverTilemapTag))
                groundCoverMaps.Add(SceneObjectManager.GetSceneIdForObject(tilemap.gameObject), tilemap);
            else if (tilemap.CompareTag(CliffsTilemapTag))
                cliffMaps.Add(SceneObjectManager.GetSceneIdForObject(tilemap.gameObject), tilemap);
        }
    }

    public static Tilemap GetGroundTilemap(string scene)
    {
        if (groundMaps.ContainsKey(scene))
            return groundMaps[scene];

        if (SceneObjectManager.SceneExists(scene))
            Debug.LogWarning("Couldn't find ground tilemap for requested scene (\"" + scene + "\") in TilemapLibrary.");

        else
            Debug.LogError("Given scene \"" + scene + "\" not found.");
        return null;
    }

    public static Tilemap GetGroundCoverTilemap(string scene)
    {
        if (groundCoverMaps.ContainsKey(scene))
            return groundCoverMaps[scene];

        if (SceneObjectManager.SceneExists(scene))
            Debug.LogWarning(
                "Couldn't find ground cover tilemap for requested scene (\"" + scene + "\") in TilemapLibrary.");

        else
            Debug.LogWarning("Given scene \"" + scene + "\" not found.");
        return null;
    }

    public static Tilemap GetCliffTilemap(string scene)
    {
        if (cliffMaps.ContainsKey(scene))
            return cliffMaps[scene];

        if (SceneObjectManager.SceneExists(scene))
            Debug.LogWarning("Couldn't find cliff tilemap for requested scene (\"" + scene + "\") in TilemapLibrary.");

        else
            Debug.LogWarning("Given scene \"" + scene + "\" not found.");
        return null;
    }
}
