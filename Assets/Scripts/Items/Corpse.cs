using System.Collections.Generic;
using ContentLibraries;
using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName = "NewItem", menuName = "Items/Corpse", order = 1)]
    public class Corpse : ItemData
    {
        private const string ActorIdModifier = "actor_id";

        public override string GetItemName(IDictionary<string, string> modifiers)
        {
            if (modifiers.TryGetValue(ActorIdModifier, out string actorId))
            {
                ActorData actor = ActorRegistry.Get(actorId).data;
                ActorRace race = ContentLibrary.Instance.Races.Get(actor.RaceId);
                return race.Name + " " + base.GetItemName(modifiers);
            }
            return base.GetItemName(modifiers);
        }

        public override Sprite GetIcon(IDictionary<string, string> modifiers)
        {
            if (modifiers.TryGetValue(ActorIdModifier, out string actorId))
            {
                ActorData actor = ActorRegistry.Get(actorId).data;
                ActorRace race = ContentLibrary.Instance.Races.Get(actor.RaceId);
                return race.ItemSprite;
            }
            return base.GetIcon(modifiers);
        }
    }
}