using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

// Describes a location within a region.
[System.Serializable]
public class Location
{
    // These are relative coords to scene.
    // Note: don't make these readonly, or they won't be deserialized.
    public float x;
    public float y;
    public string scene;
    
    [JsonIgnore]
    public Vector2 Vector2 => new Vector2 (x, y);

    public Location()
    {
        // Default constructor necessary for serialization
    }
    public Location (float x, float y, string sceneName) {
        this.x = x;
        this.y = y;
        this.scene = sceneName;
    }
    public Location (Vector2 scenePos, string sceneName)
    {
        this.x = scenePos.x;
        this.y = scenePos.y;
        this.scene = sceneName;
    }

    public static bool operator ==(Location left, Location right)
    {
        return (left.x == right.x) && (left.y == right.y) && (left.scene == right.scene);
    }

    public static bool operator !=(Location left, Location right)
    {
        return !(left == right);
    }
}
