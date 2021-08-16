using System.Collections.Generic;
using System.Linq;
using ContentLibraries;
using FeatureGenerators;
using UnityEngine;

namespace ContinentMaps
{
    public static class ContinentGenerator
    {
        //   y
        //   ^
        //   |
        //   |
        //   *----->  x

        private const float WaterLevel = 0.35f;
        private const float RegionFeatureChance = 0.2f;

        public static WorldMap Generate(int sizeX, int sizeY, int seed)
        {
            Random.InitState(seed);
            
            string worldName = ContinentNameGenerator.Generate(seed);
            
            float[,] heightmap = new float[sizeX, sizeY];
            float[,] biomeMap = new float[sizeX, sizeY];
            RegionInfo[,] regions = new RegionInfo[sizeX, sizeY];

            // Create a heightmap to determine which regions are water vs. land
            for (int y = 0; y < sizeY; y++)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    // Create a RegionInfo for each region
                    regions[x, y] = new RegionInfo();
                    // Set a different seed for each region
                    regions[x, y].seed = x + 113 * y;
                    // Give each region the world name, for now
                    regions[x, y].areaName = worldName;

                    // Island heightmap
                    Vector2 point = new Vector2(x, y);
                    heightmap[x, y] = GenerationHelper.EllipseGradient(point - new Vector2(sizeX/2f, sizeY/2f), sizeX, sizeY);
                    // Mix in 30% simplex noise
                    heightmap[x, y] = 0.7f * heightmap[x, y] + 0.3f * GenerationHelper.UniformSimplex(x/10f, y/10f, seed);
                    heightmap[x, y] = Mathf.Clamp01(heightmap[x, y]);
                    
                    // Biome map; entirely simplex noise
                    biomeMap[x, y] = GenerationHelper.UniformSimplex(x / 7.5f, y / 7f, seed);
                    biomeMap[x, y] = Mathf.Clamp01(biomeMap[x, y]);

                    // Set regions below the water level to be water.
                    regions[x, y].isWater = heightmap[x, y] < WaterLevel;
                    
                    // Sometimes add a random feature.
                    if (!regions[x,y].isWater && Random.value < RegionFeatureChance)
                    {
                        RegionFeatureGenerator featureGenerator = ContentLibrary.Instance.RegionFeatures.Get(ContentLibrary.Instance
                            .RegionFeatures
                            .GetIdList().PickRandom());
                        
                        regions[x, y].feature = featureGenerator.Id;
                        
                        // Generate any residents that come with this feature.
                        IEnumerable<ActorData> residents = featureGenerator.GenerateResidents();
                        
                        regions[x, y].residents ??= new List<string>();
                        regions[x, y].unspawnedActors ??= new List<string>();
                        
                        foreach (ActorData actor in residents)
                        {
                            ActorRegistry.Register(actor);
                            regions[x, y].residents.Add(actor.ActorId);
                            regions[x, y].unspawnedActors.Add(actor.ActorId);
                        }
                    }
                    
                    // Choose a biome.
                    regions[x, y].biome = PickBiome(x, y, biomeMap[x, y], seed);
                }
            }
            
            // Set the coasts for each region.
            for (int y = 0; y < sizeY; y++)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    if (regions[x, y].isWater) continue;
                    regions[x, y].coasts = new List<Direction>();
                    
                    if (x == 0 || regions[x - 1, y].isWater)
                        regions[x, y].coasts.Add(Direction.Left);
                        
                    if (x == sizeX - 1 || regions[x + 1, y].isWater) 
                        regions[x, y].coasts.Add(Direction.Right);
                        
                    if (y == sizeY - 1 || regions[x, y + 1].isWater)
                        regions[x, y].coasts.Add(Direction.Up);
                        
                    if (y == 0 || regions[x, y - 1].isWater)
                        regions[x, y].coasts.Add(Direction.Down);
                }
            }
            return new WorldMap(worldName, new Vector2Int(sizeX, sizeY), regions);
        }

        private static string PickBiome(int x, int y, float noiseMapValue, int seed)
        {
            noiseMapValue = Mathf.Clamp(noiseMapValue, 0f, 0.999f);

            ICollection<Biome> biomes = ContentLibrary.Instance.Biomes.GetAll();
            WeightedTable table = new WeightedTable(
                biomes.Select(biome => new KeyValuePair<string, float>(biome.Id, biome.Frequency)).ToList());
            return table.Get(noiseMapValue);
        }
    }
}
