using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthbarManager : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI healthText;

    // Update is called once per frame
    void Update()
    {
        ActorRegistry.ActorInfo playerInfo = ActorRegistry.Get(PlayerController.PlayerActorId);
        if (playerInfo == null) return;

        healthSlider.value = playerInfo.data.Health.CurrentHealth /
                             playerInfo.data.Health.MaxHealth;

        healthText.text =
            $"{Mathf.FloorToInt(playerInfo.data.Health.CurrentHealth + 0.001f)}/ " +
            $"{Mathf.FloorToInt(playerInfo.data.Health.MaxHealth + 0.001f)}";
    }
}
