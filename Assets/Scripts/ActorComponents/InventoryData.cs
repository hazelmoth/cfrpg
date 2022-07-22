using System;

namespace ActorComponents
{
    [Serializable]
    public class InventoryData : IActorComponent
    {
        public ItemStack[] mainInvArray;
        public ItemStack[] hotbarArray;
        public ItemStack equippedHat;
        public ItemStack equippedShirt;
        public ItemStack equippedPants;

        public InventoryData()
        {
            mainInvArray = new ItemStack[ActorInventory.InventorySize];
            hotbarArray = new ItemStack[ActorInventory.HotbarSize];

            // Set these fields to null, since Unity initializes serializable classes as not null
            for (int i = 0; i < mainInvArray.Length; i++) mainInvArray[i] = null;
            for (int i = 0; i < hotbarArray.Length; i++) hotbarArray[i] = null;
            equippedHat = null;
            equippedShirt = null;
            equippedPants = null;
        }
    }
}
