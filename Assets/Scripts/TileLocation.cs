using UnityEngine;

// Defines a location on a specific scene
public struct TileLocation {
	// These are relative coords to scene 
	public readonly int x;
	public readonly int y;
	public readonly string Scene;
	public Vector2 Position {
		get {return new Vector2 (x, y);}
	}

	public TileLocation (int x, int y, string sceneName) {
		this.x = x;
		this.y = y;
		this.Scene = sceneName;
	}
	public TileLocation (Vector2Int scenePos, string sceneName)
	{
		this.x = scenePos.x;
		this.y = scenePos.y;
		this.Scene = sceneName;
	}

	public static bool operator ==(TileLocation left, TileLocation right)
	{
		return (left.x == right.x) && (left.y == right.y) && (left.Scene == right.Scene);
	}

	public static bool operator !=(TileLocation left, TileLocation right)
	{
		return !(left == right);
	}
}
