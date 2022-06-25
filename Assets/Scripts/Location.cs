using System;
using Newtonsoft.Json;
using UnityEngine;

/// Describes a location within a region.
[Serializable]
public class Location
{
    // These are relative coords to scene.
    // Note: don't make these readonly, or they won't be deserialized.
    public float x;
    public float y;
    public string scene;

    public Location()
    {
        // Default constructor necessary for serialization
    }

    public Location(float x, float y, string sceneName)
    {
        this.x = x;
        this.y = y;
        scene = sceneName;
    }

    public Location(Vector2 scenePos, string sceneName)
    {
        x = scenePos.x;
        y = scenePos.y;
        scene = sceneName;
    }

    [JsonIgnore] public Vector2 Vector2 => new Vector2(x, y);

    /// Returns a new Location with the applied offset from this one.
    public Location WithOffset(Vector2 offset) =>
        new Location(x + offset.x, y + offset.y, scene);

    protected bool Equals(Location other)
    {
        return x.Equals(other.x) && y.Equals(other.y) && scene == other.scene;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
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
