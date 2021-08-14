using UnityEngine;

/// An abstract class for an item that can be occupied by some actor on a frame-by-frame
/// basis. Resets occupying actor in LateUpdate.
public abstract class Occupiable : MonoBehaviour, IOccupiable
{
    /// The Actor who has attempted to occupy during the current frame, if any.
    private Actor claimantOfNextFrame;

    private void LateUpdate()
    {
        CurrentOccupier = claimantOfNextFrame;
        claimantOfNextFrame = null;
    }

    public bool OccupyNextFrame(Actor user)
    {
        // If there's a current occupier, only they can claim the next frame.
        if (Occupied && user.ActorId != CurrentOccupier.ActorId) return false;
        // If not occupied, the first claimant keeps his claim.
        if (!Occupied && claimantOfNextFrame != null && claimantOfNextFrame.ActorId != user.ActorId) return false;
        claimantOfNextFrame = user;
        return true;
    }

    /// Whether some Actor is occupying this object.
    public bool Occupied => CurrentOccupier != null;

    /// The Actor currently occupying this object.
    public Actor CurrentOccupier { get; private set; }
}