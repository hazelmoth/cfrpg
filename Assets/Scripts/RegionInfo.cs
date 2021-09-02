using System.Collections.Generic;

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

    public bool isWater;
    
    public List<Direction> coasts;

    public string biome;

    public bool playerHome;

    public bool disableAutoRegionTravel;

    // The major feature on this region, if any
    public string feature;
    
    /*
     * The IDs of the Actors who currently live in this region, if any.
     */
    public List<string> residents;
    
    /*
     * The IDs of any unspawned actors in this region. These actors should be
     * spawned into the region when it is generated or loaded.
     */
    public List<string> unspawnedActors;
}
