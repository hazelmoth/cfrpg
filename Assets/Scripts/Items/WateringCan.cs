using ContentLibraries;
using Items;
using UnityEngine;

namespace Items
{
    [CreateAssetMenu(menuName = "Items/Watering Can")]
    public class WateringCan : ItemData, IAimable, ITileSelectable
    {
        private const string MoistFarmlandId = "farmland_moist";

        [SerializeField] private Sprite itemSprite = null;
        [SerializeField] private Direction spritePointDirection;
        [SerializeField] private float range = 6.0f;
        [SerializeField] private bool visibleTileSelector = true;

        Sprite IAimable.heldItemSprite => itemSprite;
        Direction IAimable.pointDirection => spritePointDirection;
        float ITileSelectable.TileSelectorRange => range;
        bool ITileSelectable.VisibleTileSelector => visibleTileSelector;


        void ITileSelectable.Use(TileLocation target)
        {
            GroundMaterial groundCover = RegionMapManager.GetGroundCoverAtPoint(target.Vector2Int, target.scene);

            // If the ground cover is farmland, change it to moist farmland.
            if (groundCover is { isFarmland: true })
            {
                RegionMapManager.ChangeGroundMaterial(
                    target.Vector2Int,
                    target.scene,
                    TilemapLayer.GroundCover,
                    ContentLibrary.Instance.GroundMaterials.Get(MoistFarmlandId));
            }

            // Set the map unit as moisturized.
            RegionMapManager.GetMapUnitAtPoint(target.Vector2Int, target.scene).lastMoisturizedTick =
                TimeKeeper.CurrentTick;
        }
    }
}
