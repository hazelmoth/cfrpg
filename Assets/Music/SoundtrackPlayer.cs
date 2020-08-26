using System.Collections.Generic;
using UnityEngine;

public class SoundtrackPlayer : MonoBehaviour
{
	[SerializeField] private List<AudioClip> soundtrack;
	[SerializeField] private float gapDuration = 600f;

	private AudioSource audioSource;
	private const float maxVol = 0.5f;

	private float timeLastSongEnded;
	private int lastSongIndex = -1;

    // Start is called before the first frame update
    private void Start()
    {
	    audioSource = gameObject.AddComponent<AudioSource> ();
		audioSource.spatialBlend = 0f;
		audioSource.loop = false;
		audioSource.volume = maxVol * GameConfig.MusicVolume;
    }

    // Update is called once per frame
    private void Update()
    {
	    if (audioSource == null)
	    {
		    return;
	    }

		audioSource.volume = maxVol * GameConfig.MusicVolume;

		if (audioSource.isPlaying)
		{
			timeLastSongEnded = Time.unscaledTime;
		}
		else if (Time.unscaledTime - timeLastSongEnded > gapDuration)
		{
			lastSongIndex = PickNextSong();
			audioSource.clip = soundtrack[lastSongIndex];
			audioSource.Play();
		}
	}

	private int PickNextSong()
	{
		if (soundtrack.Count == 1)
		{
			return 0;
		}
		if (lastSongIndex == -1)
		{
			return Random.Range(0, soundtrack.Count - 1);
		}

		int index = Random.Range(0, soundtrack.Count - 2);

		if (index >= lastSongIndex)
		{
			index++;
		}

		return index;
	}
}
