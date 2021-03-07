using System.Collections.Generic;
using ContentLibraries;
using UnityEngine;
using UnityEngine.Tilemaps;

// Finds paths between places.
// Note that this class relies on positions relative to scene origins, not absolute positions.
public class Pathfinder : MonoBehaviour {
	// TODO make the functions in this class take tilemap objects instead of scene names, to maximize independence

	// The maximum number of tiles that will be explored before pathfinding returns a failure.
	private const int TILE_EXPLORATION_LIMIT = 2500;
	// The travel distance across a tile diagonal
	private const float DIAGONAL_TRAVEL_COST = 1.41f;

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
		float D = 1; // The cost of moving non-diagonally
		float D2 = DIAGONAL_TRAVEL_COST; // The cost of moving diagonally

		float dx = Mathf.Abs(tilePos.x - destinationPos.x); // horizontal distance
		float dy = Mathf.Abs(tilePos.y - destinationPos.y); // vertical distance

		// 8-way movement heuristic.
		float result = D * (dx + dy) + (D2 - 2 * D) * Mathf.Min(dx, dy);

		// Scale up the result slightly so that low distance-to-target is weighted more heavily than low travel cost.
		// This will reduce the number of equally-high-cost paths.
		result *= 1.001f;
		return result;
	}



	// Uses A* algorithm to find shortest path to the desired tile, avoiding tiles in the given set.
	// Returns null if no path exists or we hit the tile exploration limit while searching.
	public static List<Vector2> FindPath (Vector2 scenePosStart, Vector2 scenePosEnd, string scene, ISet<Vector2> tileBlacklist) {

		int tileCounter = 0;

		Vector2Int startTileLocation = new Vector2Int (Mathf.FloorToInt (scenePosStart.x), Mathf.FloorToInt (scenePosStart.y));
		Vector2Int endTileLocation = new Vector2Int (Mathf.FloorToInt (scenePosEnd.x), Mathf.FloorToInt (scenePosEnd.y));

		if (RegionMapManager.GetMapObjectAtPoint(startTileLocation, scene) == null) {
			Debug.Log ("Attempted start location: " + startTileLocation.x + ", " + startTileLocation.y);
			Debug.LogError ("Attempted to start navigation at a point not defined in the world map");
			return null;
		}
		else if (RegionMapManager.GetMapObjectAtPoint(startTileLocation, scene).groundMaterial == null) {
			Debug.LogWarning ("No ground material found at navigation start point");
		}

		if (!TileIsWalkable(endTileLocation, scene))
		{
			Debug.LogWarning("Tried to navigate to an impassable or nonexistent tile!");
			return null;
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

				bool isDiagonal = currentTile.gridLocation.x != neighborTile.gridLocation.x && currentTile.gridLocation.y != neighborTile.gridLocation.y;
				neighborTile.travelCost = neighborTile.source.travelCost;
				neighborTile.travelCost += isDiagonal ? DIAGONAL_TRAVEL_COST : 1;

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
	public static HashSet<Vector2Int> GetValidAdjacentTiles(string scene, Vector2 scenePosition, ISet<Vector2> tileBlacklist)
	{
		HashSet<Vector2Int> tiles = new HashSet<Vector2Int> ();
		for (int y = 1; y >= -1; y--)
		{
			for (int x = -1; x <= 1; x++)
			{
				// Only pick a tile as valid if it is on either the same x-pos or y-pos as us (but not both)
				if (x != 0 ^ y != 0)
				{
					Vector2Int tilePos = new Vector2Int((int)scenePosition.x + x, (int)scenePosition.y + y);
					if (tileBlacklist != null && tileBlacklist.Contains(tilePos)) continue;
					if (TileIsWalkable(tilePos, scene)) tiles.Add(tilePos);
				}
			}
		}

		HashSet<Vector2Int> diagonals = new HashSet<Vector2Int>();
		
		for (int y = 1; y >= -1; y -= 2)
		{
			for (int x = -1; x <= 1; x += 2)
			{
				Vector2Int tilePos = new Vector2Int((int)scenePosition.x + x, (int)scenePosition.y + y);

				// Add diagonals only if both adjacent non-diagonals were accepted.

				bool containsSameX = false;
				bool containsSameY = false;

				foreach (Vector2Int vec in tiles)
				{
					if (vec.x == tilePos.x) containsSameX = true;
					if (vec.y == tilePos.y) containsSameY = true;
				}

				if (containsSameX && containsSameY && TileIsWalkable(tilePos, scene))
				{
					diagonals.Add(tilePos);
				}
			}
		}
		tiles.UnionWith(diagonals);
		return tiles;
	}

	private static bool TileIsWalkable(Vector2Int scenePos, string scene)
	{
		MapUnit mapUnit = RegionMapManager.GetMapObjectAtPoint(scenePos, scene);
		return (mapUnit != null &&
			!mapUnit.groundMaterial.isImpassable &&
			(mapUnit.entityId == null ||
			ContentLibrary.Instance.Entities.Get(mapUnit.entityId).canBeWalkedThrough));
	}

	// Returns the additional travel cost for the given tile based on ground type, ground cover, and entities.
	private static float GetExtraTraversalCost (string scene, Vector2Int tilePos) 
	{
		float result = 0;
		MapUnit mapUnit = RegionMapManager.GetMapObjectAtPoint(tilePos, scene);
		if (mapUnit == null) return result;

		if (mapUnit.entityId != null) 
		{
			result += ContentLibrary.Instance.Entities.Get (mapUnit.entityId).extraTraversalCost; 
		}

		if (mapUnit.groundMaterial != null)
		{
			result += mapUnit.groundMaterial.extraTraversalCost;
		}
		if (mapUnit.groundCover != null)
		{
			result += mapUnit.groundCover.extraTraversalCost;
		}

		return result;
	}

	public static Vector2 FindRandomNearbyPathTile(Vector2 startLocation, int numberOfStepsToTake, string scene) {

		Vector2 startTilePos = new Vector2 (Mathf.Floor (startLocation.x), Mathf.Floor (startLocation.y));
		List<Vector2> usedTiles = new List<Vector2> ();
		Vector2 currentPos = startTilePos;
		for (int i = 0; i < numberOfStepsToTake; i++) 
		{
			usedTiles.Add (currentPos);
			HashSet<Vector2Int> nearbyTiles = GetValidAdjacentTiles (scene, currentPos, null);
			nearbyTiles.RemoveWhere(val => usedTiles.Contains(val));

			if (nearbyTiles.Count != 0) 
			{
				currentPos = nearbyTiles.PickRandom();
			}
			else break;
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
