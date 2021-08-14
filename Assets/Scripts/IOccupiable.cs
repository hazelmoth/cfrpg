/// A thing which actors may mark as occupied on a frame-by-frame basis.
public interface IOccupiable
{
    /// Whether this object is currently occupied by an Actor.
    public bool Occupied { get; }

    /// The actor who is occupying this object during the current frame, or null if it is
    /// unoccupied.
    public Actor CurrentOccupier { get; }

    /// Attempts to claim occupation during the next frame for the given Actor.
    /// The claim succeeds if the actor is the current occupier, or if there is no
    /// occupier and no existing claimants. Returns true if claim is successful.
    /// This method must be called each frame that an Actor wants to occupy this object.
    public bool OccupyNextFrame(Actor user);
}