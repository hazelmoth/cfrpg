using System;
using System.Collections.Generic;
using Items.Modular.Components;
using UnityEngine;

namespace Items.Modular
{
    // [CreateAssetMenu(menuName="Items/HealingEdible")]
    public class HealingEdible : ModularItem
    {
        [SerializeField] private Components.HealingEdible healing;

        private void OnValidate()
        {
            components = new List<ItemComponent>
            {
                healing
            };
        }

    }
}
