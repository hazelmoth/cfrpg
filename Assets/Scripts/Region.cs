/// All the information associated with a particular region, including the actual map data.
public class Region
{
    public RegionInfo info;
    public RegionMap data;

    public string Id => info.Id;
}
