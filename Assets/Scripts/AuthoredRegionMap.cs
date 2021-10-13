using System.Collections.Generic;
using MyBox;
using UnityEngine;

[CreateAssetMenu]
public class AuthoredRegionMap : ScriptableObject
{
    [SerializeField] private bool generated;
    [SerializeField] [ConditionalField(nameof(generated), true)] private GameObject regionPrefab;
    [SerializeField] [ConditionalField(nameof(generated), true)] private CompoundWeightedTable residentTemplates;
    [Separator]
    [SerializeField] private RegionInfo regionInfo;

    public bool Generated => generated;
    public GameObject RegionPrefab => regionPrefab;
    public CompoundWeightedTable ResidentTemplates => residentTemplates;
    public RegionInfo RegionInfo => regionInfo;

    [System.Serializable]
    public struct RegionConnection
    {
        public Direction direction;
        public string portalTag;
        public string destinationId;
    }
}
