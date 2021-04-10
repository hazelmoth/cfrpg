using UnityEngine;

public class ImpactInfo
{
    public ImpactInfo(Type type, Actor source, Vector2 force)
    {
        this.type = type;
        this.source = source;
        this.force = force;
    }

    public readonly Type type;
    public readonly Actor source; // The actor who delivered this attack, if there was one
    public readonly Vector2 force; // The direction + force of the impact; magnitude = base damage dealt

    public enum Type
    {
        Bullet,
        Melee
    }
}
