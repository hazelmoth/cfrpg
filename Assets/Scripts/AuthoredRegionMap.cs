using UnityEngine;

[CreateAssetMenu]
public class AuthoredRegionMap : ScriptableObject
{
    [SerializeField] private Vector2Int location;
    [SerializeField] private GameObject regionPrefab;
    [SerializeField] private RegionInfo regionInfo;

    public Vector2Int Location => location;
    public GameObject RegionPrefab => regionPrefab;
    public RegionInfo RegionInfo => regionInfo;
}
