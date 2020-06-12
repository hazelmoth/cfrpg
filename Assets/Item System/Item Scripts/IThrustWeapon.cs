using UnityEngine;

namespace Items
{
    public interface IThrustWeapon : IPointable
    {
        float WeaponForce { get; }
        float WeaponRange { get; }
        float ThrustDistance { get; }
        float ThrustDuration { get; }
    }
}