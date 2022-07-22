using ActorComponents;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GUI
{
    public class HealthbarManager : MonoBehaviour
    {
        [SerializeField] private Slider healthSlider;
        [SerializeField] private TextMeshProUGUI healthText;

        // Update is called once per frame
        void Update()
        {
            ActorRegistry.ActorInfo playerInfo = ActorRegistry.Get(PlayerController.PlayerActorId);
            if (playerInfo == null) return;
            
            ActorHealth playerHealth = playerInfo.data.Get<ActorHealth>();

            healthSlider.value = playerHealth.CurrentHealth /
                playerHealth.MaxHealth;

            healthText.text =
                $"{Mathf.FloorToInt(playerHealth.CurrentHealth + 0.001f)}/ " +
                $"{Mathf.FloorToInt(playerHealth.MaxHealth + 0.001f)}";
        }
    }
}
