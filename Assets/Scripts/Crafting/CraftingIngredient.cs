using System;
using System.Collections.Generic;
using MyBox;

namespace Crafting
{
    [System.Serializable]
    public class CraftingIngredient
    {
        public int count;
        public string itemBaseId;
        public TagHandlingMode tagHandlingMode;

        [ConditionalField("tagHandlingMode", false, TagHandlingMode.RequireTags)]
        public CollectionWrapper<RequiredItemTag> requiredTags;

        public enum TagHandlingMode
        {
            DisallowTags,
            RequireTags,
        }

        [Serializable]
        public struct RequiredItemTag
        {
            public string key;
            public string value;
        }

        public IDictionary<string, string> GetTagsDict()
        {
            Dictionary<string, string> tags = new();
            if (tagHandlingMode == TagHandlingMode.DisallowTags) return tags;

            foreach (RequiredItemTag tag in requiredTags.Value)
            {
                tags.Add(tag.key, tag.value);
            }

            return tags;
        }
    }
}
