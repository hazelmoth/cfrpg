using System.Collections.Generic;
using ContentLibraries;
using UnityEngine;

[CreateAssetMenu(menuName = "Region Feature/Homestead")]
public class Homestead : SingleEntityFeature
{
    [SerializeField] private string actorTemplate;
    
    public override List<ActorData> GenerateResidents()
    {
        return new List<ActorData> { ActorGenerator.Generate(ContentLibrary.Instance.ActorTemplates.Get(actorTemplate)) };
    }
}
