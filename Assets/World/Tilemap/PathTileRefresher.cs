using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Just calls a tile refresh on all path tiles when the game starts (so they're hidden)
public class PathTileRefresher : MonoBehaviour {

	[SerializeField] bool refreshPathTilesOnPlay = true;

	void Start () {
		SceneLoader.OnScenesLoaded += RefreshTilesIfEnabled;
		RefreshTilesIfEnabled ();
	}
	public void RefreshTilesIfEnabled() {
		if (refreshPathTilesOnPlay)
			TilemapInterface.RefreshAllPathTileSprites ();
	}
}
