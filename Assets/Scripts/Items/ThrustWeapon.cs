using UnityEngine;

namespace Items
{
	[CreateAssetMenu(fileName = "NewItem", menuName = "Items/ThrustWeapon", order = 1)]
    public class ThrustWeapon : ItemData, IThrustWeapon
    {
        [SerializeField] private Sprite weaponSprite = null;
        [SerializeField] private float weaponForce = 0f;
        [SerializeField] private float weaponRange = 1f;
        [SerializeField] private ImpactInfo.DamageType damageDamageType = ImpactInfo.DamageType.Punch;
        [SerializeField] private float thrustDistance = 0.2f;
        [SerializeField] private float thrustDuration = 0.5f;
        [SerializeField] private Direction spritePointDirection = Direction.Up;


        ImpactInfo.DamageType IThrustWeapon.DamageType => damageDamageType;
        float IThrustWeapon.WeaponForce => weaponForce;
        float IThrustWeapon.WeaponRange => weaponRange;
        float IThrustWeapon.ThrustDistance => thrustDistance;
        float IThrustWeapon.ThrustDuration => thrustDuration;

        Sprite IAimable.heldItemSprite => weaponSprite;
        Direction IAimable.pointDirection => spritePointDirection;
    }
}
