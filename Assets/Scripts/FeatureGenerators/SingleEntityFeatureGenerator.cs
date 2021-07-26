using System.Collections.Generic;
using ContentLibraries;
using UnityEngine;

namespace FeatureGenerators
{
    [CreateAssetMenu(menuName = "Feature Generator/Single Entity")]
    public class SingleEntityFeatureGenerator : RegionFeatureGenerator
    {
        private const int PlacementAttempts = 10;
    
        [SerializeField] private string entityId;
    
        public override bool AttemptApply(RegionMap region, RegionInfo info, int seed)
        {
            EntityData shackData = ContentLibrary.Instance.Entities.Get(entityId);
            Vector2 mapCenter = new Vector2(SaveInfo.RegionSize.x / 2f, SaveInfo.RegionSize.y / 2f);

            return RegionGenerator.AttemptPlaceEntity(
                shackData,
                PlacementAttempts,
                mapCenter,
                new List<string>(),
                region,
                SaveInfo.RegionSize.x,
                SaveInfo.RegionSize.y);
        }
    }
}