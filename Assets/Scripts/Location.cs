using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

// Describes a location within a region.
[System.Serializable]
public class Location
{
    protected bool Equals(Location other)
    {
        return x.Equals(other.x) && y.Equals(other.y) && scene == other.scene;
    }

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

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Location) obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = x.GetHashCode();
            hashCode = (hashCode * 397) ^ y.GetHashCode();
            hashCode = (hashCode * 397) ^ scene.GetHashCode();
            return hashCode;
        }
    }

    public static bool operator ==(Location left, Location right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Location left, Location right)
    {
        return !Equals(left, right);
    }
}
