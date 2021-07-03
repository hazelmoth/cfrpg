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
    [SerializeField] private string grassMaterial = "dead_grass";
    [SerializeField] private List<BiotopeInfo> biotopes;
    [SerializeField] private int minSpawnCount;
    [SerializeField] private int maxSpawnCount;
    [SerializeField] private WeightedTable spawnTable;
    
    public string Id => id;
    public string Name => name;
    public float Frequency => frequency;
    public string GrassMaterial => grassMaterial;
    public List<BiotopeInfo> Biotopes => biotopes;

    public IList<String> PickSpawnTemplates()
    {
        List<String> result = new List<string>();
        int count = Mathf.FloorToInt(Random.value * (maxSpawnCount - minSpawnCount) - 0.00001f) + minSpawnCount;
        for (int i = 0; i < count; i++)
        {
            result.Add(spawnTable.PickRandom());
        }

        return result;
    }
    
    [Serializable]
    public struct BiotopeInfo
    {
        public string biotopeId;
        public float frequency;
    }
}
