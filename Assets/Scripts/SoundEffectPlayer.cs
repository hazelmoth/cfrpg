using System;
using System.Collections;
using UnityEngine;

public static class SoundEffectPlayer
{
	/// Plays the provided clip at the location of the given parent.
	/// Uses a new AudioSource which is a child of the provided parent object, so the
	/// sound will follow that object.
	public static void PlaySound(AudioClip clip, GameObject parent, float volume)
    {
	    GameObject audioObject = new GameObject("Audio Source");
	    audioObject.transform.SetParent(parent.transform);
        audioObject.transform.localPosition = Vector3.zero;

        AudioSource src = audioObject.AddComponent<AudioSource>();
        src.spatialBlend = 1f;
        src.minDistance = 1f;
        src.volume = volume;
        src.PlayOneShot(clip, 1f);
        GlobalCoroutineObject.Instance.StartCoroutine(WaitForSoundCoroutine(src, () => GameObject.Destroy(src.gameObject)));
    }

    private static IEnumerator WaitForSoundCoroutine(AudioSource source, Action callback)
    {
	    if (source.gameObject == null)
	    {
		    // Destroyed prematurely.
		    callback?.Invoke();
		    yield break;
	    }
	    while (source.isPlaying)
	    {
		    yield return null;
	    }
        callback?.Invoke();
    }
}
