using ContentLibraries;
using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName = "NewItem", menuName = "Items/SeedBag", order = 1)]
    public class SeedBag : ItemData, IPloppable
    {
        [SerializeField] private string plantEntityId = null;

        bool IPloppable.VisibleTileSelector(ItemStack instance)
        {
            return true;
        }

        ItemStack IPloppable.Use(TileLocation target, ItemStack instance)
        {
            EntityData entity = ContentLibrary.Instance.Entities.Get(plantEntityId);
            if (entity == null)
            {
                Debug.LogError("Entity " + plantEntityId + " not found!");
                return instance;
            }

            GroundMaterial ground = RegionMapManager.GetGroundMaterialAtPoint(
                target.Vector2.ToVector2Int(),
                target.scene);
            GroundMaterial groundCover =
                RegionMapManager.GetGroundCoverAtPoint(target.Vector2.ToVector2Int(), target.scene);

            if (ground == null) return instance;

            if ((groundCover != null || !ground.isFarmland) && (groundCover == null || !groundCover.isFarmland))
                return instance;

            string currentEntity = RegionMapManager.GetEntityIdAtPoint(target.Vector2.ToVector2Int(), target.scene);
            if (currentEntity != null) return instance;

            if (RegionMapManager.AttemptPlaceEntityAtPoint(entity, target.Vector2.ToVector2Int(), target.scene))
                return instance.Decremented();
            return instance;
        }
    }
}