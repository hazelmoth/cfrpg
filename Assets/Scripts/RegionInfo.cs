using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

// Contains information about a particular region in the world. This
// information holds regardless of whether the region has actually been generated.
[Serializable]
public class RegionInfo
{
    [JsonRequired][SerializeField] private string id;

    public List<AuthoredRegionMap.RegionConnection> connections;

    /// The name of the general map area this region is in (e.g. "Big Valley")
    /// or the specific name of the feature on this region, if one exists (e.g.
    /// "Downes Ranch").
    public string areaName;

    /// The seed to be used when generating this region. Regions with identical
    /// seeds and topographies will be generated identically.
    public int seed;

    public bool isWater;

    public List<Direction> coasts;

    public string biome;

    public bool playerHome;

    public bool disableAutoRegionTravel;

    /// The major feature on this region, if any
    public string feature;

    /// The IDs of the Actors who currently live in this region, if any.
    [HideInInspector]
    public List<string> residents;

    /**
     * The IDs of any unspawned actors in this region. These actors should be spawned into
     * the region when it is generated or loaded.
     */
    [HideInInspector]
    public List<string> unspawnedActors;

    public string Id => id;

    public override string ToString()
    {
        return string.Format(
            "{0}: {1}, {2}: {3}, {4}: {5}, {6}: {7}, {8}: {9}, {10}: {11}, {12}: {13}, {14}: {15}, {16}: {17}, {18}: {19}, {20}: {21}, {22}: {23}, {24}: {25}",
            nameof(id),
            id,
            nameof(connections),
            connections,
            nameof(areaName),
            areaName,
            nameof(seed),
            seed,
            nameof(isWater),
            isWater,
            nameof(coasts),
            coasts,
            nameof(biome),
            biome,
            nameof(playerHome),
            playerHome,
            nameof(disableAutoRegionTravel),
            disableAutoRegionTravel,
            nameof(feature),
            feature,
            nameof(residents),
            residents,
            nameof(unspawnedActors),
            unspawnedActors,
            nameof(Id),
            Id);
    }
}
