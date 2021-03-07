using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Contains information about a particular region in the world. This
// information holds regardless of whether the region has actually been generated.
[System.Serializable]
public class RegionInfo
{
    // The name of the general map area this region is in (e.g. "Big Valley")
    // or the specific name of the feature on this region, if one exists (e.g.
    // "Downes Ranch").
    public string areaName;

    // The seed to be used when generating this region. Regions with identical
    // seeds and topographies will be generated identically.
    public int seed;

    // The general topography of this region; e.g., coastal vs landlocked, etc.
    public RegionTopography topography;
    
    // TODO biome

    public bool playerHome;

    // The major feature on this region, if any
    public RegionFeature feature;
}