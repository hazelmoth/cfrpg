using System.Collections.Generic;
using MyBox;
using UnityEngine;

[CreateAssetMenu]
public class AuthoredRegionMap : ScriptableObject
{
    [SerializeField] private Vector2Int location;
    [SerializeField] private GameObject regionPrefab;
    [SerializeField] private CompoundWeightedTable residentTemplates;
    [Separator]
    [SerializeField] private RegionInfo regionInfo;

    public Vector2Int Location => location;
    public GameObject RegionPrefab => regionPrefab;
    public CompoundWeightedTable ResidentTemplates => residentTemplates;
    public RegionInfo RegionInfo => regionInfo;
}
