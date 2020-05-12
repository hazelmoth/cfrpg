using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundtrackPlayer : MonoBehaviour
{
	[SerializeField] private AudioClip soundtrack;

	private AudioSource audioSource;
	private const float maxVol = 0.5f;

    // Start is called before the first frame update
    private void Start()
    {
	    audioSource = gameObject.AddComponent<AudioSource> ();
		audioSource.clip = soundtrack;
		audioSource.spatialBlend = 0f;
		audioSource.loop = true;
		audioSource.volume = maxVol * GameConfig.MusicVolume;
		audioSource.Play ();
    }

    // Update is called once per frame
    private void Update()
    {
	    if (audioSource == null)
	    {
		    return;
	    }
		audioSource.volume = maxVol * GameConfig.MusicVolume;
	}
}
