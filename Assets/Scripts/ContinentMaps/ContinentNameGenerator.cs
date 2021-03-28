using System.Collections.Generic;
using System.Globalization;

namespace ContinentMaps
{
    public static class ContinentNameGenerator
    {
        public static string Generate(int seed)
        {
            List<string> suffixes = new List<string> { "green", "hay", "may", "ribb", "good", "shore", "blue", "old", "kidd", "cad", "broth", "bare", "dead" };
            List<string> roots = new List<string> { "fort", "way", "wood", "camp", "stead", "vale", "peak", "moor", "lamb", "mare", "hare", "brow", "ling", "thing", "run", "meat" };
            string result = suffixes.SeededPickRandom(seed) + roots.SeededPickRandom(seed * 2) + " Island";
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(result);
        }
    }
}
