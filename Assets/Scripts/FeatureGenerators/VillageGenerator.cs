using System.Collections.Generic;
using System.Linq;
using ContentLibraries;
using UnityEngine;

namespace FeatureGenerators
{
    [CreateAssetMenu(menuName = "Feature Generator/Village")]
    public class VillageGenerator : RegionFeatureGenerator
    {
        private const int PlacementAttempts = 100;
        private const int MinResidences = 3;
        private const int MaxResidences = 8;

        private int targetResidenceCount;

        [SerializeField] private string residentTemplate;
        [SerializeField] private List<string> residenceEntityIds;
    
        public override bool AttemptApply(RegionMap region, int seed)
        {
            Random.InitState(seed);
            if (targetResidenceCount == 0) targetResidenceCount = Random.Range(MinResidences, MaxResidences + 1);
        
            int placedResidences = 0;
            for (int i = 0; i < PlacementAttempts; i++)
            {
                EntityData residenceData = ContentLibrary.Instance.Entities.Get(residenceEntityIds.PickRandom());
                int x = Random.Range(0, SaveInfo.RegionSize.x);
                int y = Random.Range(0, SaveInfo.RegionSize.y);
            
                if (RegionGenerator.AttemptPlaceEntity(
                    residenceData, 
                    1, 
                    new Vector2(x, y), 
                    new List<string>(), 
                    region, 
                    SaveInfo.RegionSize.x,
                    SaveInfo.RegionSize.y))
                {
                    placedResidences++;
                }

                if (placedResidences == targetResidenceCount) break;
            }

            return (placedResidences > 0);
        }

        public override List<ActorData> GenerateResidents()
        {
            if (targetResidenceCount == 0) targetResidenceCount = Random.Range(MinResidences, MaxResidences + 1);
        
            return Enumerable.Range(0, targetResidenceCount).Select(i =>
                ActorGenerator.Generate(ContentLibrary.Instance.ActorTemplates.Get(residentTemplate))).ToList();
        }
    }
}
