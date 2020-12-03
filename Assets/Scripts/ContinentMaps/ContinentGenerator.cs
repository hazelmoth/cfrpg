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

        private const float waterLevel = 0.3f;

        public static ContinentMap Generate(int sizeX, int sizeY, int seed)
        {
            ContinentBlueprint continent = new ContinentBlueprint();

            float[,] heightmap = new float[sizeX, sizeY];
            RegionType[,] regions = new RegionType[sizeX, sizeY];

            for (int y = 0; y < sizeY; y++)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    Vector2 point = new Vector2(x, y);
                    heightmap[x, y] = GenerationHelper.EllipseGradient(point, sizeX, sizeY);
                    // Average the height with some noise
                    heightmap[x, y] = GenerationHelper.Avg(heightmap[x, y], GenerationHelper.UniformSimplex(x, y, seed));
                    heightmap[x, y] = Mathf.Clamp01(heightmap[x, y]);
                    regions[x, y] = heightmap[x, y] < waterLevel ? RegionType.Water : RegionType.Land;
                }
            }
            for (int y = 0; y < sizeY; y++)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    if (regions[x, y] != RegionType.Land) continue;
                    if (regions[x - 1, y] == RegionType.Water) regions[x, y] = RegionType.WestCoast;
                    else if (regions[x + 1, y] == RegionType.Water) regions[x, y] = RegionType.EastCoast;
                    else if (regions[x, y + 1] == RegionType.Water) regions[x, y] = RegionType.NorthCoast;
                    else if (regions[x, y - 1] == RegionType.Water) regions[x, y] = RegionType.SouthCoast;
                }
            }


            continent.sizeX = sizeX;
            continent.sizeY = sizeY;
            continent.regions = regions;
            continent.Name = ContinentNameGenerator.Generate();
            return new ContinentMap(continent);
        }
    }
}