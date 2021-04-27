using System.Collections.Generic;
using ContentLibraries;
using UnityEngine;

namespace ContinentMaps
{
    public static class ContinentGenerator
    {
        //   y
        //
        //   ^
        //   |
        //   |
        //   *----->   x

        private const float WaterLevel = 0.35f;

        public static ContinentMap Generate(int sizeX, int sizeY, int seed)
        {
            Random.InitState(seed);
            
            string worldName = ContinentNameGenerator.Generate(seed);
            
            float[,] heightmap = new float[sizeX, sizeY];
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
                    
                    // Set regions below the water level to be water.
                    regions[x, y].topography = heightmap[x, y] < WaterLevel ? RegionTopography.Water : RegionTopography.Land;
                    
                    // Sometimes add a random feature.
                    if (regions[x,y].topography != RegionTopography.Water && Random.value < 0.1f)
                    {
                        RegionFeature feature = ContentLibrary.Instance.RegionFeatures.Get(ContentLibrary.Instance
                            .RegionFeatures
                            .GetIdList().PickRandom());
                        
                        regions[x, y].feature = feature.Id;
                        
                        // Generate any residents that come with this feature.
                        List<ActorData> residents = feature.GenerateResidents();
                        
                        regions[x, y].residents ??= new List<string>();
                        regions[x, y].unspawnedActors ??= new List<string>();
                        
                        foreach (ActorData actor in residents)
                        {
                            ActorRegistry.Register(actor);
                            regions[x, y].residents.Add(actor.actorId);
                            regions[x, y].unspawnedActors.Add(actor.actorId);
                        }
                    }
                    
                    // Choose a biome at random.
                    regions[x, y].biome = ContentLibrary.Instance.Biomes.GetAllIds().PickRandom();
                }
            }
            
            // Now set the topography for coastal regions by checking whether
            // they're adjacent to water.
            for (int y = 0; y < sizeY; y++)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    if (regions[x, y].topography != RegionTopography.Land) continue;
                    
                    if (x == 0 || regions[x - 1, y].topography == RegionTopography.Water)
                        regions[x, y].topography = RegionTopography.WestCoast;
                        
                    else if (x == sizeX - 1 || regions[x + 1, y].topography == RegionTopography.Water) 
                        regions[x, y].topography = RegionTopography.EastCoast;
                        
                    else if (y == sizeY - 1 || regions[x, y + 1].topography == RegionTopography.Water) 
                        regions[x, y].topography = RegionTopography.NorthCoast;
                        
                    else if (y == 0 || regions[x, y - 1].topography == RegionTopography.Water) 
                        regions[x, y].topography = RegionTopography.SouthCoast;
                }
            }
            return new ContinentMap(worldName, new Vector2Int(sizeX, sizeY), regions);
        }
    }
}