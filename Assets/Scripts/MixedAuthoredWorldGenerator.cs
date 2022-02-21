using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using ContentLibraries;
using ContinentMaps;
using Newtonsoft.Json;
using UnityEngine;

public class MixedAuthoredWorldGenerator : WorldGenerator
{
    [SerializeField] private MixedAuthoredWorldTemplate worldTemplate;

    public override WorldMap Generate(int seed)
    {
        string worldName = ContinentNameGenerator.Generate(seed);

        int sizeX = worldTemplate.size.x;
        int sizeY = worldTemplate.size.y;
        List<Region> regions = new();

        worldTemplate.regions.ForEach(
            authoredMap =>
            {
                RegionInfo regionInfo = JsonClone(JsonSerializer.CreateDefault(), authoredMap.RegionInfo);
                Region region = new() { info = regionInfo };
                if (!authoredMap.Generated)
                {
                    // Generate and register actors from templates
                    ImmutableList<ActorData> residentData = authoredMap.ResidentTemplates.Pick()
                        .Select(
                            actorTemplate =>
                                ActorGenerator.Generate(ContentLibrary.Instance.ActorTemplates.Get(actorTemplate)))
                        .ToImmutableList();

                    residentData.ForEach(ActorRegistry.Register);

                    List<string> residents =
                        residentData
                        .Select(actorData => actorData.ActorId)
                        .ToList();
                    region.info.residents = residents;
                    region.info.unspawnedActors = residents;
                    region.data = RegionMapManager.BuildRegionFromPrefab(authoredMap.RegionPrefab);
                }
                regions.Add(region);
            });
        return new WorldMap(worldName, new Vector2Int(sizeX, sizeY), regions);
    }

    /// Clones the given object using the given serializer.
    private static T JsonClone<T>(JsonSerializer jsonSerializer, T value)
    {
        Type objectType = value?.GetType();
        using (MemoryStream ms = new())
        {
            using (StreamWriter sw = new(ms, new UTF8Encoding(false), 256, true))
            {
                jsonSerializer.Serialize(sw, value);
            }
            ms.Position = 0;
            using (StreamReader sr = new(ms))
            {
                return (T)jsonSerializer.Deserialize(sr, objectType);
            }
        }
    }
}
