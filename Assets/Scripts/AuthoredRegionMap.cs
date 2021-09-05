using MyBox;
using UnityEngine;

[CreateAssetMenu]
public class AuthoredRegionMap : ScriptableObject
{
    [SerializeField] private Vector2Int location;
    [SerializeField] private bool generated;
    [SerializeField] [ConditionalField(nameof(generated))] private Vector2Int regionSize;
    [SerializeField] [ConditionalField(nameof(generated), true)] private GameObject regionPrefab;
    [SerializeField] [ConditionalField(nameof(generated), true)] private CompoundWeightedTable residentTemplates;
    [Separator]
    [SerializeField] private RegionInfo regionInfo;

    public Vector2Int Location => location;
    public bool Generated => generated;
    /// The size of this region if it is to be generated; a default vector otherwise
    public Vector2Int Size => regionSize;
    public GameObject RegionPrefab => regionPrefab;
    public CompoundWeightedTable ResidentTemplates => residentTemplates;
    public RegionInfo RegionInfo => regionInfo;
}
