using System.Collections.Generic;
using System.Collections.Immutable;
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
    
        public override bool AttemptApply(RegionMap region, RegionInfo info, int seed)
        {
            Random.InitState(seed);
            if (targetResidenceCount == 0) 
                targetResidenceCount = Random.Range(MinResidences, MaxResidences + 1);

            List<Vector2Int> placements = new List<Vector2Int>();
            for (int i = 0; i < PlacementAttempts; i++)
            {
                EntityData residenceData = ContentLibrary.Instance.Entities.Get(residenceEntityIds.PickRandom());
                int x = Random.Range(0, SaveInfo.RegionSize.x);
                int y = Random.Range(0, SaveInfo.RegionSize.y);
            
                if (region.AttemptPlaceEntity(
                    residenceData,
                    1,
                    new Vector2(x, y), 
                    RegionMapUtil.PlacementSettings.PlaceOverAnything, 
                    out Vector2Int location))
                {
                    placements.Add(location);
                }
                if (placements.Count == targetResidenceCount) break;
            }
            
            // Assign houses to citizens by setting save tags.
            ImmutableList<string> residents = info.residents.ToImmutableList();
            List<Vector2Int> availableHomes = placements.ToList();
            residents.ForEach(residentId =>
            {
                if (!placements.Any()) return;
                Vector2Int housePos = availableHomes[availableHomes.Count - 1];
                availableHomes.RemoveAt(availableHomes.Count - 1);
                
                MapUnit mapUnit = region.mapDict[SceneObjectManager.WorldSceneId][housePos];
                /*mapUnit.savedComponents.Add(
                    new SavedComponentState(
                        "house",
                        new Dictionary<string, string> {{SettlementSystem.BuildingInfo.OwnerSaveTag, residentId}}));*/
                Debug.Log(string.Join(", ", mapUnit.savedComponents));
            });
            
            // If any buildings were ultimately placed, we'll qualify that as a success.
            return (placements.Count > 0);
        }

        public override IEnumerable<ActorData> GenerateResidents()
        {
            if (targetResidenceCount == 0) targetResidenceCount = Random.Range(MinResidences, MaxResidences + 1);
        
            return Enumerable.Range(0, targetResidenceCount).Select(i =>
                ActorGenerator.Generate(ContentLibrary.Instance.ActorTemplates.Get(residentTemplate))).ToList();
        }
    }
}
