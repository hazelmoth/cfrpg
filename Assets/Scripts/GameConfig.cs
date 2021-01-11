using Popcron.Console;
using System;
using UnityEngine;

public class GameConfig : MonoBehaviour
{
	private const string DebugPathfindingKey = "DEBUG_PATHFINDING";
	private const string GodModeKey = "GOD_MODE";
    private const string MusicVolumeKey = "MUSIC_VOLUME";

    private const float MusicVolumeDefault = 0.8f;

	[Command("DebugPathfinding")]
	public static bool DebugPathfinding
	{
		get => GetBool(DebugPathfindingKey, false);
		set => SetBool(DebugPathfindingKey, value);
	}

	// Allows infinite construction for free
	[Command("GodMode")]
	public static bool GodMode
	{
		get => GetBool(GodModeKey, false);
		set => SetBool(GodModeKey, value);
	}

	[Command("MusicVolume")]
    public static float MusicVolume
	{
		get => GetFloat(MusicVolumeKey, MusicVolumeDefault);
		set => SetFloat(MusicVolumeKey, value);
	}


	// ================= Wrapper methods for PlayerPrefs access/saving ================

	private static float GetFloat(string key, float defaultValue)
	{
		return PlayerPrefs.GetFloat(key, defaultValue);
	}
	private static void SetFloat(string key, float value)
	{
		PlayerPrefs.SetFloat(key, value);
		PlayerPrefs.Save();
	}

	// Booleans are stored as integers in PlayerPrefs
	private static bool GetBool(string key, bool defaultValue)
	{
		int defaultResult = defaultValue ? 1 : 0;
		return PlayerPrefs.GetInt(key, defaultResult) == 1;
	}
	private static void SetBool(string key, bool value)
	{
		PlayerPrefs.SetInt(key, value ? 1 : 0);
		PlayerPrefs.Save();
	}
}
