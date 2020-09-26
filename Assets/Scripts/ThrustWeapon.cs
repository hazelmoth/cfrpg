using UnityEngine;

namespace Items
{
	[CreateAssetMenu(fileName = "NewItem", menuName = "Items/ThrustWeapon", order = 1)]
    public class ThrustWeapon : ItemData, IThrustWeapon
    {
        [SerializeField] private Sprite weaponSprite = null;
        [SerializeField] private float weaponForce = 0f;
        [SerializeField] private float weaponRange = 1f;
        [SerializeField] private float thrustDistance = 0.2f;
        [SerializeField] private float thrustDuration = 0.5f;
        [SerializeField] private Direction spritePointDirection = Direction.Up;


        float IThrustWeapon.WeaponForce => weaponForce;
        float IThrustWeapon.WeaponRange => weaponRange;
        float IThrustWeapon.ThrustDistance => thrustDistance;
        float IThrustWeapon.ThrustDuration => thrustDuration;

        Sprite IPointable.heldItemSprite => weaponSprite;
        Direction IPointable.pointDirection => spritePointDirection;
    }
}