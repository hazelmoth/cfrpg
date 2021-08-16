using UnityEngine;

/// Defines a location within a region. Constrained to integer values.
public class TileLocation : Location {
	// These are relative coords to scene 
	public int X => Mathf.RoundToInt(x);
	public int Y => Mathf.RoundToInt(y);
	public Vector2Int Vector2Int => new Vector2Int(X, Y);

	public TileLocation (int x, int y, string sceneName) : base(x, y, sceneName) { }
	public TileLocation (Vector2Int scenePos, string sceneName) : base(scenePos, sceneName) { }
}
