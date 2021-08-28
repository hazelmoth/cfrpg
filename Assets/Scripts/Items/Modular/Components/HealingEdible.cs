using System;
using MyBox;
using UnityEngine;

namespace Items.Modular.Components
{
    [Serializable]
    public class HealingEdible : ItemComponent
    {
        [SerializeField] private int healthRegen;

        public HealingEdible(int healthRegen)
        {
            this.healthRegen = healthRegen;
        }

        public int HealthRegen => healthRegen;
    }
}
