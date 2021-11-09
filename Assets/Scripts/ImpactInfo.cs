using UnityEngine;

public class ImpactInfo
{
    public ImpactInfo(DamageType damageType, Actor source, Vector2 force)
    {
        this.damageType = damageType;
        this.source = source;
        this.force = force;
    }

    public readonly DamageType damageType;
    public readonly Actor source; // The actor who delivered this attack, if there was one
    public readonly Vector2 force; // The direction + force of the impact; magnitude = base damage dealt

    public enum DamageType
    {
        Punch,
        Blunt,
        Chop,
        Stab,
        Slash,
        Gunshot,
    }
}
