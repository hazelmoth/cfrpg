using UnityEngine;

namespace ActorAnim
{
    public abstract class BaseActorRace : ScriptableObject, IActorRace
    {
        public abstract string Id { get; }
        public abstract string Name { get; }
        public abstract float Speed { get; }
        public abstract float MaxHealth { get; }
        public abstract bool Humanoid { get; }
        public abstract bool SupportsHair { get; }
        public abstract CompoundWeightedTable ButcherDrops { get; }
        public abstract Sprite CorpseItemSprite { get; }

        public abstract Vector2 GetItemPosition(Direction dir);

        public abstract IActorSpriteController CreateSpriteController(Actor actor);
    }
}
