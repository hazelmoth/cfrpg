using System;
using System.Collections;
using System.Collections.Generic;
using ContentLibraries;
using UnityEngine;

[CreateAssetMenu(menuName = "Biome")]
public class Biome : ScriptableObject, IContentItem
{
    [SerializeField] private string id;
    [SerializeField] private new string name;
    [SerializeField] private float frequency = 1;
    [SerializeField] private List<BiotopeInfo> biotopes;
    
    public string Id => id;
    public string Name => name;
    // The relative frequency of this biome compared to a 'typical' one
    public float Frequency => frequency;
    // The individual biotopes that constitute this biome
    public List<BiotopeInfo> Biotopes => biotopes;
    
    [Serializable]
    public struct BiotopeInfo
    {
        public string biotopeId;
        public float frequency;
    }
}
