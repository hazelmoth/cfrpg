using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameDataMaster {

	public static PlayerCharData LoadedPlayerChar { get; private set; }

    // Must be set before the main scene is loaded
    public static WorldMap CurrentWorldMap { get; private set; }

    public static void SetLoadedPlayerChar (PlayerCharData character)
	{
		LoadedPlayerChar = character;
	} 
    public static void SetWorldMap (WorldMap map)
    {
        CurrentWorldMap = map;
    }
}
