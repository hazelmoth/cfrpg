using UnityEngine;

namespace ContinentMaps
{
    public abstract class WorldGenerator : MonoBehaviour, IWorldGenerator
    {
        public abstract WorldMap Generate(int seed);
    }
}
