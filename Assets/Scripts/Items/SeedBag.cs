using System;
using System.Collections.Generic;
using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName = "NewItem", menuName = "Items/SeedBag", order = 1)]
    public class SeedBag : ItemData, IPloppable
    {
        private const string UsesRemainingModifier = "uses"; // Stores the number of uses remaining
        private const string EmptyBagText = "empty"; // The text added to the item's name if it's empty

        [SerializeField] private string plantEntityId = null;
        [SerializeField] private int totalUses = 20; // How many seedlings a single bag can plant

        public override string GetItemName(IDictionary<string, string> modifiers)
        {
            if (modifiers.TryGetValue(UsesRemainingModifier, out string usesModifier))
            {
                int uses = Int32.Parse(usesModifier);
                if (uses == 0)
                {
                    return base.GetItemName(modifiers) + " (" + EmptyBagText + ")";
                }
                if (uses < this.totalUses)
                {
                    float percentUsed = (float)uses / this.totalUses;
                    string percentString = percentUsed.ToString("0%");
                    return base.GetItemName(modifiers) + " (" + percentString + ")";
                }
            }
            return base.GetItemName(modifiers);
        }

        bool IPloppable.VisibleTileSelector(ItemStack instance)
        {
            if (instance.GetModifiers().TryGetValue(UsesRemainingModifier, out string usesModifier))
            {
                int uses = Int32.Parse(usesModifier);
                return uses > 0;
            }
            return true;
        }

        void IPloppable.Use(TileLocation target, ItemStack instance)
        {
            EntityData entity = ContentLibrary.Instance.Entities.Get(plantEntityId);
            if (entity == null)
            {
                Debug.LogError("Entity " + plantEntityId + " not found!");
                return;
            }
            GroundMaterial ground = RegionMapManager.GetGroundMaterialtAtPoint(target.Position.ToVector2Int(), target.Scene);
            GroundMaterial groundCover = RegionMapManager.GetGroundCoverAtPoint(target.Position.ToVector2Int(), target.Scene);

            if (ground == null)
            {
                return;
            }
            if (groundCover == null && ground.isFarmland || groundCover != null && groundCover.isFarmland)
            {
                string currentEntity = RegionMapManager.GetEntityIdAtPoint(target.Position.ToVector2Int(), target.Scene);
                if (currentEntity != null) return;

                int remainingUses = totalUses;
                if (instance.GetModifiers().TryGetValue(UsesRemainingModifier, out string value))
                {
                    remainingUses = Int32.Parse(value);
                }
                if (remainingUses > 0)
                {
                    if (RegionMapManager.AttemptPlaceEntityAtPoint(entity, target.Position.ToVector2Int(), target.Scene))
                    {
                        remainingUses--;
                        instance.id = ItemIdParser.SetModifier(instance.id, UsesRemainingModifier, (remainingUses).ToString());
                    }
                }
                else
                {
                    // Empty bag. Destroy the item somehow?
                }
            }
        }
    }
}