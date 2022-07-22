using System.Collections.Generic;
using ContentLibraries;
using UnityEngine;

namespace FeatureGenerators
{
    [CreateAssetMenu(menuName = "Feature Generator/Homestead")]
    public class HomesteadGenerator : SingleEntityFeatureGenerator
    {
        [SerializeField] private string actorTemplate;
    
        public override IEnumerable<ActorData> GenerateResidents()
        {
            return new List<ActorData>
            {
                (ContentLibrary.Instance.ActorTemplates.Get(actorTemplate)).CreateActor(
                    ActorRegistry.IdIsAvailable,
                    out _)
            };
        }
    }
}
