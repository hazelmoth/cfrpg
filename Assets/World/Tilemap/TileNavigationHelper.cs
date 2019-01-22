using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileNavigationHelper : MonoBehaviour {

	class NavTile {
		public Vector2 gridLocation;
		public float travelCost;
		public float totalCost;
		public NavTile source;

		public NavTile () {}
		public NavTile (Vector2 gridLocation, NavTile source, float travelCost, float totalCost) {
			this.gridLocation = gridLocation;
			this.source = source;
			this.totalCost = totalCost;
		}
	}

	// Uses A* algorithm to find shortest path to the desired tile
	public static List<Vector2> FindPath (Vector2 startPos, Vector2 endPos) {

		int tileCounter = 0;

		Vector2 startTileLocation = new Vector2 (Mathf.Floor (startPos.x), Mathf.Floor (startPos.y));
		Vector2 endTileLocation = new Vector2 (Mathf.Floor (endPos.x), Mathf.Floor (endPos.y));
		TileBase startTile = TilemapInterface.GetPathTileAtWorldPosition (startTileLocation.x, startTileLocation.y);
		TileBase endTile = TilemapInterface.GetPathTileAtWorldPosition (endTileLocation.x, endTileLocation.y);
		if (startTile == null) {
			throw new System.Exception ("Tried to start navigation from a tile that isn't a path tile!");
			// TODO
			// Oh no, the start position is not on a path!
			// We'll have to find one somehow!
		}
		if (endTile == null) {
			throw new System.Exception ("Tried to navigate to a tile that isn't a path tile!");
			// TODO
			// Figure out how to get from a path to the destination.
		}

		List<NavTile> tileQueue = new List<NavTile> ();
		List<NavTile> finishedTiles = new List<NavTile> ();
		List<Vector2> path = new List<Vector2> ();
		NavTile currentTile = new NavTile(startTileLocation, null, 0, Vector2.Distance(startTileLocation, endTileLocation));

		while (Vector2.Distance (currentTile.gridLocation, endTileLocation) > 0) {
			foreach (Vector2 location in GetValidAdjacentTiles(TilemapInterface.GetPathTilemap(), currentTile.gridLocation)) {

				NavTile navTile = new NavTile ();
				navTile.gridLocation = location;
				navTile.source = currentTile;
				navTile.travelCost = navTile.source.travelCost + 1;
				navTile.totalCost = navTile.travelCost + Vector2.Distance (location, endPos);

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
			// TODO: handle a situation with no valid path tiles
			if (tileQueue.Count == 0)
				throw new UnityException("Can't seem to find a navigation path; there are no tiles in the queue!");

			NavTile currentBestTile = null;
			// Find the lowest-cost tile in the queue
			foreach (NavTile tile in tileQueue) {
				if (currentBestTile == null || tile.totalCost < currentBestTile.totalCost) {
					currentBestTile = tile;
				}
			}

			tileCounter++;
			if (tileCounter > 400)
				throw new UnityException("Hold on, we've already searched 400 tiles. Something must be wrong here.");
				
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

	// Returns a list of locations of valid path tiles bordering the given tile
	// (If you ever want to implement diagonal walking, this is the function to change)
	private static List<Vector2> GetValidAdjacentTiles(Tilemap tilemap, Vector2 position)
	{
		List<Vector2> tiles = new List<Vector2> (4);
		int index = 0;
		for (int y = 1; y >= -1; y--)
		{
			for (int x = -1; x <= 1; x++)
			{
				if (x != 0 ^ y != 0)
				{
					Vector2 tilePosition = new Vector2(position.x + x, position.y + y);
					if (TilemapInterface.GetPathTileAtWorldPosition(tilePosition.x, tilePosition.y) != null) {
						tiles.Add (tilePosition);
					}
				}
			}
		}
		return tiles;
	}

	public static Vector2 FindRandomNearbyPathTile(Vector2 startLocation, int numberOfStepsToTake) {
		Vector2 startTilePos = new Vector2 (Mathf.Floor (startLocation.x), Mathf.Floor (startLocation.y));
		List<Vector2> usedTiles = new List<Vector2> ();
		Vector2 currentPos = startTilePos;
		for (int i = 0; i < numberOfStepsToTake; i++) {
			usedTiles.Add (currentPos);
			List<Vector2> nearbyTiles = GetValidAdjacentTiles (TilemapInterface.GetPathTilemap (), currentPos);
			foreach (Vector2 pos in nearbyTiles.ToArray()) {
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
}
