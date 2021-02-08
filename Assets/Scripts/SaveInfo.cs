using UnityEngine;

public static class SaveInfo 
{
    public static string SaveFileId { get; set; }
	public static WorldSave SaveToLoad { get; set; }
	public static ActorData NewlyCreatedPlayer { get; set; }
	public static string WorldName { get; set; }
	public static Vector2Int RegionSize { get; set; }
}
