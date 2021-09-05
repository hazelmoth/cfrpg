using System.Collections.Generic;
using System.Linq;
using MyBox;
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
        RegionInfo[,] regionInfos = new RegionInfo[sizeX, sizeY];
        RegionMap[,] regions = new RegionMap[sizeX, sizeY];

        worldTemplate.regions.ForEach(
            map =>
            {
                regionInfos[map.Location.x, map.Location.y] = map.RegionInfo;
                if (!map.Generated)
                {
                    // Generate and register actors from templates
                    List<string> residents = map.ResidentTemplates.Pick()
                        .Select(
                            actorTemplate =>
                                ActorGenerator.Generate(ContentLibrary.Instance.ActorTemplates.Get(actorTemplate)))
                        .ForEach(ActorRegistry.Register)
                        .Select(actorData => actorData.ActorId)
                        .ToList();
                    map.RegionInfo.residents = residents;
                    map.RegionInfo.unspawnedActors = residents;
                    regions[map.Location.x, map.Location.y] = RegionMapManager.BuildMapForScene(map.RegionPrefab);
                }
            });
        return new WorldMap(worldName, new Vector2Int(sizeX, sizeY), regionInfos, regions);
    }
}
