using UnityEngine;

namespace Items
{
	[CreateAssetMenu(fileName = "NewItem", menuName = "Items/SeedBag", order = 1)]
    public class SeedBag : ItemData, IPloppable
    {
        [SerializeField] private string plantEntityId = null;

        void IPloppable.Use(TileLocation target)
        {
            EntityData entity = ContentLibrary.Instance.Entities.Get(plantEntityId);
            if (entity == null)
            {
                Debug.LogError("Entity " + plantEntityId + " not found!");
                return;
            }
            GroundMaterial ground = WorldMapManager.GetGroundMaterialtAtPoint(target.Position.ToVector2Int(), target.Scene);
            if (ground != null && ground.isFarmable)
            {
                WorldMapManager.AttemptPlaceEntityAtPoint(entity, target.Position.ToVector2Int(), target.Scene);
            }
        }
    }
}