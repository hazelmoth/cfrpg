using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameDataMaster {

	public static PlayerCharData LoadedPlayerChar { get; private set; }

	public static void SetLoadedPlayerChar (PlayerCharData character)
	{
		LoadedPlayerChar = character;
	} 
}
