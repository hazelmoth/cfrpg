using Items;
using UnityEngine;

namespace Items
{
    [CreateAssetMenu(menuName = "Items/Watering Can")]
    public class WateringCan : ItemData, IAimable, ITileSelectable
    {
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
            GameObject entity = RegionMapManager.GetEntityObjectAtPoint(new Vector2Int(target.X, target.Y), target.scene);
            if (entity != null)
            {
                if (entity.GetComponent<GrowablePlant>() is GrowablePlant plant)
                {
                    plant.Water();
                }
            }
        }
    }
}