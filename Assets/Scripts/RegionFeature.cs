using UnityEngine;
using UnityEngine.UI;

/*
 * Represents a major feature of a region; for example, a homestead, a town, etc.
 */
public abstract class RegionFeature : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private Sprite mapIcon;

    /*
     * The unique identifier of this feature.
     */
    public string Id => id;
    
    /*
    * The icon of this feature which appears on the map, or null if this feature
    * doesn't show an icon.
    */
    public Sprite MapIcon => mapIcon;

    /*
     * Adds this feature to the given region. Returns false iff the feature was
     * unable to be added.
     */
    public abstract bool AttemptApply(RegionMap region, int seed);
}