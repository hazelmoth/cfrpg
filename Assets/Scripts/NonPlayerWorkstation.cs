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
            Location origin = GetComponentInParent<EntityObject>()?.Location;
            if (origin == null)
            {
                Debug.LogError("Workstation object is missing EntityObject component.", this);
                return new TileLocation(
                    transform.position.ToVector2Int() + userTileLocationOffset,
                    SceneObjectManager.WorldSceneId);
            }

            return new TileLocation(origin.Vector2.ToVector2Int() + userTileLocationOffset, origin.scene);
        }
    }

    /// Which direction the user of this object should look when using this station.
    /// (Note: this is not enforced.)
    public Direction UserDirection => userDirection;
}