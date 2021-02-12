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

        private const float WaterLevel = 0.3f;

        public static ContinentMap Generate(int sizeX, int sizeY, int seed)
        {
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
                    heightmap[x, y] = GenerationHelper.EllipseGradient(point, sizeX, sizeY);
                    // Average the height with some noise
                    heightmap[x, y] = GenerationHelper.Avg(heightmap[x, y], GenerationHelper.UniformSimplex(x, y, seed));
                    heightmap[x, y] = Mathf.Clamp01(heightmap[x, y]);
                    regions[x, y].topography = heightmap[x, y] < WaterLevel ? RegionTopography.Water : RegionTopography.Land;
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