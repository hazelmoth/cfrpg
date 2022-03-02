/// All the information associated with a particular region, including the actual map data.
public class Region
{
    public RegionInfo info;
    public RegionMap data;

    public Region(RegionInfo info, RegionMap data = null)
    {
        this.info = info;
        this.data = data;
    }

    public string Id => info.Id;
}
