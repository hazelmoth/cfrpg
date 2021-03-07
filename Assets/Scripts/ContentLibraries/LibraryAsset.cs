using System.Collections.Generic;
using UnityEngine;

namespace ContentLibraries
{
    [CreateAssetMenu]
    public class LibraryAsset<T> : ScriptableObject where T : IContentItem 
    {
        [SerializeField]
        public List<T> content;
    }
}