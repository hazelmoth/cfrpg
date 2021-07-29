using System.Collections.Generic;
using System.Collections.Immutable;
using ContentLibraries;
using UnityEngine;

/*
 * Represents a major feature of a region; for example, a homestead, a town, etc.
 */
namespace FeatureGenerators
{
    public abstract class RegionFeatureGenerator : ScriptableObject, IContentItem
    {
        [SerializeField] private string id;
        [SerializeField] private Sprite mapIcon;
            
        /// The unique identifier of this feature.
        public string Id => id;

        /// The icon of this feature which appears on the map, or null if this feature
        /// doesn't show an icon.
        public Sprite MapIcon => mapIcon;

        /// Returns a list of actors who will be residents in the region where this
        /// feature is located. These actors are not yet registered.
        public virtual IEnumerable<ActorData> GenerateResidents()
        {
            return ImmutableList.Create<ActorData>();
        }

        /// Adds this feature to the given region. Returns false iff the feature was
        /// unable to be added.
        public abstract bool AttemptApply(RegionMap region, RegionInfo info, int seed);
    }
}