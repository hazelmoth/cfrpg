using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Just calls a tile refresh on all path tiles when the game starts (so they're hidden)
// Nevermind does nothing because path tiles are obsolete, delete this class please
public class PathTileRefresher : MonoBehaviour {

	[SerializeField] bool refreshPathTilesOnPlay = true;

	void Start () {
		InitialSceneLoader.OnInitialScenesLoaded += RefreshTilesIfEnabled;
		RefreshTilesIfEnabled ();
	}
	public void RefreshTilesIfEnabled() {

	}
}
