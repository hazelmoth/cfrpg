using System.Collections.Generic;
using MyBox;
using UnityEngine;

[CreateAssetMenu]
public class MixedAuthoredWorldTemplate : ScriptableObject
{
    public Vector2Int size;
    public List<AuthoredRegionMap> regions;
}
