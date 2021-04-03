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

        healthSlider.value = playerInfo.data.PhysicalCondition.CurrentHealth /
                             playerInfo.data.PhysicalCondition.MaxHealth;

        healthText.text =
            $"{Mathf.FloorToInt(playerInfo.data.PhysicalCondition.CurrentHealth + 0.001f)}/ " +
            $"{Mathf.FloorToInt(playerInfo.data.PhysicalCondition.MaxHealth + 0.001f)}";
    }
}
