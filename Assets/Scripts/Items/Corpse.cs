using System.Collections.Generic;
using ContentLibraries;
using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName = "NewItem", menuName = "Items/Corpse")]
    public class Corpse : ItemData
    {
        /// Save tag for whose corpse this is
        private const string ActorIdModifier = "actor_id";

        // This tag doesn't add new info, but we include it to make butchering easier.
        // In theory, it also allows us to make generic corpses that don't have an actor.
        private const string ActorRaceModifier = "race";

        public override string GetItemName(IDictionary<string, string> modifiers)
        {
            if (modifiers.TryGetValue(ActorRaceModifier, out string raceId))
            {
                ActorRace race = ContentLibrary.Instance.Races.Get(raceId);
                return race.Name + " " + base.GetItemName(modifiers);
            }
            return base.GetItemName(modifiers);
        }

        public override Sprite GetIcon(IDictionary<string, string> modifiers)
        {
            if (modifiers.TryGetValue(ActorRaceModifier, out string raceId))
            {
                ActorRace race = ContentLibrary.Instance.Races.Get(raceId);
                return race.ItemSprite;
            }
            return base.GetIcon(modifiers);
        }
    }
}
