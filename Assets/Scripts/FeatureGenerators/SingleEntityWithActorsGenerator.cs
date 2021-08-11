using System.Collections.Generic;
using System.Linq;
using ContentLibraries;
using MyBox;
using UnityEngine;

namespace FeatureGenerators
{
    [CreateAssetMenu(menuName = "Feature Generator/Single Entity With Actors")]
    public class SingleEntityWithActorsGenerator : SingleEntityFeatureGenerator
    {
        [Separator] [SerializeField] private CompoundWeightedTable actorTemplateTable;

        public override IEnumerable<ActorData> GenerateResidents()
        {
            return actorTemplateTable.Pick()
                .Select(template => ActorGenerator.Generate(ContentLibrary.Instance.ActorTemplates.Get(template)));
        }
    }
}