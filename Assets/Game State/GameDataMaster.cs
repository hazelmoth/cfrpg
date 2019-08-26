using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameDataMaster {

    public static string SaveFileId { get; set; }
	public static WorldSave SaveToLoad { get; set; }
	public static SavedPlayerChar PlayerToLoad { get; set; }
	public static string WorldName { get; set; }
}
