using System.Collections;
using System.Collections.Generic;
using Popcron.Console;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameConfig : MonoBehaviour
{
    private const string MusicVolumeKey = "MUSIC_VOLUME";

    private const float MusicVolumeDefault = 0.8f;

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
