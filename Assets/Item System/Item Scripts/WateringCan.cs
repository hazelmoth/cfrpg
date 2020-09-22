using Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Items/Watering Can")]
public class WateringCan : ItemData, IPointable, ITileSelectable
{
    [SerializeField] private Sprite itemSprite;
    [SerializeField] private Direction spritePointDirection;
    [SerializeField] private float range = 6.0f;
    [SerializeField] private bool visibleTileSelector = true;

    Sprite IPointable.heldItemSprite => itemSprite;
    Direction IPointable.pointDirection => spritePointDirection;
    float ITileSelectable.TileSelectorRange => range;
    bool ITileSelectable.VisibleTileSelector => visibleTileSelector;


    void ITileSelectable.Use(TileLocation target)
    {
        GameObject entity = WorldMapManager.GetEntityObjectAtPoint(new Vector2Int(target.x, target.y), target.Scene);
        if (entity != null)
        {
            if (entity.GetComponent<GrowablePlant>() is GrowablePlant plant) {
                plant.Water();
            }
        }
    }
}
