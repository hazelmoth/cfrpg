using System;
using System.Collections;
using System.Collections.Generic;
using ContentLibraries;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Biome")]
public class Biome : ScriptableObject, IContentItem
{
    [SerializeField] private string id;
    [SerializeField] private new string name;
    [SerializeField] private float frequency = 1;
    [SerializeField] private string groundMaterial = "dead_grass";
    [SerializeField] private List<BiotopeInfo> biotopes;
    [SerializeField] private int minSpawnCount;
    [SerializeField] private int maxSpawnCount;
    [SerializeField] private CompoundWeightedTable spawnTable;
    
    public string Id => id;
    public string Name => name;
    public float Frequency => frequency;
    public string GroundMaterial => groundMaterial;
    public List<BiotopeInfo> Biotopes => biotopes;

    public IList<String> PickSpawnTemplates()
    {
        return spawnTable.Pick();
    }
    
    [Serializable]
    public struct BiotopeInfo
    {
        public string biotopeId;
        public float frequency;
    }
}
