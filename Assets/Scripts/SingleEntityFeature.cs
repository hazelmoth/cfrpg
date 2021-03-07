using System.Collections.Generic;
using ContentLibraries;
using UnityEngine;

[CreateAssetMenu(menuName = "Region Feature/Single Entity")]
class SingleEntityFeature : RegionFeature
{
    private const int PlacementAttempts = 10;
    
    [SerializeField] private string entityId;
    
    public override bool AttemptApply(RegionMap region, int seed)
    {
        EntityData shackData = ContentLibrary.Instance.Entities.Get(entityId);
        Vector2 mapCenter = new Vector2(SaveInfo.RegionSize.x / 2f, SaveInfo.RegionSize.y / 2f);

        if (!RegionGenerator.AttemptPlaceEntity(shackData, PlacementAttempts, mapCenter, new List<string>(), region, SaveInfo.RegionSize.x,
            SaveInfo.RegionSize.y))
        {
            return false;
        }

        return true;
    }
}