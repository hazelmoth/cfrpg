using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepSoundPlayer : MonoBehaviour
{
	private static FootstepSoundPlayer instance;
	[SerializeField] private List<AudioClip> footsteps;

    // Start is called before the first frame update
    void Start()
    {
	    instance = this;
    }

    public static void PlayRandomFootstep(GameObject audioParent)
    {
        SoundEffectPlayer.PlaySound(instance.footsteps.PickRandom(), audioParent);
    }
}
