﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ContinentMaps
{
    public static class ContinentNameGenerator
    {
        public static string Generate()
        {
            List<string> suffixes = new List<string> { "green", "hay", "may", "ribb", "good", "shore", "blue", "old", "kidd", "cad", "broth", "bare", "dead" };
            List<string> roots = new List<string> { "fort", "way", "wood", "camp", "stead", "vale", "peak", "moor", "lamb", "mare", "hare", "brow", "ling", "thing", "run", "meat" };
            return suffixes.PickRandom().ToUpper() + roots.PickRandom() + " Island";
        }
    }
}