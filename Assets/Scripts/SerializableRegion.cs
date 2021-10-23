using System;
using Newtonsoft.Json;

/// All the information associated with a particular region, including the actual map data.
/// This version of the class is serializable, for save file purposes.
[Serializable]
public class SerializableRegion
{
    public RegionInfo info;
    public SerializableRegionMap data;

    [JsonConstructor]
    public SerializableRegion() { }

    public SerializableRegion (Region original)
    {
        info = original.info;
        data = new SerializableRegionMap(original.data);
    }
}
