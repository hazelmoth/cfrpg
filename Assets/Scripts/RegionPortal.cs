using System;
using ContinentMaps;
using UnityEngine;

/// A component which activates region travel if the player touches its trigger collider.
public class RegionPortal : MonoBehaviour
{
    [SerializeField] private Direction exitDirection = Direction.Right;
    public Direction ExitDirection => exitDirection;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent(out Actor actor)) return;
        if (actor.ActorId != PlayerController.PlayerActorId) return;

        RegionTravel.TravelToAdjacent(actor, exitDirection, null);
    }
}
