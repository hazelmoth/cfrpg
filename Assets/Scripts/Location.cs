using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

// Describes a location within a region.
[System.Serializable]
public class Location
{
    // These are relative coords to scene 
    public readonly float x;
    public readonly float y;
    public readonly string Scene;
    
    [JsonIgnore]
    public Vector2 Vector2 {
        get {return new Vector2 (x, y);}
    }

    public Location()
    {
        // Default constructor necessary for serialization
    }
    public Location (float x, float y, string sceneName) {
        this.x = x;
        this.y = y;
        this.Scene = sceneName;
    }
    public Location (Vector2 scenePos, string sceneName)
    {
        this.x = scenePos.x;
        this.y = scenePos.y;
        this.Scene = sceneName;
    }

    public static bool operator ==(Location left, Location right)
    {
        return (left.x == right.x) && (left.y == right.y) && (left.Scene == right.Scene);
    }

    public static bool operator !=(Location left, Location right)
    {
        return !(left == right);
    }
}
