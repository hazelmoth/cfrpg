using System.Collections.Generic;
using FeatureGenerators;
using UnityEngine;

namespace ContentLibraries
{
    [CreateAssetMenu(menuName = "Content Libraries/Region Feature Library")]
    public class RegionFeatureLibraryAsset : ScriptableObject
    {
        public List<RegionFeatureGenerator> features;
    
    }
}
