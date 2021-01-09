using Popcron.Console;
using UnityEngine;

public class GameConfig : MonoBehaviour
{
	private const string GodModeKey = "GOD_MODE";
    private const string MusicVolumeKey = "MUSIC_VOLUME";

    private const float MusicVolumeDefault = 0.8f;

	// Allows infinite construction for free
	[Command("GodMode")]
	public static bool GodMode
	{
		get
		{
			if (PlayerPrefs.HasKey(GodModeKey))
			{
				return PlayerPrefs.GetInt(GodModeKey) == 1;
			}
			return false;
		}
		set
		{
			PlayerPrefs.SetInt(GodModeKey, value ? 1 : 0);
			PlayerPrefs.Save();
		}
	}

	[Command("MusicVolume")]
    public static float MusicVolume
    {
	    set
	    {
            PlayerPrefs.SetFloat(MusicVolumeKey, value);
            PlayerPrefs.Save();
	    }
	    get
	    {
		    if (PlayerPrefs.HasKey(MusicVolumeKey))
		    {
			    return PlayerPrefs.GetFloat(MusicVolumeKey);
		    }
		    else
		    {
			    return MusicVolumeDefault;
		    }
	    }
    }
}
