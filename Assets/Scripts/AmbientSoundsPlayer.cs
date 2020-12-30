using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientSoundsPlayer : MonoBehaviour
{
    private AudioSource audioSource;
    private List<float> waveMultipliers = new List<float> { 8.1f, 1f, 0.55f, 0.24f }; // These match the waves in the grass sway shader
    private float speed = 1;
    private float volumeMultiplier = 1;
    private Actor player;
    private float maxVolume = 0.3f;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (!audioSource.isPlaying) audioSource.Play();
        
        volumeMultiplier = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
            player = ActorRegistry.Get(PlayerController.PlayerActorId)?.actorObject;
        if (player == null) return;

        Vector2 playerPos = Vector2.zero;
        if (player != null)
        {
            playerPos = player.transform.position;
        }

        float oldMultiplier = volumeMultiplier;
        volumeMultiplier = 1;
        foreach (float wave in waveMultipliers)
        {
            volumeMultiplier *= Mathf.Sin(wave * (Time.time * speed + playerPos.x + playerPos.y));
        }
        volumeMultiplier = Mathf.Abs(volumeMultiplier);
        volumeMultiplier = Mathf.Lerp(oldMultiplier, 2 * volumeMultiplier, 1 * Time.deltaTime); // interpolate values over time
        volumeMultiplier = Mathf.Clamp01(volumeMultiplier);

        audioSource.volume = maxVolume * volumeMultiplier;
    }
}
