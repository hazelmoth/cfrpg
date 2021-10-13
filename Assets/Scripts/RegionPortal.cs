using System.Collections.Generic;
using ContinentMaps;
using UnityEngine;

/// A component which activates region travel if the player touches its trigger collider.
public class RegionPortal : MonoBehaviour, ISaveable
{
    [SerializeField] private Direction exitDirection = Direction.Right;
    [SerializeField] private string connectionTag;

    public string ComponentId => "RegionPortal";
    public Direction ExitDirection => exitDirection;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent(out Actor actor)) return;
        if (actor.ActorId != PlayerController.PlayerActorId) return;
        RegionTravel.TravelToAdjacent(actor, exitDirection, connectionTag, null);
    }

    public IDictionary<string, string> GetTags()
    {
        return new Dictionary<string, string> {{"tag", connectionTag}};
    }

    public void SetTags(IDictionary<string, string> tags)
    {
        if (tags.TryGetValue("tag", out string value)) connectionTag = value;
    }
}
