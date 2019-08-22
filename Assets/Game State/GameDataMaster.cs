using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameDataMaster {

    public static string SaveFileId { get; set; }
    public static string WorldName { get; set; }

    public static PlayerCharData LoadedPlayerChar { get; private set; }

    // Must be set before the main scene is loaded
    public static WorldMap LoadedWorldMap { get; private set; }

    public static void SetLoadedPlayerChar (PlayerCharData character)
	{
		LoadedPlayerChar = character;
	} 
    public static void SetWorldMap (WorldMap map)
    {
        LoadedWorldMap = map;
    }
}
