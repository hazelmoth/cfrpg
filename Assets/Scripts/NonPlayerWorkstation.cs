using UnityEngine;

/// A thing that can be occupied by an Actor when they are standing at a certain location.
public class NonPlayerWorkstation : Occupiable
{
    /// Which tile the user of this station should stand on, relative to this object's
    /// origin.
    [SerializeField] [Tooltip("Which tile the user of this station should stand on, relative to this object's origin.")]
    private Vector2 userLocationOffset;

    [SerializeField] private Direction userDirection;

    /// Where the user of this object should stand on to use this station.
    /// (Note: this is not enforced.)
    public Location UserLocation
    {
        get
        {
            TileLocation origin = GetComponentInParent<EntityObject>()?.Location;
            if (origin != null)
                return origin.WithOffset(userLocationOffset);

            Debug.LogError("Workstation object is missing EntityObject component.", this);
            return new Location(
                transform.localPosition.ToVector2() + userLocationOffset,
                PlayerController.GetPlayerActor().CurrentScene);
        }
    }

    /// Which direction the user of this object should look when using this station.
    /// (Note: this is not enforced.)
    public Direction UserDirection => userDirection;
}
