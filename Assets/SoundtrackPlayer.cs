using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundtrackPlayer : MonoBehaviour
{
	[SerializeField] AudioClip soundtrack;
    // Start is called before the first frame update
    void Start()
    {
		AudioSource audioSource = gameObject.AddComponent<AudioSource> ();
		audioSource.clip = soundtrack;
		audioSource.spatialBlend = 0f;
		audioSource.loop = true;
		audioSource.volume = 0.4f;
		audioSource.Play ();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
