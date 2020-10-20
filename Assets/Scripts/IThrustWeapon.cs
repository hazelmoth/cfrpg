namespace Items
{
	public interface IThrustWeapon : IAimable
    {
        float WeaponForce { get; }
        float WeaponRange { get; }
        float ThrustDistance { get; }
        float ThrustDuration { get; }
    }
}