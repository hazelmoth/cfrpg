using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ContentLibraries;
using ContinentMaps;
using UnityEngine;

public class MixedAuthoredWorldGenerator : WorldGenerator
{
    [SerializeField] private MixedAuthoredWorldTemplate worldTemplate;

    public override WorldMap Generate(int seed)
    {
        string worldName = ContinentNameGenerator.Generate(seed);

        int sizeX = worldTemplate.size.x;
        int sizeY = worldTemplate.size.y;
        List<RegionInfo> regions = new List<RegionInfo>();

        worldTemplate.regions.ForEach(
            map =>
            {
                regions.Add(map.RegionInfo);
                if (!map.Generated)
                {
                    // Generate and register actors from templates
                    ImmutableList<ActorData> residentData = map.ResidentTemplates.Pick()
                        .Select(
                            actorTemplate =>
                                ActorGenerator.Generate(ContentLibrary.Instance.ActorTemplates.Get(actorTemplate)))
                        .ToImmutableList();

                    residentData.ForEach(ActorRegistry.Register);

                    List<string> residents =
                        residentData
                        .Select(actorData => actorData.ActorId)
                        .ToList();
                    map.RegionInfo.residents = residents;
                    map.RegionInfo.unspawnedActors = residents;
                    map.RegionInfo.mapData = RegionMapManager.BuildMapForScene(map.RegionPrefab);
                }
            });
        return new WorldMap(worldName, new Vector2Int(sizeX, sizeY), regions);
    }
}
