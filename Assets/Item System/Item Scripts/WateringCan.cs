using Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Items/Watering Can")]
public class WateringCan : ItemData, IPointable
{
    [SerializeField] private Sprite itemSprite;
    [SerializeField] private Direction spritePointDirection;

    Sprite IPointable.heldItemSprite => itemSprite;
    Direction IPointable.pointDirection => spritePointDirection;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
