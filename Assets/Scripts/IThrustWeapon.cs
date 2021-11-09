namespace Items
{
	public interface IThrustWeapon : IAimable
    {
        ImpactInfo.DamageType DamageType { get; }
        float WeaponForce { get; }
        float WeaponRange { get; }
        float ThrustDistance { get; }
        float ThrustDuration { get; }
    }
}
