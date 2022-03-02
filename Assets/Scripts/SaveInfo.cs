using IntroSequences;
using UnityEngine;

/// Stores a bunch of information about a save file.
public static class SaveInfo 
{
    public static string SaveFileId { get; set; }
	public static WorldSave SaveToLoad { get; set; }
	public static ActorData NewlyCreatedPlayer { get; set; }
	public static string WorldName { get; set; }
	public static Vector2Int RegionSize { get; set; }
	public static IIntroSequence IntroSequence { get; set; }
}
