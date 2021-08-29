using UnityEngine;

namespace Items
{
    /// An item which regenerates health when eaten.
    [CreateAssetMenu(menuName = "Items/Food")]
    public class Food : ItemData, IEdible
    {
        [SerializeField] private float healthRegen;

        public void ApplyEffects(ActorData actorData)
        {
            actorData.Health.AdjustHealth(healthRegen);
        }
    }
}
