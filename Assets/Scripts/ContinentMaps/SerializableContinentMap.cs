
namespace ContinentMaps
{
    [System.Serializable]
    public class SerializableContinentMap
    {
        public Vector2IntSerializable dimensions;
        public string continentName;
        public SerializableRegionMap[,] regions;
        public RegionInfo[,] regionInfo;

        public WorldMap ToNonSerializable()
        {
            WorldMap newMap = new WorldMap(continentName, dimensions.ToNonSerializable(), regionInfo);
            
            for (int x = 0; x < regions.GetLength(0); x++)
            {
                for (int y = 0; y < regions.GetLength(1); y++)
                {
                    if (regions[x,y] != null)
                        newMap.regions[x,y] = regions[x,y].ToNonSerializable();
                }
            }

            return newMap;
        }
    }

    public static class ContinentMapExtension
    {
        // Creates a serializable continent map from the given continent map.
        public static SerializableContinentMap ToSerializable(this WorldMap original)
        {
            SerializableContinentMap serializable = new SerializableContinentMap();
            serializable.continentName = original.continentName;
            serializable.dimensions = original.dimensions.ToSerializable();
            serializable.regionInfo = original.regionInfo;

            SerializableRegionMap[,] rMap = new SerializableRegionMap[serializable.dimensions.x, serializable.dimensions.y];
            for (int x = 0; x < original.regions.GetLength(0); x++)
            {
                for (int y = 0; y < original.regions.GetLength(1); y++)
                {
                    if (original.regions[x,y] != null)
                        rMap[x,y] =  new SerializableRegionMap(original.regions[x,y]);
                }
            }
            serializable.regions = rMap;

            return serializable;
        }
    }
}