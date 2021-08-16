using UnityEngine;

/// A thing that can be occupied by an Actor when they are standing at a certain location.
public class NonPlayerWorkstation : Occupiable
{
    /// Which tile the user of this station should stand on, relative to this object's
    /// origin.
    [SerializeField] [Tooltip("Which tile the user of this station should stand on, relative to this object's origin.")]
    private Vector2Int userTileLocationOffset;

    [SerializeField] private Direction userDirection;

    /// Which tile the user of this object should stand on to use this station.
    /// (Note: this is not enforced.)
    public TileLocation UserTileLocation
    {
        get
        {
            TileLocation origin = GetComponentInParent<EntityObject>()?.Location;
            if (origin != null)
                return new TileLocation(origin.Vector2Int + userTileLocationOffset, origin.scene);

            Debug.LogError("Workstation object is missing EntityObject component.", this);
            return new TileLocation(
                transform.position.ToVector2Int() + userTileLocationOffset,
                SceneObjectManager.WorldSceneId);
        }
    }

    /// Which direction the user of this object should look when using this station.
    /// (Note: this is not enforced.)
    public Direction UserDirection => userDirection;
}
