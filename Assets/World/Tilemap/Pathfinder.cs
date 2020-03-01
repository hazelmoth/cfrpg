using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Finds paths between places.
// Note that this class relies on positions relative to scene origins, not absolute positions.
public class Pathfinder : MonoBehaviour {
	// TODO make the functions in this class take tilemap objects instead of scene names, to maximize independence

	// The maximum number of tiles that will be explored before pathfinding returns a failure.
	private const int TILE_EXPLORATION_LIMIT = 1000;

	class NavTile {
		public Vector2Int gridLocation;
		public float travelCost;
		public float tileBonusCost; // An extra cost for tiles that are less desirable to walk through
		public float totalCost;
		public NavTile source;

		public NavTile () {}
		public NavTile (Vector2Int gridLocation, NavTile source, float travelCost, float totalCost) {
			this.gridLocation = gridLocation;
			this.source = source;
			this.totalCost = totalCost;
		}
	}

	// Uses A* algorithm to find shortest path to the desired tile, avoiding tiles in the given set.
	// Returns null if no path exists or we hit tile exploration limit while searching.
	public static List<Vector2> FindPath (Vector2 relativeStartPos, Vector2 relativeEndPos, string scene, ISet<Vector2> tileBlacklist) {

		int tileCounter = 0;

		Vector2Int startTileLocation = new Vector2Int (Mathf.FloorToInt (relativeStartPos.x), Mathf.FloorToInt (relativeStartPos.y));
		Vector2Int endTileLocation = new Vector2Int (Mathf.FloorToInt (relativeEndPos.x), Mathf.FloorToInt (relativeEndPos.y));

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
		NavTile currentTile = new NavTile(startTileLocation, null, 0, Vector2.Distance(startTileLocation, endTileLocation));

		while (Vector2.Distance (currentTile.gridLocation, endTileLocation) > 0) {
			foreach (Vector2 location in GetValidAdjacentTiles(scene, currentTile.gridLocation, tileBlacklist)) {

				NavTile navTile = new NavTile ();
				navTile.gridLocation = new Vector2Int ((int)location.x, (int)location.y);
				navTile.source = currentTile;
				if (location != endTileLocation) { // Don't add extra travel cost to the destination tile
					navTile.tileBonusCost = CheckExtraTravelCostAtPos (scene, navTile.gridLocation);
				}
				navTile.travelCost = navTile.source.travelCost + 1;
				navTile.totalCost = navTile.travelCost + navTile.tileBonusCost + Vector2.Distance (location, relativeEndPos);

				bool alreadySearched = false;
				bool alreadyInQueue = false;
				NavTile tileInQueue = null;

				// Don't add this tile to the queue if we've already expanded it
				foreach (NavTile finishedTile in finishedTiles) {
					if (finishedTile.gridLocation == navTile.gridLocation) {
						alreadySearched = true;
						break;
					}
				}
				foreach (NavTile queuedTile in tileQueue) {
					if (queuedTile.gridLocation == navTile.gridLocation) {
						alreadyInQueue = true;
						tileInQueue = queuedTile;
						break;
					}
				}
				// If the tile we're checking is already in the queue, see if we have a better path to it
				if (alreadyInQueue && tileInQueue.travelCost > navTile.travelCost + 1) {
					tileInQueue.totalCost -= tileInQueue.travelCost;
					tileInQueue.travelCost = navTile.travelCost + 1;
					tileInQueue.totalCost += tileInQueue.travelCost;
					tileInQueue.source = navTile;
				}

				if (!alreadySearched && !alreadyInQueue) {
					tileQueue.Add (navTile);
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

			tileCounter++;
			if (tileCounter > TILE_EXPLORATION_LIMIT) {
				Debug.Log ("Pathfinding failed; tile exploration limit reached.");
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
	// (If you ever want to implement diagonal walking, this is the method to change)
	public static List<Vector2Int> GetValidAdjacentTiles(string scene, Vector2 position, ISet<Vector2> tileBlacklist)
	{
		List<Vector2Int> tiles = new List<Vector2Int> (4);
		for (int y = 1; y >= -1; y--)
		{
			for (int x = -1; x <= 1; x++)
			{
				// Only pick a tile as valid if it is on either the same x-pos or y-pos as us
				// (but not both)
				if (x != 0 ^ y != 0)
				{
					Vector2Int tilePos = new Vector2Int((int)position.x + x, (int)position.y + y);
					MapUnit mapUnit = WorldMapManager.GetMapObjectAtPoint(tilePos, scene);
					if (mapUnit != null &&
						!mapUnit.groundMaterial.isWater &&
						!(tileBlacklist != null && tileBlacklist.Contains(tilePos)) &&
						(mapUnit.entityId == null ||
						ContentLibrary.Instance.Entities.GetEntityFromID(WorldMapManager.GetMapObjectAtPoint(tilePos, scene).entityId).canBeWalkedThrough)) 
					{
						tiles.Add (tilePos);
					}
				}
			}
		}
		return tiles;
	}
	public static float CheckExtraTravelCostAtPos (string scene, Vector2Int tilePos) {
		if (WorldMapManager.GetMapObjectAtPoint(tilePos, scene) != null &&
			WorldMapManager.GetMapObjectAtPoint(tilePos, scene).entityId != null &&
			ContentLibrary.Instance.Entities.GetEntityFromID(WorldMapManager.GetMapObjectAtPoint(tilePos, scene).entityId).canBeWalkedThrough) 
		{
			return ContentLibrary.Instance.Entities.GetEntityFromID (WorldMapManager.GetMapObjectAtPoint (tilePos, scene).entityId).extraTraversalCost; 
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
