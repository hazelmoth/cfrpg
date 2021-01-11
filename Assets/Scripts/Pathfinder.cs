using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Tilemaps;

// Finds paths between places.
// Note that this class relies on positions relative to scene origins, not absolute positions.
public class Pathfinder : MonoBehaviour {
	// TODO make the functions in this class take tilemap objects instead of scene names, to maximize independence

	// The maximum number of tiles that will be explored before pathfinding returns a failure.
	private const int TILE_EXPLORATION_LIMIT = 2000;

	private class NavTile 
	{
		public Vector2Int gridLocation; // Position of this tile in SCENE COORDINATES.
		public float travelCost; // The cost to reach this tile from our current location, taking into account travel distance as well as extra travel costs (from water, etc.)
		public float totalCost; // The travel cost, plus the distance from this tile to the target.
		public NavTile source;

		public NavTile () {}
		public NavTile (Vector2Int gridLocation, NavTile source, float travelCost, float totalCost) {
			this.gridLocation = gridLocation;
			this.source = source;
			this.travelCost = travelCost;
			this.totalCost = totalCost;
		}
	}

	private static float CalculateHeuristic(Vector2 tilePos, Vector2 destinationPos)
	{
		// Manhattan distance. Since we can only walk on two axes, this is the most accurate heuristic.
		float result = (Mathf.Abs(tilePos.x - destinationPos.x) + Mathf.Abs(tilePos.y - destinationPos.y));
		// Scale up the result slightly so that low distance-to-target is weighted more heavily than low travel cost.
		// This will reduce the number of equally-high-cost paths.
		result *= 1.001f;
		return result;
	}

	// Uses A* algorithm to find shortest path to the desired tile, avoiding tiles in the given set.
	// Returns null if no path exists or we hit tile exploration limit while searching.
	public static List<Vector2> FindPath (Vector2 scenePosStart, Vector2 scenePosEnd, string scene, ISet<Vector2> tileBlacklist) {

		int tileCounter = 0;

		Vector2Int startTileLocation = new Vector2Int (Mathf.FloorToInt (scenePosStart.x), Mathf.FloorToInt (scenePosStart.y));
		Vector2Int endTileLocation = new Vector2Int (Mathf.FloorToInt (scenePosEnd.x), Mathf.FloorToInt (scenePosEnd.y));

		//TODO verify that start and end tiles are valid

		if (WorldMapManager.GetMapObjectAtPoint(startTileLocation, scene) == null) {
			Debug.Log ("Attempted start location: " + startTileLocation.x + ", " + startTileLocation.y);
			Debug.LogError ("Attempted to start navigation at a point not defined in the world map");
		}
		else if (WorldMapManager.GetMapObjectAtPoint(startTileLocation, scene).groundMaterial == null) {
			Debug.LogWarning ("No ground material found at navigation start point");
		}

		List<NavTile> tileQueue = new List<NavTile> ();
		List<NavTile> finishedTiles = new List<NavTile> ();
		List<Vector2> path = new List<Vector2> ();
		NavTile currentTile = new NavTile(startTileLocation, null, 0, CalculateHeuristic(startTileLocation, endTileLocation));

		while (Vector2.Distance (currentTile.gridLocation, endTileLocation) > 0.01f) 
		{
			// Examine each tile adjacent to the current tile, ignoring tiles in the provided blacklist.
			foreach (Vector2 neighborLocation in GetValidAdjacentTiles(scene, currentTile.gridLocation, tileBlacklist)) 
			{
				NavTile neighborTile = new NavTile
				{
					gridLocation = neighborLocation.ToVector2Int(),
					source = currentTile
				};

				neighborTile.travelCost = neighborTile.source.travelCost + 1;

				if (neighborLocation != endTileLocation) // Ignore extra travel cost for the destination tile.
					neighborTile.travelCost += GetExtraTraversalCost(scene, neighborTile.gridLocation);

				neighborTile.totalCost = neighborTile.travelCost + CalculateHeuristic(neighborLocation, scenePosEnd);

				bool alreadySearched = false;
				bool alreadyInQueue = false;
				NavTile tileInQueue = null;


				// Check if this tile is already in the finished list.
				foreach (NavTile finishedTile in finishedTiles) 
				{
					if (finishedTile.gridLocation == neighborTile.gridLocation)
					{
						alreadySearched = true;

						// If this tile in the finished list has a worse path, remove it.
						// (This shouldn't normally happen, but it might if the heuristic isn't completely consistent).
						if (finishedTile.travelCost > neighborTile.travelCost)
						{
							finishedTiles.Remove(finishedTile);
							alreadySearched = false;
						}
						break;
					}
				}

				// Check if this tile is already in the queue.
				foreach (NavTile queuedTile in tileQueue)
				{
					if (queuedTile.gridLocation == neighborTile.gridLocation)
					{
						alreadyInQueue = true;
						tileInQueue = queuedTile;
						break;
					}
				}

				// If the tile we're checking is already in the queue, see if we have a better path to it; if so, replace it.
				if (alreadyInQueue && tileInQueue.travelCost > neighborTile.travelCost) 
				{
					tileInQueue.travelCost = neighborTile.travelCost;
					tileInQueue.totalCost = neighborTile.totalCost;
					tileInQueue.source = neighborTile.source;
				}

				// If this tile now isn't in either list, add it to the queue.
				if (!alreadySearched && !alreadyInQueue) {
					tileQueue.Add (neighborTile);
				}
			}

			if (tileQueue.Count == 0)
			{
				Debug.Log("Pathfinding failed; there are no tiles in the queue.");
				return null;
			}

			NavTile currentBestTile = null;

			// Find the lowest-cost tile in the queue
			foreach (NavTile tile in tileQueue) {
				if (currentBestTile == null || tile.totalCost < currentBestTile.totalCost) {
					currentBestTile = tile;
				}
			}

			if (GameConfig.DebugPathfinding)
			{
				// Color code by distance to target
				Color color = new Color(
					1 - Mathf.Clamp01(Mathf.Abs(currentBestTile.gridLocation.x - endTileLocation.x) / 60f),
					1 - Mathf.Clamp01(Mathf.Abs(currentBestTile.gridLocation.y - endTileLocation.y) / 60f),
					GetExtraTraversalCost(scene, currentBestTile.gridLocation) / 10f);

				// Draw a cross on this tile
				Debug.DrawLine(currentBestTile.gridLocation + new Vector2(0.5f, 0.5f) + Vector2.down * 0.3f, currentBestTile.gridLocation + new Vector2(0.5f, 0.5f) + Vector2.up * 0.3f, color, 3f, false);
				Debug.DrawLine(currentBestTile.gridLocation + new Vector2(0.5f, 0.5f) + Vector2.left * 0.3f, currentBestTile.gridLocation + new Vector2(0.5f, 0.5f) + Vector2.right * 0.3f, color, 3f, false);
			}

			tileCounter++;
			if (tileCounter > TILE_EXPLORATION_LIMIT) {
				Debug.Log ("Pathfinding failed; tile exploration limit reached.\n" +
					"Tried to navigate to " + scenePosEnd + " in " + scene);
				return null;
			}
			
			finishedTiles.Add (currentTile);
			currentTile = currentBestTile;
			tileQueue.Remove (currentTile);
		}

		// Navigate through the finished tiles to build the path
		List<NavTile> tilePath = new List<NavTile>();
		tilePath.Add (currentTile);
		path.Add (currentTile.gridLocation);
		while (tilePath[tilePath.Count - 1].gridLocation != startTileLocation) {
			tilePath.Add (tilePath [tilePath.Count - 1].source);
			path.Insert (0, tilePath [tilePath.Count - 1].gridLocation);
		}
		// Remove the starting location of the path, since we're already there
		path.RemoveAt(0);
		return path;
	}
	
	// Returns a list of locations of valid navigable tiles bordering the given tile
	// (If you ever want to implement diagonal walking, this is the method to change).
	public static List<Vector2Int> GetValidAdjacentTiles(string scene, Vector2 scenePosition, ISet<Vector2> tileBlacklist)
	{
		List<Vector2Int> tiles = new List<Vector2Int> (4);
		for (int y = 1; y >= -1; y--)
		{
			for (int x = -1; x <= 1; x++)
			{
				// Only pick a tile as valid if it is on either the same x-pos or y-pos as us (but not both)
				if (x != 0 ^ y != 0)
				{
					Vector2Int tilePos = new Vector2Int((int)scenePosition.x + x, (int)scenePosition.y + y);
					MapUnit mapUnit = WorldMapManager.GetMapObjectAtPoint(tilePos, scene);
					if (mapUnit != null &&
						!mapUnit.groundMaterial.isImpassable &&
						!(tileBlacklist != null && tileBlacklist.Contains(tilePos)) &&
						(mapUnit.entityId == null ||
						ContentLibrary.Instance.Entities.Get(WorldMapManager.GetMapObjectAtPoint(tilePos, scene).entityId).canBeWalkedThrough)) 
					{
						tiles.Add (tilePos);
					}
				}
			}
		}
		return tiles;
	}

	private static float GetExtraTraversalCost (string scene, Vector2Int tilePos) 
	{
		MapUnit mapUnit = WorldMapManager.GetMapObjectAtPoint(tilePos, scene);

		if (mapUnit != null &&
			mapUnit.entityId != null &&
			ContentLibrary.Instance.Entities.Get(mapUnit.entityId).canBeWalkedThrough) 
		{
			return ContentLibrary.Instance.Entities.Get (mapUnit.entityId).extraTraversalCost + mapUnit.groundMaterial.extraTraversalCost; 
		}

		return 0f;
	}

	public static Vector2 FindRandomNearbyPathTile(Vector2 startLocation, int numberOfStepsToTake, string scene) {
		Vector2 startTilePos = new Vector2 (Mathf.Floor (startLocation.x), Mathf.Floor (startLocation.y));
		List<Vector2> usedTiles = new List<Vector2> ();
		Vector2 currentPos = startTilePos;
		for (int i = 0; i < numberOfStepsToTake; i++) {
			usedTiles.Add (currentPos);
			List<Vector2Int> nearbyTiles = GetValidAdjacentTiles (scene, currentPos, null);
			foreach (Vector2Int pos in nearbyTiles.ToArray()) {
				if (usedTiles.Contains (pos))
					nearbyTiles.Remove (pos);
			}
			if (nearbyTiles.Count != 0) {
				currentPos = nearbyTiles [Random.Range (0, nearbyTiles.Count)];
			}
			else {
				break;
			}
		}
		return currentPos;
	}

	public static Direction GetDirectionToLocation(Vector2 startLocation, Vector2 endLocation) {
		float xDist = endLocation.x - startLocation.x;
		float yDist = endLocation.y - startLocation.y;

		if (startLocation == endLocation)
			return Direction.Down;

		if (xDist >= yDist) {
			if (xDist > 0)
				return Direction.Right;
			else
				return Direction.Left;
		}

		if (yDist > 0)
			return Direction.Up;
		else
			return Direction.Down;
	}
}
