using System.Collections.Generic;
using ContinentMaps;
using UnityEngine;

/// A component which activates region travel if the player touches its trigger collider.
public class RegionPortal : MonoBehaviour, ISaveable
{
    [SerializeField] private Direction exitDirection = Direction.Right;
    [SerializeField] private string portalTag;
    [SerializeField] private bool exitOnly;

    public string ComponentId => "RegionPortal";
    public Direction ExitDirection => exitDirection;
    public string PortalTag => portalTag;
    public bool ExitOnly => exitOnly;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent(out Actor actor)) return;
        if (actor.ActorId != PlayerController.PlayerActorId) return;
        RegionTravel.TravelToAdjacent(actor, exitDirection, portalTag, null);
    }

    public IDictionary<string, string> GetTags()
    {
        return new Dictionary<string, string> {{"tag", portalTag}};
    }

    public void SetTags(IDictionary<string, string> tags)
    {
        if (tags.TryGetValue("tag", out string value)) portalTag = value;
    }
}
